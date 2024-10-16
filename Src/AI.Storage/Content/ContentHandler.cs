using AI.Storage.Entities;
using AI.Storage.Http.Contracts;
using FluentValidation;

namespace AI.Storage.Content;

/// <summary>
/// Handles business logic operations related to Contents.
/// This class is responsible for creating and retrieving Content entities.
/// </summary>
public class ContentHandler
{
    private readonly ProjectDbContext _dbContext;
    private readonly IValidator<CreateContentCommand> _validator;

    /// <summary>
    /// Initializes a new instance of the ContentHandler class.
    /// </summary>
    /// <param name="dbContext">The database context for accessing Content entities.</param>
    /// <param name="validator">The validator for CreateContentCommand.</param>
    public ContentHandler(ProjectDbContext dbContext, IValidator<CreateContentCommand> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }

    /// <summary>
    /// Creates a new Content entity based on the provided command.
    /// </summary>
    /// <param name="command">The command containing details for creating the Content.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>The ID of the newly created Content.</returns>
    /// <exception cref="ValidationException">Thrown when the command fails validation.</exception>
    public async Task<long> CreateContent(CreateContentCommand command, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var aggregate = new ContentEntity
        {
            Name = command.Name
        };

        _dbContext.Contents.Add(aggregate);
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        return aggregate.Id;
    }

    /// <summary>
    /// Retrieves an Content entity by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the Content to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>The retrieved ContentEntity.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an Content with the specified ID is not found.</exception>
    public async Task<ContentEntity> GetContent(long id, CancellationToken cancellationToken)
    {
        var aggregate = await _dbContext.Contents.FindAsync([id], cancellationToken);
            
        if (aggregate == null)
        {
            throw new InvalidOperationException($"Content with id {id} not found");
        }

        return aggregate;
    }
}