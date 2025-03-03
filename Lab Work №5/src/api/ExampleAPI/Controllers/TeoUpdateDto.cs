namespace ExampleAPI.Controllers
{
	public class TeoUpdateDto
	{
		public int? filial { get; set; }
		public List<int>? settlements { get; set; }
		public int? housing_type { get; set; }
		public string? name { get; set; }
		public string? author { get; set; }
	}
}
