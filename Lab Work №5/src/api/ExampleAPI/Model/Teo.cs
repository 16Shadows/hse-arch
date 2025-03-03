namespace ExampleAPI.Model
{
	#nullable disable
	public class Teo : ISupportsDateCreated, ISupportsDateUpdated
	{
		public int ID { get; set; }
		
		public string Name { get; set; }

		public string Author { get; set; }

		public DateTime DateCreated { get; set; }

		public DateTime DateUpdated { get; set; }

		public List<Settlement> Settlements { get; set; }

		public Filial Filial { get; set; }

		public HousingType HousingType { get; set; }
	}
}
