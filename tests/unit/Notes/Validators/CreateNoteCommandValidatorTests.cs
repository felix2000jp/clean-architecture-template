using FluentAssertions;
using service.Notes.Commands;
using Xunit;

namespace unit.Notes.Validators;

public class CreateNoteCommandValidatorTests
{
    [Fact]
    public void CreateNoteCommandValidator_ItemsAreValid_Ok()
    {
        // Arrange
        var validator = new CreateNoteCommandValidator();
        var note = new CreateNoteCommand("title", "description");

        // Act
        var actual = validator.Validate(note);

        // Assert
        actual.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CreateNoteCommandValidator_ItemsAreEmpty_Error()
    {
        // Arrange
        var validator = new CreateNoteCommandValidator();
        var note = new CreateNoteCommand("", "");

        // Act
        var actual = validator.Validate(note);

        // Assert
        actual.IsValid.Should().BeFalse();
        actual.Errors.Count.Should().Be(2);
        actual.Errors[0].PropertyName.Should().Be("Title");
        actual.Errors[1].PropertyName.Should().Be("Description");
    }

    [Fact]
    public void CreateNoteCommandValidator_ItemsAreTooLong_Error()
    {
        // Arrange
        var validator = new CreateNoteCommandValidator();
        var title = new string('a', 151);
        var description = new string('a', 501);
        var note = new CreateNoteCommand(title, description);

        // Act
        var actual = validator.Validate(note);

        // Assert
        actual.IsValid.Should().BeFalse();
        actual.Errors.Count.Should().Be(2);
        actual.Errors[0].PropertyName.Should().Be("Title");
        actual.Errors[1].PropertyName.Should().Be("Description");
    }
}