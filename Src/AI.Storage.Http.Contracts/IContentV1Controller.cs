using RestEase;

namespace AI.Storage.Http.Contracts;

/// <summary>
/// Represents the contract for the Content API (version 1).
/// This interface defines the operations available for interacting with Content resources.
/// It is designed to be used with RestEase for generating API clients.
/// </summary>
[Header("Accept", "application/json")]
public interface IContentV1Controller
{
    /// <summary>
    /// Creates a new Content.
    /// </summary>
    /// <param name="command">The command containing the details for creating the Content.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>The ID of the newly created Content.</returns>
    [Post("api/v1/Content")]
    Task<long> CreateContent([Body] CreateContentCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an Content by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the Content to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>The Content DTO containing the details of the requested Content.</returns>
    [Get("api/v1/Content/{id}")]
    Task<ContentDto> GetContent([Path] long id, CancellationToken cancellationToken = default);
}