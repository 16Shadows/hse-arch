using AuthAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Controllers
{
	[Route("users")]
	[ApiController]
	public class UsersController : Controller
	{
		private readonly DatabaseContext _context;

		public UsersController(DatabaseContext context)
		{
			_context = context;
		}

		[HttpGet("{username}/permissions/{permission}")]
		public async Task<ActionResult> GetPermission(string username, string permission)
		{
			Console.WriteLine(username);
			Console.WriteLine(permission);
			var perm = await _context.Permissions.Where(x => x.Name == permission && x.Holders.Any(y => y.Username == username)).FirstOrDefaultAsync();
			if (perm == null)
				return NotFound();

			return Ok(new {
				name=perm.Name
			});
		}
	}
}
