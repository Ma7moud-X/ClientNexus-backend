using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;

namespace ClientNexus.Domain.ValueObjects
{
    public class MapPoint : Point
    {
        public MapPoint(double longitude, double latitude)
            : base(longitude, latitude)
        {
            SRID = 4326;
        }
    }
}
