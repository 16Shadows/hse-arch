namespace ExampleAPI.Model
{
	#nullable disable
	public class Filial
	{
		public int ID { get; set; }

		public string Name { get; set; }

		public List<Settlement> Settlements { get; set; }
	}
}
