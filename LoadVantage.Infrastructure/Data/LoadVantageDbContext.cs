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

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}

		public DbSet<Broker> Brokers { get; set; }
		public DbSet<Dispatcher> Dispatchers { get; set; }
		public DbSet<Driver> Drivers { get; set; }
	}
}
