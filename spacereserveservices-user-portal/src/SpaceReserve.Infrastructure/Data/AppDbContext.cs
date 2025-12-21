using Microsoft.EntityFrameworkCore;
using SpaceReserve.Infrastructure.Entities;

namespace SpaceReserve.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {      
    }

    public DbSet<User> Users { get; set; }
    public DbSet<ModeOfWorkModel> ModeOfWorkModels { get; set; }
    public DbSet<DesignationModel> DesignationModels { get; set; }
    public DbSet<CityModel> CityModels { get; set; }
    public DbSet<UserWorkingDay> UserWorkingDays { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingStatusModel> BookingStatus { get; set; }
    public DbSet<FloorModel> FloorModels { get; set; }
    public DbSet<ColumnModel> ColumnModels { get; set; }
    public DbSet<WorkingDay> WorkingDays { get; set; }
    public DbSet<Seat> Seats { get; set; }
    public DbSet<SeatConfiguration> SeatConfigurations { get; set; }
    public DbSet<NotificationModel> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().ToTable("User", "User");
        modelBuilder.Entity<UserWorkingDay>().ToTable("UserWorkingDay", "User");
        modelBuilder.Entity<Booking>().ToTable("Booking", "User");
        modelBuilder.Entity<SeatConfiguration>().ToTable("SeatConfiguration", "User");
        modelBuilder.Entity<ModeOfWorkModel>().ToTable("ModeOfWork", "Ref");
        modelBuilder.Entity<DesignationModel>().ToTable("Designation", "Ref");
        modelBuilder.Entity<CityModel>().ToTable("City", "Ref");
        modelBuilder.Entity<BookingStatusModel>().ToTable("BookingStatus", "Ref");
        modelBuilder.Entity<FloorModel>().ToTable("Floor", "Ref");
        modelBuilder.Entity<ColumnModel>().ToTable("Column", "Ref");
        modelBuilder.Entity<WorkingDay>().ToTable("WorkingDay", "Ref");
        modelBuilder.Entity<Seat>().ToTable("Seat", "User");
        modelBuilder.Entity<NotificationModel>().ToTable("Notification", "User");

        // USER
        modelBuilder.Entity<User>()
            .HasMany(u => u.Bookings)
            .WithOne(b => b.User)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.UserWorkingDays)
            .WithOne(uw => uw.User)
            .HasForeignKey(uw => uw.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // BOOKING
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.BookingStatusModel)
            .WithMany()
            .HasForeignKey(b => b.BookingStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Seat)
            .WithMany(s=> s.Bookings)
            .HasForeignKey(b => b.SeatId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Creator)
            .WithMany()
            .HasForeignKey(b => b.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Modifier)
            .WithMany()
            .HasForeignKey(b => b.ModifiedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Deleter)
            .WithMany()
            .HasForeignKey(b => b.DeletedBy)
            .OnDelete(DeleteBehavior.Restrict);

        // SEAT CONFIGURATION
        modelBuilder.Entity<SeatConfiguration>()
            .HasOne(sc => sc.User)
            .WithMany()
            .HasForeignKey(sc => sc.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SeatConfiguration>()
            .HasOne(sc => sc.Seat)
            .WithMany(s=> s.SeatConfigurations)
            .HasForeignKey(sc => sc.SeatId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SeatConfiguration>()
            .HasOne(sc => sc.Creator)
            .WithMany()
            .HasForeignKey(sc => sc.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SeatConfiguration>()
            .HasOne(sc => sc.Modifier)
            .WithMany()
            .HasForeignKey(sc => sc.ModifiedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SeatConfiguration>()
            .HasOne(sc => sc.Deleter)
            .WithMany()
            .HasForeignKey(sc => sc.DeletedBy)
            .OnDelete(DeleteBehavior.Restrict);

        // USER WORKING DAY
        modelBuilder.Entity<UserWorkingDay>()
            .HasOne(uw => uw.WorkingDay)
            .WithMany(wd => wd.UserWorkingDays)
            .HasForeignKey(uw => uw.WorkingDayId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserWorkingDay>()
            .HasOne(uw => uw.Creator)
            .WithMany()
            .HasForeignKey(uw => uw.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserWorkingDay>()
            .HasOne(uw => uw.Modifier)
            .WithMany()
            .HasForeignKey(uw => uw.ModifiedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserWorkingDay>()
            .HasOne(uw => uw.Deleter)
            .WithMany()
            .HasForeignKey(uw => uw.DeletedBy)
            .OnDelete(DeleteBehavior.Restrict);

        // SEAT
        modelBuilder.Entity<Seat>()
            .HasOne(s => s.ColumnModel)
            .WithMany(c => c.Seats)
            .HasForeignKey(s => s.ColumnId)
            .OnDelete(DeleteBehavior.Restrict);

        // COLUMN
        modelBuilder.Entity<ColumnModel>()
            .HasOne(c => c.FloorModel)
            .WithMany(f => f.ColumnModels)
            .HasForeignKey(c => c.FloorId)
            .OnDelete(DeleteBehavior.Restrict);

        // FLOOR
        modelBuilder.Entity<FloorModel>()
            .HasOne(f => f.CityModel)
            .WithMany(c => c.FloorModels)
            .HasForeignKey(f => f.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        // USER optional foreign keys
        modelBuilder.Entity<User>()
            .HasOne(u => u.ModeOfWorkModel)
            .WithMany()
            .HasForeignKey(u => u.ModeOfWorkId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasOne(u => u.DesignationModel)
            .WithMany()
            .HasForeignKey(u => u.DesignationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasOne(u => u.CityModel)
            .WithMany()
            .HasForeignKey(u => u.CityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
