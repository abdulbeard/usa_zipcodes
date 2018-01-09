using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USA_ZipCodes.Geojson
{
    public class DbGeographyService
    {
        private static Rootobject _unitedStates;
        private static Rootobject _usRegions;
        static DbGeographyService()
        {
            _unitedStates = JsonConvert.DeserializeObject<Rootobject>(
                System.IO.File.ReadAllText(@"C:\Development\Github\usa_zipcodes\USA_ZipCodes\Geojson\us_outline_geojson.geojson"), new DbGeographyConverter());
            _usRegions = JsonConvert.DeserializeObject<Rootobject>(
                System.IO.File.ReadAllText(@"C:\Development\Github\usa_zipcodes\USA_ZipCodes\Geojson\us_regions_geojson.geojson"), new DbGeographyConverter());
        }
        public bool IsInTheUnitedStates(DbGeography dbGeography)
        {
            return dbGeography.Intersects(_unitedStates.features[0].geometry.coordinates);
        }

        public string GetRegionInTheUnitedStates(DbGeography dbGeography)
        {
            foreach(var region in _usRegions.features)
            {
                if (region.geometry.coordinates.Intersects(dbGeography)){
                    return region.properties.Name;
                }
            }
            return string.Empty;
        }
    }
}
