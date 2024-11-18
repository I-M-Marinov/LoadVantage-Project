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
        public DbSet<Load> Loads { get; set; }
        public DbSet<BookedLoad> BookedLoads { get; set; }
        public DbSet<PostedLoad> PostedLoads { get; set; }
        public DbSet<BilledLoad> BilledLoads { get; set; }
        public DbSet<UserImage> UsersImages { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Driver>()
				.HasOne(d => d.Truck)
				.WithOne(t => t.Driver)
				.HasForeignKey<Driver>(d => d.TruckId);

			modelBuilder.Entity<UserImage>()
				.HasOne(ui => ui.User)
				.WithOne(u => u.UserImage)
				.HasForeignKey<UserImage>(ui => ui.UserId);

			modelBuilder.Entity<User>()
				.HasDiscriminator<string>("Position")
				.HasValue<Dispatcher>("Dispatcher")
				.HasValue<Broker>("Broker")
				.HasValue<Administrator>("Administrator");

            modelBuilder.Entity<Load>()
                .HasOne(l => l.PostedLoad)
                .WithOne(pl => pl.Load)
                .HasForeignKey<PostedLoad>(pl => pl.LoadId);

            modelBuilder.Entity<Load>()
                .HasOne(l => l.BookedLoad)
                .WithOne(bl => bl.Load)
                .HasForeignKey<BookedLoad>(bl => bl.LoadId);

            modelBuilder.Entity<Load>()
                .HasOne(l => l.BilledLoad)
                .WithOne(b => b.Load)
                .HasForeignKey<BilledLoad>(b => b.LoadId);

            modelBuilder.Entity<User>()
	            .HasOne(bu => bu.UserImage) 
	            .WithOne(ui => ui.User)    
	            .HasForeignKey<UserImage>(ui => ui.UserId); 

			modelBuilder.Entity<ChatMessage>()
	            .HasOne(cm => cm.Sender)
	            .WithMany(u => u.SentMessages)
	            .HasForeignKey(cm => cm.SenderId)
	            .OnDelete(DeleteBehavior.Restrict); // Prevents cascading deletes

            modelBuilder.Entity<ChatMessage>()
	            .HasOne(cm => cm.Receiver)
	            .WithMany(u => u.ReceivedMessages)
	            .HasForeignKey(cm => cm.ReceiverId)
	            .OnDelete(DeleteBehavior.Restrict); // Prevents cascading deletes
		}
    }
}

