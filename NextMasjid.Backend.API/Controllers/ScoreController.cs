using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NextMasjid.Backend.API.Models;
using System.Linq;

namespace NextMasjid.Backend.API.Controllers
{
    [ApiController]
    [Route("score")]
    public class ScoreController : BaseController<ScoreController>
    {

        [HttpGet("byArea/{swLat}/{swLng}/{neLat}/{neLng}/{step}")]
        public IEnumerable<ScoreModel> Get(double swLat, double swLng, double neLat, double neLng, int step)
        {

            if (step < 1)
                step = 1;

            // dual step
            var subScores90 = Core.Score.SearchAreaFast(Scores, new Core.GeoPoint(swLat, swLng), new Core.GeoPoint(neLat, neLng), 2 * step).Where(s => s.Value >= 85);
            var subScores80 = Core.Score.SearchAreaFast(Scores, new Core.GeoPoint(swLat, swLng), new Core.GeoPoint(neLat, neLng), 4 * step).Where(s => s.Value >= 70 && s.Value < 85);
            //var subScores70 = Core.Score.SearchAreaFast(Scores, new Core.GeoPoint(swLat, swLng), new Core.GeoPoint(neLat, neLng), 32).Where(s => s.Value >= 70 && s.Value < 80);
            var subScoresElse = Core.Score.SearchAreaFast(Scores, new Core.GeoPoint(swLat, swLng), new Core.GeoPoint(neLat, neLng), 32 * step).Where(s => s.Value <70 );

            var result90 = subScores90.Select(s => new ScoreModel() { Lat = ((decimal)s.Key.Item1 / 10000), Lng = ((decimal)s.Key.Item2 / 10000), Value = s.Value });
            var result80 = subScores80.Select(s => new ScoreModel() { Lat = ((decimal)s.Key.Item1 / 10000), Lng = ((decimal)s.Key.Item2 / 10000), Value = s.Value });
            //var result70 = subScores70.Select(s => new ScoreModel() { Lat = ((double)s.Key.Item1 / 10000), Lng = ((double)s.Key.Item2 / 10000), Value = s.Value });
            var resultsElse = subScoresElse.Select(s => new ScoreModel() { Lat = ((decimal)s.Key.Item1 / 10000), Lng = ((decimal)s.Key.Item2 / 10000), Value = s.Value });

            var result = new List<ScoreModel>();
            result.AddRange(result90);
            result.AddRange(result80);
            //result.AddRange(result70);
            result.AddRange(resultsElse);

            return result;

        }

        //[HttpGet("byArea/{swLat}/{swLng}/{neLat}/{neLng}/")]
        //public IEnumerable<ScoreModel> Get(double swLat, double swLng, double neLat, double neLng)
        //{

        //    var subScores = Core.Score.SearchAreaFast(Scores, new Core.GeoPoint(swLat, swLng), new Core.GeoPoint(neLat, neLng), 2).Where(s => s.Value >= 50);
            
        //    return subScores.Select(s => new ScoreModel() { Lat = ((double)s.Key.Item1 / 10000), Lng = ((double)s.Key.Item2 / 10000), Value = s.Value }); ;

        //}

        [HttpGet("byPoint/{lat}/{lng}")]
        public ScoreModel Get(double lat, double lng)
        {
            int value = Backend.Core.Score.SearchPoint(Scores, new Core.GeoPoint(lat, lng));
            decimal nLat = (int)(lat * 10000) / (decimal)10000;
            decimal nLng = (int)(lng * 10000) / (decimal)10000;

            return new ScoreModel() { Lat =  nLat, Lng = nLng, Value = value };
        }


        [HttpGet("byPointDetails/{lang}/{lat}/{lng}")]
        public ScoreDetailedModel Get(string lang, double lat, double lng)
        {
            int value = Backend.Core.Score.SearchPoint(Scores, new Core.GeoPoint(lat, lng));

            decimal nLat = (int)(lat * 10000) / (decimal)10000;
            decimal nLng = (int)(lng * 10000) / (decimal)10000;


            // get city where the

            var city = Provinces.SelectMany(p => p.Cities).FirstOrDefault(c => c.Polygons.First().Contains(lat, lng));

            // get nearest 3 mosuqes
            var nearestMasjids = Core.Masjid.GetMasjidsDistanceToLatLng(Masjids.Where(m => m.CityID == city.CityID), lat, lng);


            while(nearestMasjids.Count < 3)
            {
                nearestMasjids.Add(new Core.Masjid() { Name = "لايوجد" }, 0);
            }

            string density = "عالية";
            if (city.RankInDensity == 1)
                density = "منخفضة";
            else if (city.RankInDensity == 2 || city.RankInDensity == 3)
                density = "متوسطة";
            

            return new ScoreDetailedModel()
            {
                Lat = nLat,
                Lng = nLng,
                Value = value,
                ExpectedPrayers = "غير متوفر حالياً", // based on distance from nearest mosquee 
                MosqueDensity = Convert.ToDecimal( nearestMasjids.Count * 1000 / city.Population).ToString("#.##"),
                PopulationDensity = density,
                NearestMosqueDistance = nearestMasjids.First().Value.ToString(),

                FirstNearestMasjidName = nearestMasjids.First().Key.Name,
                SecondNearestMasjidName = nearestMasjids.Skip(1).First().Key.Name,
                ThirdNearestMasjidName = nearestMasjids.Skip(2).First().Key.Name,

                FirstNearestMasjidDistance = nearestMasjids.First().Value.ToString(),
                SecondNearestMasjidDistance = nearestMasjids.Skip(1).First().Value.ToString(),
                ThirdNearestMasjidDistance = nearestMasjids.Skip(2).First().Value.ToString()
            };
        }
    }
}
