using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LoadVantage.Infrastructure.Data
{
	public class LoadVantageDbContext : IdentityDbContext<User, Role, Guid>
	{
		public LoadVantageDbContext(DbContextOptions<LoadVantageDbContext> options)
			: base(options)
		{

		}

        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Broker> Brokers { get; set; }
		public DbSet<Dispatcher> Dispatchers { get; set; }
		public DbSet<Truck> Trucks { get; set; }
		public DbSet<Driver> Drivers { get; set; }
		public DbSet<PostedLoad> PostedLoads { get; set; }
		public DbSet<BookedLoad> BookedLoads { get; set; }
		public DbSet<BilledLoad> BilledLoads { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Driver>()
				.HasOne(d => d.Truck)
				.WithOne(t => t.Driver)
				.HasForeignKey<Driver>(d => d.TruckId);

			modelBuilder.Entity<User>()
				.HasDiscriminator<string>("Position")
				.HasValue<Dispatcher>("Dispatcher")
				.HasValue<Broker>("Broker")
				.HasValue<Administrator>("Administrator");


		}
    }
}

