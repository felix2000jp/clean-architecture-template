using AutoFixture;
using core.Notes;
using core.Results;
using FluentAssertions;
using NSubstitute;
using service.Notes.Commands;
using Xunit;

namespace unit.Notes.Handlers;

public class CreateNoteCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly INoteService _noteService = Substitute.For<INoteService>();
    private readonly CreateNoteCommandHandler _handler;

    public CreateNoteCommandHandlerTests()
    {
        _handler = new CreateNoteCommandHandler(_noteService);
    }

    [Fact]
    public async Task Handle_TitleIsUnique_Ok()
    {
        // Arrange
        var note = _fixture.Create<Note>();
        var value = ResultTypes.Ok();
        var request = new CreateNoteCommand(note.Title, note.Description);

        _noteService
            .CreateNote(Arg.Any<string>(), Arg.Any<string>())
            .Returns(value);

        // Act
        var actual = await _handler.Handle(request);

        // Assert
        actual.IsError.Should().BeFalse();
        actual.Value.Should().Be(value);
    }

    [Fact]
    public async Task Handle_IdDoesNotExist_Conflict()
    {
        // Arrange
        var note = _fixture.Create<Note>();
        var error = ResultTypes.Conflict($"Note with Title [{note.Title}] already exists");
        var request = new CreateNoteCommand(note.Title, note.Description);

        _noteService
            .CreateNote(Arg.Any<string>(), Arg.Any<string>())
            .Returns(error);

        // Act
        var actual = await _handler.Handle(request);

        // Assert
        actual.IsError.Should().BeTrue();
        actual.Error.Should().Be(error);
    }
}