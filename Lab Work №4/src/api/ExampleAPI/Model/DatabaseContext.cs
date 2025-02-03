using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ExampleAPI.Model
{
	public class DatabaseContext : DbContext
	{
		public DbSet<Teo> Teos { get; set; }
		public DbSet<Filial> Filials { get; set; }
		public DbSet<HousingType> HousingTypes { get; set; }
		public DbSet<Settlement> Settlements { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			SqliteConnectionStringBuilder sb = new SqliteConnectionStringBuilder();
			sb.DataSource = "./database.db";

			optionsBuilder.UseSqlite(sb.ToString());
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Filial>(b =>
			{
				b.HasKey(x => x.ID);
				b.Property(x => x.Name).IsRequired().IsUnicode();
				b.ToTable("filials");
			});

			modelBuilder.Entity<HousingType>(b =>
			{
				b.HasKey(x => x.ID);
				b.Property(x => x.Name).IsRequired().IsUnicode();
				b.ToTable("housing_types");
			});

			modelBuilder.Entity<Settlement>(b =>
			{
				b.HasKey(x => x.ID);
				b.Property(x => x.Name).IsRequired().IsUnicode();
				b.HasOne(x => x.Filial).WithMany(x => x.Settlements);
				b.ToTable("settlements");
			});

			modelBuilder.Entity<Teo>(b =>
			{
				b.HasKey(x => x.ID);
				b.Property(x => x.Name).IsRequired().IsUnicode();
				b.Property(x => x.Author).IsRequired().IsUnicode();
				b.Property(x => x.DateCreated).IsRequired();
				b.Property(x => x.DateUpdated).IsRequired();

				b.HasOne(x => x.Filial).WithMany();
				b.HasMany(x => x.Settlements).WithMany();
				b.HasOne(x => x.HousingType).WithMany();
				b.ToTable("teos");
			});
		}

		public override int SaveChanges(bool acceptAllChangesOnSuccess)
		{
			DateTime now = DateTime.UtcNow;

			//Присвоить время создания сущностям, которые отслеживают время создания

			var newEntires = ChangeTracker.Entries() //Из всех сущностей с изменениями
											.Where(x => x.State == EntityState.Added) //Выбрать только что добавленные
											.Select(x => x.Entity); //Взять саму сущность каждой записи
			
			foreach (var entry in newEntires.OfType<ISupportsDateCreated>()) //Выбрать сущности, которые реализуют интерфейс ISupportsDateCreated
				entry.DateCreated = now;

			//Присвоить время изменения сущностям, которые отслеживают время изменения

			var changedEntries = ChangeTracker.Entries() //Из всех сущностей с изменениями
												.Where(x => x.State == EntityState.Added || x.State == EntityState.Modified) //Выбрать только что изменённые
												.Select(x => x.Entity); //Взять саму сущность каждой записи

			foreach (var entry in changedEntries.OfType<ISupportsDateUpdated>()) //Выбрать сущности, которые реализуют интерфейс ISupportsDateCreated
				entry.DateUpdated = now;

			return base.SaveChanges(acceptAllChangesOnSuccess);
		}

		public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
		{
			DateTime now = DateTime.UtcNow;

			//Присвоить время создания сущностям, которые отслеживают время создания

			var newEntires = ChangeTracker.Entries() //Из всех сущностей с изменениями
											.Where(x => x.State == EntityState.Added) //Выбрать только что добавленные
											.Select(x => x.Entity); //Взять саму сущность каждой записи
			
			foreach (var entry in newEntires.OfType<ISupportsDateCreated>()) //Выбрать сущности, которые реализуют интерфейс ISupportsDateCreated
				entry.DateCreated = now;

			//Присвоить время изменения сущностям, которые отслеживают время изменения

			var changedEntries = ChangeTracker.Entries() //Из всех сущностей с изменениями
												.Where(x => x.State == EntityState.Added || x.State == EntityState.Modified) //Выбрать только что изменённые
												.Select(x => x.Entity); //Взять саму сущность каждой записи

			foreach (var entry in changedEntries.OfType<ISupportsDateUpdated>()) //Выбрать сущности, которые реализуют интерфейс ISupportsDateCreated
				entry.DateUpdated = now;

			return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
		}
	}
}
