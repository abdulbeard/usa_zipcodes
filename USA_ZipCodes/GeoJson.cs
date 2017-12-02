namespace USA_ZipCodes
{
    public class Rootobject
    {
        public string type { get; set; }
        public string name { get; set; }
        public Crs crs { get; set; }
        public Feature[] features { get; set; }
    }

    public class Crs
    {
        public string type { get; set; }
        public Properties properties { get; set; }
    }

    public class Properties
    {
        public string name { get; set; }
    }

    public class Feature
    {
        public string type { get; set; }
        public Properties1 properties { get; set; }
        public Geometry geometry { get; set; }
    }

    public class Properties1
    {
        public string Name { get; set; }
        public string description { get; set; }
        public object timestamp { get; set; }
        public object begin { get; set; }
        public object end { get; set; }
        public string altitudeMode { get; set; }
        public int tessellate { get; set; }
        public int extrude { get; set; }
        public int visibility { get; set; }
        public object drawOrder { get; set; }
        public object icon { get; set; }
        public string ZCTA5CE10 { get; set; }
        public string AFFGEOID10 { get; set; }
        public string GEOID10 { get; set; }
        public string ALAND10 { get; set; }
        public string AWATER10 { get; set; }
    }

    public class Geometry
    {
        public string type { get; set; }
        public System.Data.Spatial.DbGeography coordinates { get; set; }
    }

}
