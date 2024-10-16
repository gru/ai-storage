using AI.Storage.Content;
using AI.Storage.Http.Contracts;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

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
    private readonly ContentHandler _aggregateHandler;

    /// <summary>
    /// Initializes a new instance of the ContentController class.
    /// </summary>
    /// <param name="aggregateHandler">The handler for aggregate-related operations.</param>
    public ContentController(ContentHandler aggregateHandler)
    {
        _aggregateHandler = aggregateHandler;
    }

    /// <summary>
    /// Creates a new Content.
    /// </summary>
    /// <param name="command">The command containing the details for creating the Content.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>An ActionResult containing the ID of the newly created Content.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateContent([FromBody] CreateContentCommand command, CancellationToken cancellationToken)
    {
        var aggregateId = await _aggregateHandler.CreateContent(command, cancellationToken);
        return CreatedAtAction(nameof(GetContent), new { id = aggregateId }, aggregateId);
    }

    /// <summary>
    /// Retrieves an Content by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the Content to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>An ActionResult containing the ContentDto of the requested Content.</returns>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<ContentDto>> GetContent(long id, CancellationToken cancellationToken)
    {
        var aggregate = await _aggregateHandler.GetContent(id, cancellationToken);
        return Ok(aggregate);
    }
}