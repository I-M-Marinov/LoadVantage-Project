using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LoadVantage.Infrastructure.Data
{
	public class LoadVantageDbContext : IdentityDbContext<BaseUser, Role, Guid>
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
        public DbSet<DeliveredLoad> DeliveredLoads { get; set; }
        public DbSet<UserImage> UsersImages { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<BaseUser>()
				.HasOne(u => u.Role)
				.WithMany()  // if a Role can have multiple users
				.HasForeignKey(u => u.RoleId);

			// One Truck with One Driver 
			modelBuilder.Entity<Driver>()
				.HasOne(d => d.Truck)
				.WithOne(t => t.Driver)
				.HasForeignKey<Driver>(d => d.TruckId);

			// One Dispatcher with Many Drivers
			modelBuilder.Entity<Driver>()
				.HasOne(d => d.Dispatcher)
				.WithMany(d => d.Drivers) 
				.HasForeignKey(d => d.DispatcherId);

			// Position as dicriminator ---> Dispatcher, Broker or Administrator 
			modelBuilder.Entity<BaseUser>()
				.HasDiscriminator<string>("Position")
				.HasValue<User>("User")
				.HasValue<Dispatcher>("Dispatcher")
				.HasValue<Broker>("Broker")
				.HasValue<Administrator>("Administrator");

			// One Posted Load with One Load 
			modelBuilder.Entity<Load>()
                .HasOne(l => l.PostedLoad)
                .WithOne(pl => pl.Load)
                .HasForeignKey<PostedLoad>(pl => pl.LoadId);

            // One Booked Load with One Load 
			modelBuilder.Entity<Load>()
                .HasOne(l => l.BookedLoad)
                .WithOne(bl => bl.Load)
                .HasForeignKey<BookedLoad>(bl => bl.LoadId);

			// One Delivered Load with One Load 
			modelBuilder.Entity<Load>()
                .HasOne(l => l.DeliveredLoad)
                .WithOne(b => b.Load)
                .HasForeignKey<DeliveredLoad>(b => b.LoadId);

			// One Sender with Many Sent Messages 
			modelBuilder.Entity<ChatMessage>()
	            .HasOne(cm => cm.Sender)
	            .WithMany(u => u.SentMessages)
	            .HasForeignKey(cm => cm.SenderId)
	            .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes

			// One Receiver with Many Received Messages 
			modelBuilder.Entity<ChatMessage>()
	            .HasOne(cm => cm.Receiver)
	            .WithMany(u => u.ReceivedMessages)
	            .HasForeignKey(cm => cm.ReceiverId)
	            .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes
		}
    }
}

