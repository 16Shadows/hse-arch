using ExampleAPI.Middleware;
using ExampleAPI.Model;

namespace ExampleAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddDbContext<DatabaseContext>();
			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			string authServer = Environment.GetEnvironmentVariable("AUTH_SERVER")!;
			app.UseAuthMiddleware((username, permission) => $"http://{authServer}/users/{username}/permissions/{permission}");
			app.MapControllers();

			app.Run();
		}
	}
}