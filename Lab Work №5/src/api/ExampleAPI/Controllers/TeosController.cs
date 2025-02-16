using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExampleAPI.Model;

namespace ExampleAPI.Controllers
{
    [Route("teos")]
    [ApiController]
    public class TeosController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public TeosController(DatabaseContext context)
        {
            _context = context;
        }

        static readonly Dictionary<string, string> OrderByMapping = new Dictionary<string, string>
        {
            { "name", "teo.Name" },
            { "author", "teo.Author" },
            { "filial", "filial.Name" },
            { "date_created", "teo.DateCreated" },
            { "date_updated", "teo.DateUpdated" }
        };

        // GET: teos
        [HttpGet]
        public async Task<ActionResult> GetTeos([FromQuery] string? sortBy, [FromQuery] string? sortOrder, [FromQuery] int? count, [FromQuery] int? skip)
        {
            count ??= 10;
            sortBy ??= "date_created";
            sortOrder ??= "desc";
            skip ??= 0;
            
            if (count < 1 || count > 100)
                return BadRequest(new { invalid_params = new string[] { "count" } });
            else if (skip < 0)
                return BadRequest(new { invalid_params = new string[] { "skip" } });
            else if (sortOrder != "asc" && sortOrder != "desc")
                return BadRequest(new { invalid_params = new string[] { "sortOrder" } });
            else if (!OrderByMapping.TryGetValue(sortBy, out sortBy))
                return BadRequest(new { invalid_params = new string[] { "sortBy" } });

            Console.WriteLine($"SELECT teo.* FROM teos AS teo INNER JOIN filials AS filial ORDER BY {sortBy} {sortOrder} LIMIT {count} OFFSET {skip}");

            var res = await _context.Teos.FromSqlRaw(
                $"SELECT teo.* FROM teos AS teo INNER JOIN filials AS filial ORDER BY {sortBy} {sortOrder} LIMIT {count} OFFSET {skip}"
            ).Select(teo => new TeoDto()
            {
                id = teo.ID,
                name = teo.Name,
                author = teo.Author,
                date_created = teo.DateCreated.ToString("s"),
                date_updated = teo.DateUpdated.ToString("s"),
                filial = new FilialDto()
                {
                    id = teo.Filial.ID,
                    name = teo.Filial.Name
                },
                housing_type = new HousingTypeDto()
                {
                    id = teo.HousingType.ID,
                    name = teo.HousingType.Name
                },
                settlements = teo.Settlements.Select(x => new SettlementDto()
                {
                    id = x.ID,
                    name = x.Name,
                }).ToList()
            }).ToListAsync();

            return Ok(new {
                teos = res
            });
        }

        // POST: teos/5
        [HttpPost]
        public async Task<ActionResult> CreateTeo([FromBody] TeoUpdateDto data)
        {
            if (data.name == null)
                return BadRequest(new
                {
                    invalid_params = new object[]
                    {
                        new
                        {
                            name = "name",
                            value = data.name
                        }
                    }
                });

            if (data.author == null)
                return BadRequest(new
                {
                    invalid_params = new object[]
                    {
                        new
                        {
                            name = "author",
                            value = data.author
                        }
                    }
                });

            if (data.filial == null)
                return BadRequest(new
                {
                    invalid_params = new object[]
                    {
                        new
                        {
                            name = "filial",
                            value = data.filial
                        }
                    }
                });

            var filial = await _context.Filials.FirstOrDefaultAsync(x => x.ID == data.filial);
            if (filial == null)
                return BadRequest(new
                {
                    invalid_params = new object[]
                    {
                        new
                        {
                            name = "filial",
                            value = data.filial
                        }
                    }
                });

            if (data.housing_type == null)
                return BadRequest(new
                {
                    invalid_params = new object[]
                    {
                        new
                        {
                            name = "housing_type",
                            value = data.housing_type
                        }
                    }
                });
            
            var housing_type = await _context.HousingTypes.FirstOrDefaultAsync(x => x.ID == data.housing_type);
            if (housing_type == null)
                return BadRequest(new
                {
                    invalid_params = new object[]
                    {
                        new
                        {
                            name = "housing_type",
                            value = data.housing_type
                        }
                    }
                });

            
            if (data.settlements == null || data.settlements.Count == 0)
                return BadRequest(new
                {
                    invalid_params = new object[]
                    {
                        new
                        {
                            name = "settlements",
                            values = data.settlements
                        }
                    }
                });

            var settlments = await _context.Settlements.Where(x => data.settlements.Contains(x.ID) && x.Filial.ID == filial.ID).ToListAsync();
            if (settlments.Count != data.settlements.Count)
                return BadRequest(new
                {
                    invalid_params = new object[]
                    {
                        new
                        {
                            name = "settlments",
                            values = data.settlements.Where(x => !settlments.Any(y => y.ID == x)).ToList()
                        }
                    }
                });


            var teo = new Teo()
            {
                Name = data.name,
                Author = data.author,
                Filial = filial,
                HousingType = housing_type,
                Settlements = settlments
            };

            _context.Teos.Add(teo);
            await _context.SaveChangesAsync();

            return Ok(new TeoDto()
            {
                id = teo.ID,
                name = teo.Name,
                author = teo.Author,
                date_created = teo.DateCreated.ToString("s"),
                date_updated = teo.DateUpdated.ToString("s"),
                filial = new FilialDto()
                {
                    id = teo.Filial.ID,
                    name = teo.Filial.Name
                },
                housing_type = new HousingTypeDto()
                {
                    id = teo.HousingType.ID,
                    name = teo.HousingType.Name
                },
                settlements = teo.Settlements.Select(x => new SettlementDto()
                {
                    id = x.ID,
                    name = x.Name,
                }).ToList()
            });
        }

        // PATCH: teos/5
        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdateTeo(int id, [FromBody] TeoUpdateDto data)
        {
            if (data.name == null && data.author == null && data.settlements == null && data.housing_type == null && data.filial == null)
                return BadRequest();

            var teo = await _context.Teos.Include(x => x.Filial).Include(x => x.HousingType).Include(x => x.Settlements).FirstOrDefaultAsync(x => x.ID == id);
            if (teo == null)
                return NotFound();

            string? name = null;
            if (data.name != null)
                name = data.name;

            string? author = null;
            if (data.author != null)
                author = data.author;

            HousingType? housing_type = null;
            if (data.housing_type != null)
            {
                housing_type = await _context.HousingTypes.FirstOrDefaultAsync(x => x.ID == data.housing_type);
                if (housing_type == null)
                    return BadRequest(new
                    {
                        invalid_params = new object[]
                        {
                            new
                            {
                                name = "housing_type",
                                value = data.housing_type
                            }
                        }
                    });
            }
            
            List<Settlement>? settlements = null;

            Filial? filial = null;
            if (data.filial != null)
            {
                filial = await _context.Filials.FirstOrDefaultAsync(x => x.ID == data.filial);
                if (filial == null)
                    return BadRequest(new
                    {
                        invalid_params = new object[]
                        {
                            new
                            {
                                name = "filial",
                                value = data.filial
                            }
                        }
                    });

                if (filial.ID != teo.Filial.ID && data.settlements == null)
                    settlements = new List<Settlement>();
            }

            if (data.settlements != null)
            {
                var filial_for_validation = filial ?? teo.Filial;
                settlements = await _context.Settlements.Where(x => data.settlements.Contains(x.ID) && x.Filial.ID == filial_for_validation.ID).ToListAsync();
                if (settlements.Count != data.settlements.Count)
                    return BadRequest(new
                    {
                        invalid_params = new object[]
                        {
                            new
                            {
                                name = "settlments",
                                values = data.settlements.Where(x => !settlements.Any(y => y.ID == x)).ToList()
                            }
                        }
                    });
            }

            if (name != null)
                teo.Name = name;

            if (author != null)
                teo.Author = author;

            if (housing_type != null)
                teo.HousingType = housing_type;

            if (filial != null)
                teo.Filial = filial;

            if (settlements != null)
                teo.Settlements = settlements;

            await _context.SaveChangesAsync();

            return Ok(new TeoDto()
            {
                id = teo.ID,
                name = teo.Name,
                author = teo.Author,
                date_created = teo.DateCreated.ToString("s"),
                date_updated = teo.DateUpdated.ToString("s"),
                filial = new FilialDto()
                {
                    id = teo.Filial.ID,
                    name = teo.Filial.Name
                },
                housing_type = new HousingTypeDto()
                {
                    id = teo.HousingType.ID,
                    name = teo.HousingType.Name
                },
                settlements = teo.Settlements.Select(x => new SettlementDto()
                {
                    id = x.ID,
                    name = x.Name,
                }).ToList()
            });
        }

        // DELETE: api/Teos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTeo(int id)
        {
            var teo = await _context.Teos.FirstOrDefaultAsync(x => x.ID == id);
            if (teo == null)
                return NotFound();

            _context.Teos.Remove(teo);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
