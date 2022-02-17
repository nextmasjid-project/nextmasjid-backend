namespace NextMasjid.Backend.Core
{
    public class ScoreDetailedModel
    {
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
        public int Value { get; set; }
        public string MosqueDensity { get; set; }
        public string ExpectedPrayers { get; set; }
        public string PopulationDensity { get; set; }
        public string NearestMosqueDistance { get; set; }

        public string FirstNearestMasjidName { get; set; }
        public string FirstNearestMasjidDistance { get; set; }

        public string SecondNearestMasjidName { get; set; }
        public string SecondNearestMasjidDistance { get; set; }

        public string ThirdNearestMasjidName { get; set; }
        public string ThirdNearestMasjidDistance { get; set; }

    }
}