namespace ExampleAPI.Model
{
	#nullable disable
	public class Settlement
	{
		public int ID { get; set; }

		public string Name { get; set; }

		public Filial Filial { get; set; }
	}
}
