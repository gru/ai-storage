using Microsoft.EntityFrameworkCore;

namespace AI.Storage.Entities;

/// <summary>
/// Represents the database context for the project.
/// This class is responsible for configuring the database connection and mapping entity classes to database tables.
/// </summary>
public class StorageDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the StorageDbContext class.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    public StorageDbContext(DbContextOptions<StorageDbContext> options)
        : base(options)
    {
    }
    
    /// <summary>
    /// Gets or sets the DbSet for Content entities.
    /// This property provides access to query and save instances of ContentEntity.
    /// </summary>
    public DbSet<ContentEntity> Contents { get; set; } = null!;

    /// <summary>
    /// Configures the model that was discovered by convention from the entity types
    /// exposed in DbSet properties on your derived context.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ContentEntity>(entity =>
        {
            // Configure the table name
            entity.ToTable("contents");
            
            // Configure the primary key
            entity.HasKey(e => e.Id);
            
            // Configure the Id property
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("bigint")
                .UseIdentityAlwaysColumn();
            
            // Configure the FileName property
            entity.Property(e => e.FileName)
                .HasColumnName("file_name")
                .HasColumnType("varchar(150)")
                .IsRequired();
            
            // Configure the ContentType property
            entity.Property(e => e.ContentType)
                .HasColumnName("content_type")
                .HasColumnType("varchar(50)")
                .IsRequired();
            
            // Configure the BlobKey property
            entity.Property(e => e.BlobKey)
                .HasColumnName("blob_key")
                .HasColumnType("varchar(100)")
                .IsRequired();
        });
    }
}