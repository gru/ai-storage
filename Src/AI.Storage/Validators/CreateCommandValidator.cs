using AI.Storage.Http.Contracts;
using FluentValidation;

namespace AI.Storage.Validators;

/// <summary>
/// Validator for the CreateContentCommand.
/// </summary>
public class CreateCommandValidator : AbstractValidator<CreateContentCommand>
{
    /// <summary>
    /// Initializes a new instance of the CreateCommandValidator class.
    /// Sets up validation rules for the CreateContentCommand.
    /// </summary>
    public CreateCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(5, 150).WithMessage("Name must be between 5 and 150 characters.");
    }
}