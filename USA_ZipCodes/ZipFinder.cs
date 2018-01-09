using System;
using System.Collections.Generic;
using System.Data.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USA_ZipCodes
{
    public class ZipFinder : IZipFinder
    {
        public ZipFinderResult ExtendedFind(double latitude, double longitude)
        {
            throw new NotImplementedException();
        }

        public ZipFinderResult ExtendedFind(DbGeography dbGeography)
        {
            throw new NotImplementedException();
        }

        public string Find(double latitude, double longitude)
        {
            var point = DbGeography.FromText($"POINT({longitude} {latitude})");
            return string.Empty;
        }
    }
}
