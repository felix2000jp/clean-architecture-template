using FluentAssertions;
using service.Notes.Commands;
using Xunit;

namespace unit.Notes.Validators;

public class DeleteNoteCommandValidatorTests
{
    [Fact]
    public void DeleteNoteCommandValidator_ItemsAreValid_Ok()
    {
        // Arrange
        var validator = new DeleteNoteCommandValidator();
        var note = new DeleteNoteCommand(Guid.NewGuid());

        // Act
        var actual = validator.Validate(note);

        // Assert
        actual.IsValid.Should().BeTrue();
    }

    [Fact]
    public void DeleteNoteCommandValidator_ItemsAreEmpty_Error()
    {
        // Arrange
        var validator = new DeleteNoteCommandValidator();
        var note = new DeleteNoteCommand(Guid.Empty);

        // Act
        var actual = validator.Validate(note);

        // Assert
        actual.IsValid.Should().BeFalse();
        actual.Errors[0].PropertyName.Should().Be("Id");
    }
}