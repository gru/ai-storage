using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI.Storage.Entities;

/// <summary>
/// Represents a Content entity in the database.
/// This class defines the structure and properties of the Content table,
/// representing a file stored in S3 storage.
/// </summary>
[Table("contents")]
public class ContentEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the Content.
    /// This is the primary key of the table.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id", TypeName = "bigint")]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the file name of the Content.
    /// This field is required and stored as varchar(150) in the database.
    /// </summary>
    [Required]
    [Column("file_name", TypeName = "varchar(150)")]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content type of the file.
    /// This field is required and stored as varchar(50) in the database.
    /// </summary>
    [Required]
    [Column("content_type", TypeName = "varchar(50)")]
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the blob key of the file in S3 storage.
    /// This field is required and stored as varchar(100) in the database.
    /// The format is "{BucketName}/{FileName}".
    /// </summary>
    [Required]
    [Column("blob_key", TypeName = "varchar(100)")]
    public string BlobKey { get; set; } = string.Empty;
}