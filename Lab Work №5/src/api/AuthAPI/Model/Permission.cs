namespace AuthAPI.Model
{
	#nullable disable
	public class Permission
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public List<User> Holders { get; set; }
	}
}
