using FluentAssertions;
using service.Notes.Commands;
using Xunit;

namespace unit.Notes.Validators;

public class UpdateNoteCommandValidatorTests
{
    [Fact]
    public void UpdateNoteCommandValidator_ItemsAreValid_Ok()
    {
        // Arrange
        var validator = new UpdateNoteCommandValidator();
        var note = new UpdateNoteCommand(Guid.NewGuid(), "title", "description");

        // Act
        var actual = validator.Validate(note);

        // Assert
        actual.IsValid.Should().BeTrue();
    }

    [Fact]
    public void UpdateNoteCommandValidator_ItemsAreEmpty_Error()
    {
        // Arrange
        var validator = new UpdateNoteCommandValidator();
        var note = new UpdateNoteCommand(Guid.Empty, "", "");

        // Act
        var actual = validator.Validate(note);

        // Assert
        actual.IsValid.Should().BeFalse();
        actual.Errors.Count.Should().Be(3);
        actual.Errors[0].PropertyName.Should().Be("Id");
        actual.Errors[1].PropertyName.Should().Be("Title");
        actual.Errors[2].PropertyName.Should().Be("Description");
    }

    [Fact]
    public void UpdateNoteCommandValidator_ItemsAreTooLong_Error()
    {
        // Arrange
        var validator = new UpdateNoteCommandValidator();
        var title = new string('a', 151);
        var description = new string('a', 501);
        var note = new UpdateNoteCommand(Guid.NewGuid(), title, description);

        // Act
        var actual = validator.Validate(note);

        // Assert
        actual.IsValid.Should().BeFalse();
        actual.Errors.Count.Should().Be(2);
        actual.Errors[0].PropertyName.Should().Be("Title");
        actual.Errors[1].PropertyName.Should().Be("Description");
    }
}