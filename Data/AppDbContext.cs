using CinemaDashboard.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CinemaDashboard.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<MovieImage> MovieImages { get; set; }
        public DbSet<ApplicationUserOTP> ApplicationUserOTPs { get; set; }
        public DbSet<FavoriteItem> FavoriteItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<BookingSeat> BookingSeats { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MovieActor>()
                .HasKey(ma => new { ma.MovieId, ma.ActorId });

            modelBuilder.Entity<MovieActor>()
                .HasOne(ma => ma.Movie)
                .WithMany(m => m.MovieActors)
                .HasForeignKey(ma => ma.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MovieActor>()
                .HasOne(ma => ma.Actor)
                .WithMany(a => a.MovieActors)
                .HasForeignKey(ma => ma.ActorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MovieActor>()
                .HasKey(ma => new { ma.MovieId, ma.ActorId });

            modelBuilder.Entity<ApplicationUserOTP>()
                .HasOne(o => o.ApplicationUser)
                .WithMany()
                .HasForeignKey(o => o.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUserOTP>()
                .HasIndex(o => o.ApplicationUserId);

            modelBuilder.Entity<Movie>()
               .Property(m => m.Price)
               .HasPrecision(18, 2);

            modelBuilder.Entity<Order>().Property(x => x.TotalAmount).HasPrecision(18, 2);
            modelBuilder.Entity<Payment>().Property(x => x.Amount).HasPrecision(18, 2);

            modelBuilder.Entity<FavoriteItem>().HasIndex(x => new { x.ApplicationUserId, x.MovieId }).IsUnique();
            modelBuilder.Entity<BookingSeat>().HasIndex(x => new { x.MovieId, x.RowNumber, x.SeatNumber }).IsUnique();

            modelBuilder.Entity<FavoriteItem>().HasOne(x => x.ApplicationUser).WithMany().HasForeignKey(x => x.ApplicationUserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<FavoriteItem>().HasOne(x => x.Movie).WithMany(x => x.FavoriteItems).HasForeignKey(x => x.MovieId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Order>().HasOne(x => x.ApplicationUser).WithMany().HasForeignKey(x => x.ApplicationUserId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Order>().HasOne(x => x.Movie).WithMany(x => x.Orders).HasForeignKey(x => x.MovieId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<BookingSeat>().HasOne(x => x.Order).WithMany(x => x.Seats).HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<BookingSeat>().HasOne(x => x.Movie).WithMany().HasForeignKey(x => x.MovieId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Payment>().HasOne(x => x.Order).WithOne(x => x.Payment).HasForeignKey<Payment>(x => x.OrderId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
