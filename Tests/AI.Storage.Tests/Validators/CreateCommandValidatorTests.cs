using AI.Storage.Http.Contracts;
using AI.Storage.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace AI.Storage.Tests.Validators
{
    public class CreateCommandValidatorTests
    {
        private readonly CreateCommandValidator _validator = new();

        [Fact]
        public void Should_have_error_when_Name_is_null()
        {
            var command = new CreateContentCommand { Name = null! };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("Name is required.");
        }

        [Fact]
        public void Should_have_error_when_Name_is_empty()
        {
            var command = new CreateContentCommand { Name = string.Empty };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("Name is required.");
        }

        [Fact]
        public void Should_have_error_when_Name_is_too_short()
        {
            var command = new CreateContentCommand { Name = "A" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("Name must be between 5 and 150 characters.");
        }

        [Fact]
        public void Should_have_error_when_Name_is_too_long()
        {
            var command = new CreateContentCommand { Name = new string('A', 151) };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("Name must be between 5 and 150 characters.");
        }

        [Fact]
        public void Should_not_have_error_when_Name_is_valid()
        {
            var command = new CreateContentCommand { Name = "Valid Name" };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}