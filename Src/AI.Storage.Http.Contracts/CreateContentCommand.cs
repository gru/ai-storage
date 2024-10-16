namespace AI.Storage.Http.Contracts;

/// <summary>
/// Command to create a new aggregate.
/// </summary>
public class CreateContentCommand
{
    /// <summary>
    /// The name of the aggregate to be created.
    /// </summary>
    public required string Name { get; set; }
}