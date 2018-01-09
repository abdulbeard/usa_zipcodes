using System;
using System.Collections.Generic;
using System.Data.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USA_ZipCodes
{
    public interface IZipFinder
    {
        string Find(double latitude, double longitude);
        ZipFinderResult ExtendedFind(double latitude, double longitude);
        ZipFinderResult ExtendedFind(DbGeography dbGeography);
    }

    public class ZipFinderResult
    {
        public ZipData ExactMatch { get; set; }
        public List<ZipData> ClosestZipcodes { get; set; }
    }

    public class ZipData
    {
        public string ZipCode { get; set; }
        public double DistanceInMeters { get; set; }
        public string State { get; set; }
        public string Region { get; set; }
    }
}
