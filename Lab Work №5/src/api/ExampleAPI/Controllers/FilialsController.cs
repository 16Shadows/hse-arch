using ExampleAPI.Middleware;
using ExampleAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExampleAPI.Controllers
{
	[Route("filials")]
	[ApiController]
	public class FilialsController : ControllerBase
	{
		private readonly DatabaseContext _context;

		public FilialsController(DatabaseContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult> GetFilials()
		{
			return Ok(new
			{
				filials = await _context.Filials.Select(x => new FilialDto()
				{
					id = x.ID,
					name = x.Name
				}).ToListAsync()
			});
		}
		
		[HttpGet("{id}/settlements")]
        [AuthPermission("teo.get")]
		public async Task<ActionResult> GetSettlements(int id)
		{
			var filial = await _context.Filials.Where(x => x.ID == id).FirstOrDefaultAsync();

			if (filial == null)
				return NotFound();

			return Ok(new
			{
				settlements = await _context.Settlements.Where(x => x.Filial.ID == id).Select(x => new SettlementDto()
				{
					id = x.ID,
					name = x.Name
				}).ToListAsync()
			});
		}

	}
}
