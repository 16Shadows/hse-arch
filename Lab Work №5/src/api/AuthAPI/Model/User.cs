namespace AuthAPI.Model
{
	#nullable disable
	public class User
	{
		public int ID {get; set;}
		public string Username { get; set; }
		public List<Permission> Permissions { get; set; }
	}
}