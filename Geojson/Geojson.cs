using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geojson
{

    public class Rootobject
    {
        public string type { get; set; }
        public Geometry geometry { get; set; }
    }

    public class Geometry
    {
        public string type { get; set; }
        public System.Data.Spatial.DbGeography coordinates { get; set; }
    }

}
