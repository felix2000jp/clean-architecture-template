using FluentAssertions;
using service.Notes.Queries;
using Xunit;

namespace unit.Notes.Validators;

public class GetNotesQueryValidatorTests
{
    [Fact]
    public void GetNotesQueryValidator_ItemsAreValid_Ok()
    {
        // Arrange
        var validator = new GetNotesQueryValidator();
        var note = new GetNotesQuery(0);

        // Act
        var actual = validator.Validate(note);

        // Assert
        actual.IsValid.Should().BeTrue();
    }

    [Fact]
    public void GetNotesQueryValidator_ItemsAreNegative_Error()
    {
        // Arrange
        var validator = new GetNotesQueryValidator();
        var note = new GetNotesQuery(-1);

        // Act
        var actual = validator.Validate(note);

        // Assert
        actual.IsValid.Should().BeFalse();
        actual.Errors[0].PropertyName.Should().Be("Page");
    }
}