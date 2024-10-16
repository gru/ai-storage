namespace AI.Storage.Http.Contracts;

/// <summary>
/// Represents the response for the CreateContent operation.
/// </summary>
public class CreateContentResponse
{
    /// <summary>
    /// Gets or sets the list of identifiers for the created ContentEntity objects.
    /// </summary>
    public IReadOnlyCollection<long> ContentIds { get; set; } = Array.Empty<long>();
}