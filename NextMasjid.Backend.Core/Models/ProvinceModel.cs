namespace NextMasjid.Backend.Core
{
    public class ProvinceModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public CityModel[] Cities { get; set; }

    }
}