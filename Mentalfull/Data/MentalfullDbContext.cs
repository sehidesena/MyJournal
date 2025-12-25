using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Mentalfull.Entities.JournalEntries;
using Mentalfull.Entities.Chats;
using Mentalfull.Entities.Analysis;
using Mentalfull.Entities.MoodTracking;
using Mentalfull.Entities.Recommendations;
using Volo.Abp.Identity;

namespace Mentalfull.Data;

public class MentalfullDbContext : AbpDbContext<MentalfullDbContext>
{
    
    public const string DbTablePrefix = "App";
    public const string DbSchema = null;

    public DbSet<JournalEntry> JournalEntries { get; set; }
    public DbSet<ChatSession> ChatSessions { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<MoodLog> MoodLogs { get; set; }
    public DbSet<EmotionalAnalysisResult> EmotionalAnalysisResults { get; set; }
    public DbSet<Recommendation> Recommendations { get; set; }

    public MentalfullDbContext(DbContextOptions<MentalfullDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigurePermissionManagement();
        builder.ConfigureBlobStoring();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        
        /* Configure your own entities here */
        
        // JournalEntry configuration
        builder.Entity<JournalEntry>(b =>
        {
            b.ToTable(DbTablePrefix + "JournalEntries", DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.Title).IsRequired().HasMaxLength(200);
            b.Property(x => x.Content).IsRequired();
            b.Property(x => x.UserId).IsRequired();
            b.Property(x => x.EntryDate).IsRequired();
            
            // Relationship with User
            b.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            b.HasIndex(x => x.UserId);
            b.HasIndex(x => x.EntryDate);
        });

        // ChatSession configuration
        builder.Entity<ChatSession>(b =>
        {
            b.ToTable(DbTablePrefix + "ChatSessions", DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.Title).IsRequired().HasMaxLength(200);
            b.Property(x => x.UserId).IsRequired();
            
            // Relationship with User
            b.HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            b.HasIndex(x => x.UserId);
        });

        // ChatMessage configuration
        builder.Entity<ChatMessage>(b =>
        {
            b.ToTable(DbTablePrefix + "ChatMessages", DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.ChatSessionId).IsRequired();
            b.Property(x => x.Content).IsRequired();
            
            // Relationship with ChatSession
            b.HasOne<ChatSession>()
                .WithMany()
                .HasForeignKey(x => x.ChatSessionId)
                .OnDelete(DeleteBehavior.Cascade);
            
            b.HasIndex(x => x.ChatSessionId);
        });

        // MoodLog configuration
        builder.Entity<MoodLog>(b =>
        {
            b.ToTable(DbTablePrefix + "MoodLogs", DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.UserId).IsRequired();
            b.Property(x => x.Intensity).IsRequired();
            b.Property(x => x.PrimaryEmotion).IsRequired().HasMaxLength(50);
            
            // Relationship with User
            b.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            b.HasIndex(x => x.UserId);
            b.HasIndex(x => x.Timestamp);
        });

        // EmotionalAnalysisResult configuration
        builder.Entity<EmotionalAnalysisResult>(b =>
        {
            b.ToTable(DbTablePrefix + "EmotionalAnalysisResults", DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.JournalEntryId).IsRequired();
            
            // 1:1 with JournalEntry
            b.HasOne(x => x.JournalEntry)
                .WithOne(x => x.AnalysisResult)
                .HasForeignKey<EmotionalAnalysisResult>(x => x.JournalEntryId)
                .OnDelete(DeleteBehavior.Cascade);
            
            b.HasIndex(x => x.JournalEntryId).IsUnique();
        });

        // Recommendation configuration
        builder.Entity<Recommendation>(b =>
        {
            b.ToTable(DbTablePrefix + "Recommendations", DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.UserId).IsRequired();
            b.Property(x => x.Title).IsRequired().HasMaxLength(200);
            b.Property(x => x.Reasoning).IsRequired();
            
            // Relationship with User
            b.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            b.HasIndex(x => x.UserId);
        });
    }
}