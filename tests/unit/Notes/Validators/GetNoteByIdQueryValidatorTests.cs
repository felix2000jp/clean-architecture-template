using FluentAssertions;
using service.Notes.Queries;
using Xunit;

namespace unit.Notes.Validators;

public class GetNoteByIdQueryValidatorTests
{
    [Fact]
    public void GetNoteByIdQueryValidator_ItemsAreValid_Ok()
    {
        // Arrange
        var validator = new GetNoteByIdQueryValidator();
        var note = new GetNoteByIdQuery(Guid.NewGuid());

        // Act
        var actual = validator.Validate(note);

        // Assert
        actual.IsValid.Should().BeTrue();
    }

    [Fact]
    public void GetNoteByIdQueryValidator_ItemsAreEmpty_Error()
    {
        // Arrange
        var validator = new GetNoteByIdQueryValidator();
        var note = new GetNoteByIdQuery(Guid.Empty);

        // Act
        var actual = validator.Validate(note);

        // Assert
        actual.IsValid.Should().BeFalse();
        actual.Errors[0].PropertyName.Should().Be("Id");
    }
}