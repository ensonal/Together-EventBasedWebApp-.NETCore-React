using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Together.DataAccess.Entities;

namespace Together.DataAccess;

public class TogetherDbContext : IdentityDbContext<IdentityUser>
{
    public TogetherDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<UserInfo> UserInfo { get; set; }
    public DbSet<UserAccountLevel> UserAccountLevels { get; set; }
    public DbSet<UserEquipment> UserEquipments { get; set; }
    public DbSet<Sport> Sports { get; set; }
    public DbSet<UserSport> UserSports { get; set; }
    public DbSet<SportExperience> SportExperience { get; set; }
    public DbSet<UserEvent> UserEvents { get; set; }
    public DbSet<EventStatus> EventStatuses { get; set; }
    public DbSet<UserEventRequest> UserEventRequests { get; set; }
    public DbSet<EventRequestStatus> EventRequestStatuses { get; set; }
    public DbSet<UserFavoriteEvent> UserFavoriteEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserInfo>(entity =>
        {
            entity.HasKey(c => c.UserID);
            entity.ToTable(name: "UserInfo");
        });
        
        modelBuilder.Entity<UserAccountLevel>(entity =>
        {
            entity.HasKey(c => c.UserId);
            entity.ToTable(name: "UserAccountLevel");
        });
        
        modelBuilder.Entity<UserEvent>()
            .HasOne(ue => ue.UserInfo)
            .WithMany(ui => ui.UserEvents)
            .HasForeignKey(ue => ue.UserId)
            .HasPrincipalKey(ui => ui.UserID);

        modelBuilder.Entity<UserFavoriteEvent>()
            .HasOne(ufe => ufe.UserInfo)
            .WithMany(ui => ui.UserFavoriteEvents)
            .HasForeignKey(ui => ui.UserId);
        
        modelBuilder.Entity<UserFavoriteEvent>()
            .HasOne(ufe => ufe.UserEvent)
            .WithMany(ue => ue.UserFavoriteEvents)
            .HasForeignKey(ufe => ufe.EventId);

    }
}