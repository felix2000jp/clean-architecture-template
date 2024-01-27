using AutoFixture;
using core.Notes;
using core.Results;
using FluentAssertions;
using NSubstitute;
using service.Notes.Commands;
using Xunit;

namespace unit.Notes.Handlers;

public class UpdateNoteCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly INoteService _noteService = Substitute.For<INoteService>();
    private readonly UpdateNoteCommandHandler _handler;

    public UpdateNoteCommandHandlerTests()
    {
        _handler = new UpdateNoteCommandHandler(_noteService);
    }

    [Fact]
    public async Task Handle_IdExistsAndTitleIsUnique_Ok()
    {
        // Arrange
        var note = _fixture.Create<Note>();
        var value = ResultTypes.Ok();
        var request = new UpdateNoteCommand(note.Id, note.Title, note.Description);

        _noteService
            .UpdateNote(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(value);

        // Act
        var actual = await _handler.Handle(request);

        // Assert
        actual.IsError.Should().BeFalse();
        actual.Value.Should().Be(value);
    }

    [Fact]
    public async Task Handle_IdExistsAndTitleIsNotUpdated_Ok()
    {
        // Arrange
        var ok = ResultTypes.Ok();
        var note = _fixture.Create<Note>();
        var request = new UpdateNoteCommand(note.Id, note.Title, note.Description);

        _noteService
            .UpdateNote(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(ok);

        // Act
        var actual = await _handler.Handle(request);

        // Assert
        actual.IsError.Should().BeFalse();
        actual.Value.Should().Be(ok);
    }

    [Fact]
    public async Task Handle_IdDoesNotExist_NotFound()
    {
        // Arrange
        var note = _fixture.Create<Note>();
        var error = ResultTypes.NotFound($"Note with Id [{note.Id}] does not exist");
        var request = new UpdateNoteCommand(note.Id, note.Title, note.Description);

        _noteService
            .UpdateNote(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(error);

        // Act
        var actual = await _handler.Handle(request);

        // Assert
        actual.IsError.Should().BeTrue();
        actual.Error.Should().Be(error);
    }

    [Fact]
    public async Task Handle_TitleIsNotUnique_Conflict()
    {
        // Arrange
        var note = _fixture.Create<Note>();
        var error = ResultTypes.Conflict($"Note with Title [{note.Title}] already exists");
        var request = new UpdateNoteCommand(note.Id, note.Title, note.Description);

        _noteService
            .UpdateNote(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(error);

        // Act
        var actual = await _handler.Handle(request);

        // Assert
        actual.IsError.Should().BeTrue();
        actual.Error.Should().Be(error);
    }
}