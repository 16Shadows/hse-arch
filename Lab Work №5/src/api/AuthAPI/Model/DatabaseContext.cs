using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AuthAPI.Model
{
	public class DatabaseContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Permission> Permissions { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			NpgsqlConnectionStringBuilder sb = new NpgsqlConnectionStringBuilder();
			sb.Host = Environment.GetEnvironmentVariable("POSTGRES_HOST");
			sb.Port = int.Parse(Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432");
			sb.Username = Environment.GetEnvironmentVariable("POSTGRES_USER");
			sb.Password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
			sb.Database = Environment.GetEnvironmentVariable("POSTGRES_DATABASE");
			optionsBuilder.UseNpgsql(sb.ToString());
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>(b =>
			{
				b.ToTable("users");
			
				b.HasKey(x => x.ID);
				b.HasIndex(x => x.Username).IsUnique(true);

				b.HasMany(x => x.Permissions).WithMany(x => x.Holders);
			});

			modelBuilder.Entity<Permission>(b =>
			{
				b.ToTable("_permissions");
				b.HasKey(x => x.ID);
				b.Property(x => x.Name).IsRequired().IsUnicode(true);
				b.HasAlternateKey(x => x.Name);
			});
		}
	}
}
