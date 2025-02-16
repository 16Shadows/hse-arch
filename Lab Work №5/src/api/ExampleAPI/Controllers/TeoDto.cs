namespace ExampleAPI.Controllers
{
    #nullable disable
	public class TeoDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string author { get; set; }
        public string date_created { get; set; }
        public string date_updated { get; set; }
        public FilialDto filial { get; set; }
        public List<SettlementDto> settlements { get; set; }
        public HousingTypeDto housing_type { get; set; }
    }
}
