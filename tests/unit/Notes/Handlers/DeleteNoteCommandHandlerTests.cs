using AutoFixture;
using core.Notes;
using core.Results;
using FluentAssertions;
using NSubstitute;
using service.Notes.Commands;
using Xunit;

namespace unit.Notes.Handlers;

public class DeleteNoteCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly INoteService _noteService = Substitute.For<INoteService>();
    private readonly DeleteNoteCommandHandler _handler;

    public DeleteNoteCommandHandlerTests()
    {
        _handler = new DeleteNoteCommandHandler(_noteService);
    }

    [Fact]
    public async Task Handle_IdExists_Ok()
    {
        // Arrange
        var note = _fixture.Create<Note>();
        var value = ResultTypes.Ok();
        var request = new DeleteNoteCommand(note.Id);

        _noteService
            .DeleteNote(Arg.Any<Guid>())
            .Returns(value);

        // Act
        var actual = await _handler.Handle(request);

        // Assert
        actual.IsError.Should().BeFalse();
        actual.Value.Should().Be(value);
    }

    [Fact]
    public async Task Handle_IdDoesNotExist_NotFound()
    {
        // Arrange
        var note = _fixture.Create<Note>();
        var error = ResultTypes.NotFound($"Note with Id [{note.Id}] does not exist");
        var request = new DeleteNoteCommand(note.Id);

        _noteService
            .DeleteNote(Arg.Any<Guid>())
            .Returns(error);


        // Act
        var actual = await _handler.Handle(request);

        // Assert
        actual.IsError.Should().BeTrue();
        actual.Error.Should().Be(error);
    }
}