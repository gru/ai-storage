using AI.Storage.Content;
using AI.Storage.Entities;
using AI.Storage.Host.Internal;
using AI.Storage.Http.Contracts;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.AspNetCore.Http.Extensions;


namespace AI.Storage.Host.Controllers;

/// <summary>
/// Controller responsible for handling Content-related HTTP requests.
/// This controller is versioned and mapped to the route "api/v{version:apiVersion}/[controller]".
/// </summary>
[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ContentController : ControllerBase
{
    private readonly ContentHandler _contentHandler;

    /// <summary>
    /// Initializes a new instance of the ContentController class.
    /// </summary>
    /// <param name="contentHandler">The handler for content-related operations.</param>
    public ContentController(ContentHandler contentHandler)
    {
        _contentHandler = contentHandler;
    }

    /// <summary>
    /// Creates new Content entities based on uploaded files.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>An ActionResult containing the CreateContentResponse with IDs of the newly created Content entities.</returns>
    [HttpPost]
    [DisableFormValueModelBinding]
    public async Task<ActionResult<CreateContentResponse>> CreateContent(CancellationToken cancellationToken)
    {
        var response = await _contentHandler.CreateContent(HttpContext, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Retrieves and downloads a Content entity by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the Content to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>A FileStreamResult containing the downloaded content.</returns>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetContent(long id, CancellationToken cancellationToken)
    {
        var fileStreamResult = await _contentHandler.GetContent(id, cancellationToken);
        return fileStreamResult;
    }
}