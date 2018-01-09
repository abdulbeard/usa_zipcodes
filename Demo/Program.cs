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
            var logFile = @"C:\Development\Github\usa_zipcodes\USA_ZipCodes\Geojson\zipMinMax.txt";

            var zipCodes = JsonConvert.DeserializeObject<Rootobject>(System.IO.File.ReadAllText(@"C:\Development\Github\usa_zipcodes\USA_ZipCodes\Geojson\geoJson\geoJson.geojson"), new DbGeographyConverter());
            var dict = new Dictionary<string, MinMax>();
            foreach (var zip in zipCodes.features)
            {
                var listLatValues = new List<double>();
                var listLongValues = new List<double>();
                for(int i = 1; i <= zip.geometry.coordinates.PointCount; i++)
                {
                    var point = zip.geometry.coordinates.PointAt(i);
                    listLatValues.Add(point.Latitude.Value);
                    listLongValues.Add(point.Longitude.Value);
                }
                dict.Add(zip.properties.ZCTA5CE10, new MinMax()
                {
                    minLat = listLatValues.Min(),
                    maxLat = listLatValues.Max(),
                    minLong = listLongValues.Min(),
                    maxLong = listLongValues.Max()
                });
            }
            System.IO.File.WriteAllText(logFile, JsonConvert.SerializeObject(dict));
        }


        //static void Main(string[] args)
        //{
        //    var logFile = @"C:\Development\Github\usa_zipcodes\USA_ZipCodes\Geojson\logfile.txt";
        //    var lines = System.IO.File.ReadAllLines(logFile);
        //    var yolo = lines.Select(x => {
        //        var splitResult = x.Split(':');
        //        return new KeyValuePair<string, string>(splitResult[0], splitResult[1]);
        //    }).GroupBy(x=>x.Key).ToDictionary(x=>x.Key, x=>x.Select(y=>y.Value).ToList());
        //    System.IO.File.WriteAllText(@"C:\Development\Github\usa_zipcodes\USA_ZipCodes\Geojson\statesAndZips.json", Newtonsoft.Json.JsonConvert.SerializeObject(yolo));
        //}

        //static void Main(string[] args)
        //{
        //    var logFile = @"C:\Development\Github\usa_zipcodes\USA_ZipCodes\Geojson\logfile.txt";

        //    var states = JsonConvert.DeserializeObject<Rootobject>(System.IO.File.ReadAllText(@"C:\Development\Github\usa_zipcodes\USA_ZipCodes\Geojson\us_states\us_states.geojson"), new DbGeographyConverter());
        //    var zipCodes = JsonConvert.DeserializeObject<Rootobject>(System.IO.File.ReadAllText(@"C:\Development\Github\usa_zipcodes\USA_ZipCodes\Geojson\geoJson\geoJson.geojson"), new DbGeographyConverter());
        //    var statesAndAbbreviations = getStateAndAbbrDict();
        //    foreach (var state in states.features)
        //    {
        //        foreach (var zip in zipCodes.features)
        //        {
        //            if (zip.geometry.coordinates.Intersects(state.geometry.coordinates))
        //            {
        //                System.IO.File.AppendAllText(logFile, $"{state.properties.Name}=>{zip.properties.ZCTA5CE10}{Environment.NewLine}");
        //            }
        //        }
        //    }
        //    System.IO.File.AppendAllText(logFile, "All done baby!");
        //}

        static Dictionary<string, string> getStateAndAbbrDict()
        {
            var dictStateFullNameToAbbr = new Dictionary<string, string>();
            var lines = System.IO.File.ReadLines(@"C:\Development\Github\usa_zipcodes\USA_ZipCodes\Geojson\stateAbbreviations.csv");
            foreach (var line in lines)
            {
                var lineSplit = line.Split(',');
                dictStateFullNameToAbbr.Add(lineSplit[0], lineSplit[1]);
            }
            return dictStateFullNameToAbbr;
        }

        //static void Main(string[] args)
        //{
        //    var states = new List<StateZips>();
        //    var textFileLines = System.IO.File.ReadAllLines(@"C:\Users\AbdulBeard\Downloads\zips_per_state.csv");
        //    foreach (var line in textFileLines)
        //    {
        //        var state = new StateZips { Zips = new List<string>() };
        //        var firstFive = line.Substring(0, 5);
        //        state.Zips.Add(firstFive);
        //        var firstThree = line.Substring(0, 3);
        //        var charAtSixthPos = line.ElementAt(5);
        //        var charArray = line.ToCharArray();
        //        var rangeBeginning = 0;
        //        var rangeEnd = 0;
        //        var currentBuffer = new List<char>();
        //        for (var i = 5; i < line.Length; i++)
        //        {
        //            if (char.IsWhiteSpace(charArray[i]))
        //            {
        //                continue;
        //            }
        //            else if (charArray[i] == ',')
        //            {
        //                if (i == 5) continue;
        //                if (currentBuffer.Count == 2)
        //                {
        //                    var sdf = firstThree + new String(currentBuffer.ToArray()).Trim();
        //                    state.Zips.Add(sdf.PadLeft(5, '0'));
        //                    currentBuffer = new List<char>();
        //                }
        //            }
        //            else if (charArray[i] == '-')
        //            {
        //                rangeBeginning = currentBuffer.Count == 2 ?
        //                    int.Parse(new string(new List<char> { firstThree[0], firstThree[1], firstThree[2], currentBuffer.ElementAt(0), currentBuffer.ElementAt(1) }.ToArray())) : 
        //                    int.Parse(firstFive);
        //                rangeEnd = int.Parse(new string(new List<char> { firstThree[0], firstThree[1], firstThree[2], charArray[i + 1], charArray[i + 2] }.ToArray()));
        //                state.Zips.AddRange(getZipsForRange(rangeBeginning, rangeEnd));
        //                i += 2;
        //            }
        //            else if (char.IsLetter(charArray[i])) { break; }
        //            else if (char.IsNumber(charArray[i]))
        //            {
        //                currentBuffer.Add(charArray[i]);
        //            }
        //        }
        //        var commaSplit = line.Split(',');
        //        var commaSplitSpaceSplit = commaSplit[commaSplit.Length - 1].Split(' ');
        //        var stateCode = commaSplitSpaceSplit[commaSplitSpaceSplit.Length - 2];
        //        var city = commaSplit[commaSplit.Length - 1].Split(new string[] { stateCode }, StringSplitOptions.None)[0];
        //        state.City = city.Trim();
        //        state.State = stateCode.Trim();
        //        state.Zips = state.Zips.Distinct().ToList();
        //        states.Add(state);
        //    }
        //    //var marysVille = states.Where(x => x.State == "MI");
        //    //marysVille = marysVille.Where(x=>x.City == "MARYSVILLE");

        //    //test code
        //    var allZips = states.SelectMany(x => x.Zips).ToList();
        //    var postDistinctCount = allZips.Distinct().ToList();
        //    var except = postDistinctCount.Except(allZips);
        //    var dupe = allZips.GroupBy(x => x).Where(y => y.Count() > 1).Select(z=>z);
        //}

        //static void Main(string[] args)
        //{
        //    var dictStateFullNameToAbbr = new Dictionary<string, string>();
        //    var lines = System.IO.File.ReadLines(@"C:\Development\Github\usa_zipcodes\USA_ZipCodes\Geojson\stateAbbreviations.csv");
        //    foreach (var line in lines)
        //    {
        //        var lineSplit = line.Split(',');
        //        dictStateFullNameToAbbr.Add(lineSplit[0], lineSplit[1]);
        //    }

        //    var regionsAndStates = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(
        //        System.IO.File.ReadAllText(@"C:\Development\Github\usa_zipcodes\USA_ZipCodes\Geojson\results.json"));
        //    foreach (var state in regionsAndStates)
        //    {
        //        for (int i = 0; i < state.Value.Count; i++)
        //        {
        //            var stateFullName = state.Value.ElementAt(i);
        //            state.Value[i] = dictStateFullNameToAbbr[stateFullName];
        //        }
        //    }








        //    var usStatesGeojson = @"C:\Development\Github\usa_zipcodes\USA_ZipCodes\Geojson\us_states\us_states.geojson";
        //    var usZipCodesGeojson = @"C:\Development\Github\usa_zipcodes\USA_ZipCodes\Geojson\geoJson\geoJson.geojson";
        //    var usregions = @"C:\Development\Github\usa_zipcodes\USA_ZipCodes\Geojson\us_regions_geojson.geojson";
        //    var us = @"C:\Development\Github\usa_zipcodes\USA_ZipCodes\Geojson\us_outline_geojson.geojson";

        //    var results = new Dictionary<string, List<string>>();

        //    var usOutline = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(
        //            System.IO.File.ReadAllText(us
        //                ), new DbGeographyConverter());
        //    var usRegions = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(
        //            System.IO.File.ReadAllText(usregions
        //                ), new DbGeographyConverter());
        //    var usStates = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(
        //            System.IO.File.ReadAllText(usStatesGeojson
        //                ), new DbGeographyConverter());

        //    foreach (var region in usRegions.features)
        //    {
        //        results.Add(region.properties.Name, new List<string>());
        //    }

        //    foreach (var state in usStates.features)
        //    {
        //        foreach (var region in usRegions.features)
        //        {
        //            if (state.geometry.coordinates.Intersects(region.geometry.coordinates))
        //            {
        //                results[region.properties.Name].Add(state.properties.Name);
        //            }
        //        }
        //    }

        //    System.IO.File.WriteAllText(@"C:\Development\Github\usa_zipcodes\USA_ZipCodes\Geojson\results.json", JsonConvert.SerializeObject(results));

        //    var ro =
        //        Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(
        //            System.IO.File.ReadAllText(usZipCodesGeojson
        //                ), new DbGeographyConverter());
        //    var sdflksdfs = System.Data.Spatial.DbGeography.MultiPolygonFromText("", 4326);
        //    var slkjdflkjsdfjklslkjdf = System.Data.Spatial.DbGeography.PolygonFromText("", 4326);
        //    sdflksdfs.Intersects(slkjdflkjsdfjklslkjdf);
        //}

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

    

    class MinMax
    {
        public double minLat { get; set; }
        public double maxLat { get; set; }
        public double minLong { get; set; }
        public double maxLong { get; set; }
    }
}
