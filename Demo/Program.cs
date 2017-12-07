using System;
using System.Collections.Generic;
using System.Data.Spatial;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using USA_ZipCodes;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Types;
using Newtonsoft.Json.Linq;


namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var states = new List<StateZips>();
            var textFileLines = System.IO.File.ReadAllLines(@"C:\Users\AbdulBeard\Downloads\zips_per_state.csv");
            foreach (var line in textFileLines)
            {
                var state = new StateZips { Zips = new List<string>() };
                var firstFive = line.Substring(0, 5);
                state.Zips.Add(firstFive);
                var firstThree = line.Substring(0, 3);
                var charAtSixthPos = line.ElementAt(5);
                var charArray = line.ToCharArray();
                var rangeBeginning = 0;
                var rangeEnd = 0;
                var currentBuffer = new List<char>();
                for (var i = 5; i < line.Length; i++)
                {
                    if (char.IsWhiteSpace(charArray[i]))
                    {
                        continue;
                    }
                    else if (charArray[i] == ',')
                    {
                        if (i == 5) continue;
                        if (currentBuffer.Count == 2)
                        {
                            var sdf = firstThree + new String(currentBuffer.ToArray());
                            state.Zips.Add(sdf.PadLeft(5, '0'));
                            currentBuffer = new List<char>();
                        }
                    }
                    else if (charArray[i] == '-')
                    {
                        rangeBeginning = currentBuffer.Count == 2 ?
                            int.Parse(new string(new List<char> { firstThree[0], firstThree[1], firstThree[2], currentBuffer.ElementAt(0), currentBuffer.ElementAt(1) }.ToArray())) : 
                            int.Parse(firstFive);
                        rangeEnd = int.Parse(new string(new List<char> { firstThree[0], firstThree[1], firstThree[2], charArray[i + 1], charArray[i + 2] }.ToArray()));
                        state.Zips.AddRange(getZipsForRange(rangeBeginning, rangeEnd));
                        i += 2;
                    }
                    else if (char.IsLetter(charArray[i])) { break; }
                    else if (char.IsNumber(charArray[i]))
                    {
                        currentBuffer.Add(charArray[i]);
                    }
                }
                var commaSplit = line.Split(',');
                var commaSplitSpaceSplit = commaSplit[commaSplit.Length - 1].Split(' ');
                var stateCode = commaSplitSpaceSplit[commaSplitSpaceSplit.Length - 2];
                var city = commaSplit[commaSplit.Length - 1].Split(stateCode.ToArray())[0];
                state.City = city;
                state.State = stateCode;
                states.Add(state);
            }
        }

        public static List<string> getZipsForRange(int x, int y)
        {
            var result = new List<string> { x.ToString().PadLeft(5, '0'), y.ToString().PadLeft(5, '0') };
            x++;
            while (x < y)
            {
                result.Add(x.ToString().PadLeft(5, '0'));
                x++;
            }
            return result;
        }
    }

    public class StateZips
    {
        public string State { get; set; }
        public List<string> Zips { get; set; }
        public string City { get; set; }
    }

    //static void Main(string[] args)
    //{
    //    var usStatesGeojson = @"C:\Users\az185030\Downloads\cb_2016_us_state_500k\us_states.geojson";
    //    var usZipCodesGeojson = @"C:\Users\az185030\Downloads\cb_2016_us_zcta510_500k_kml\geoJson.geojson";
    //    var ro =
    //        Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(
    //            System.IO.File.ReadAllText(usStatesGeojson
    //                ), new DbGeographyConverter());
    //    var sdflksdfs = System.Data.Spatial.DbGeography.MultiPolygonFromText("", 4326);
    //    var slkjdflkjsdfjklslkjdf = System.Data.Spatial.DbGeography.PolygonFromText("", 4326);
    //    sdflksdfs.Intersects(slkjdflkjsdfjklslkjdf);
    //}

    public class DbGeographyConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(Geometry))
            {
                return true;
            }
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            var type = jo["type"].ToString();
            DbGeography result = null;
            if (type == "MultiPolygon")
            {
                var coords = jo["coordinates"];
                var multipolygon = coords.ToObject<IEnumerable<IEnumerable<IEnumerable<IEnumerable<double>>>>>();
                result = Parse(multipolygon);
            }
            else if (type == "Polygon")
            {
                var coords = jo["coordinates"];
                var polygon = coords.ToObject<IEnumerable<IEnumerable<IEnumerable<double>>>>();
                result = Parse(polygon);
            }
            return new Geometry
            {
                coordinates = result,
                type = type
            };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public static DbGeography Parse(IEnumerable<IEnumerable<IEnumerable<IEnumerable<double>>>> input)
        {
            //return DbGeography.MultiPolygonFromText(
            //    MakePolygonValid("MULTIPOLYGON(((10 10,50 10,50 50,10 50,10 10),(20 20, 30 20, 30 30, 20 30,20 20)),((30 30,60 20,60 40,30 30)))"),
            //    DbGeography.DefaultCoordinateSystemId);
            var mpWkt = GetMultiPolygonWkt(input);
            return DbGeography.MultiPolygonFromText(mpWkt, DbGeography.DefaultCoordinateSystemId);
        }

        public static DbGeography Parse(IEnumerable<IEnumerable<IEnumerable<double>>> input)
        {
            return DbGeography.PolygonFromText(GetPolygonWkt(input), DbGeography.DefaultCoordinateSystemId);
        }

        public static string GetMultiPolygonWkt(IEnumerable<IEnumerable<IEnumerable<IEnumerable<double>>>> input)
        {
            var result = new StringBuilder("MULTIPOLYGON(");
            var inputCount = input.Count();
            var inputIteration = 1;

            foreach (var inp in input)
            {
                result.Append(MakePolygonValid(GetPolygonWkt(inp)).Replace("POLYGON", ""));
                if (inputIteration != inputCount)
                {
                    result.Append(",");
                }
                inputIteration++;
            }
            result.Append(")");
            return result.ToString();
        }

        public static string GetPolygonWkt(IEnumerable<IEnumerable<IEnumerable<double>>> input)
        {
            var result = new StringBuilder("POLYGON(");
            var inputCount = input.Count();
            var inputIteration = 1;
            foreach (var set in input)
            {
                result.Append("(");
                var reverseSet = set.Reverse();
                var setCount = set.Count();
                var setIteration = 1;
                foreach (var subset in set)
                {
                    result.Append($"{subset.ElementAt(0)} {subset.ElementAt(1)}");
                    if (setIteration != setCount)
                    {
                        result.Append(",");
                    }
                    setIteration++;
                }
                result.Append(")");
                if (inputIteration != inputCount)
                {
                    result.Append(",");
                }
                inputIteration++;
            }
            result.Append(")");
            return result.ToString();
        }

        private static string approach_two(string wkt)
        {
            //First, get the area defined by the well-known text using left-hand rule
            var sqlGeography =
            SqlGeography.STGeomFromText(new SqlChars(wkt), DbGeography.DefaultCoordinateSystemId).MakeValid();

            //Now get the inversion of the above area
            var invertedSqlGeography = sqlGeography.ReorientObject();

            //Whichever of these is smaller is the enclosed polygon, so we use that one.
            if (sqlGeography.STArea() > invertedSqlGeography.STArea())
            {
                sqlGeography = invertedSqlGeography;
            }
            var sqlGeographyAsString = sqlGeography.ToString();
            var returnValue = DbGeography.FromText(sqlGeographyAsString).AsText();
            return returnValue;
        }

        private static string approach_one(string polygonWkt)
        {
            var result = "";
            try
            {
                // Create a DbGeography instance from a WKT string
                DbGeography polygon = DbGeography.FromText(polygonWkt);

                // If the polygon area is larger than an earth hemisphere (510 Trillion m2 / 2), we know it needs to be fixed
                if (polygon.Area.HasValue && polygon.Area.Value > 255000000000000L)
                {
                    // Convert our DbGeography polygon into a SqlGeography object for the ReorientObject() call
                    var sqlPolygon = SqlGeography.STGeomFromWKB(new System.Data.SqlTypes.SqlBytes(polygon.AsBinary()), 4326);

                    // ReorientObject will flip the polygon so the outside becomes the inside
                    sqlPolygon = sqlPolygon.ReorientObject();

                    // Convert the SqlGeography object back into a WKT string
                    polygon = DbGeography.FromBinary(sqlPolygon.STAsBinary().Value);
                    result = polygon.AsText();
                }
            }
            catch (Exception ex)
            {
                result = string.Empty;
            }
            return result;
        }


        private static string MakePolygonValid(string polygonWkt)
        {
            return approach_two(polygonWkt);
            //return approach_one(polygonWkt);
        }
    }
}
