using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI.Storage.Entities;

/// <summary>
/// Represents an Content entity in the database.
/// This class defines the structure and properties of the Content table.
/// </summary>
[Table("aggregates")]
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
    /// Gets or sets the name of the Content.
    /// This field is required and stored as text in the database.
    /// </summary>
    [Required]
    [Column("name", TypeName = "text")]
    public string Name { get; set; } = string.Empty;
}