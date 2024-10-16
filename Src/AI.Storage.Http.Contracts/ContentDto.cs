namespace AI.Storage.Http.Contracts;

/// <summary>
/// Content.
/// </summary>
public class ContentDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the Content.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the Content.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}