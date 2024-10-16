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
    /// Creates new Content entities based on the files uploaded in the HTTP request.
    /// </summary>
    /// <param name="content">The HttpContent containing the files to be uploaded. This should typically be a MultipartFormDataContent.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>A CreateContentResponse containing the IDs of the newly created Content entities.</returns>
    [Post("api/v1/Content")]
    Task<CreateContentResponse> CreateContent([Body] HttpContent content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves and downloads a Content entity by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the Content to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>An HttpResponseMessage containing the file content and metadata.</returns>
    [Get("api/v1/Content/{id}")]
    Task<HttpResponseMessage> GetContent([Path] long id, CancellationToken cancellationToken = default);
}