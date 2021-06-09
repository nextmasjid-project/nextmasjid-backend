using System;
using System.Collections.Generic;
using System.Linq;

namespace NextMasjid.Backend.Core
{
    public class Masjid
    {
        public string MasjidID { get; set; }
        public string Name { get; set; }
        public GeoPoint Location { get; set; }
        public int CityID { get; set; }
        public bool IsGrandMosque { get; set; }


        public static Dictionary<Masjid, int> GetMasjidsDistanceToLatLng(IEnumerable<Masjid> masjids, double lat, double lng)
        {
            Dictionary<Masjid, int> result = new Dictionary<Masjid, int>();

            foreach(var masjid in masjids)
            {
                var masjidInMeters = Util.UtilMath.ConvertToMetersPoint(masjid.Location);
                var pointInMeters = Util.UtilMath.ConvertToMetersPoint(new GeoPoint(lat, lng));

                double distance = Math.Sqrt(Util.UtilMath.CalculateDistanceFast(masjidInMeters.Lat, masjidInMeters.Lng, pointInMeters.Lat, pointInMeters.Lng));
                result.Add(masjid, Convert.ToInt32(distance));
            }


            return result.OrderBy(m => m.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
        }


    }
}
