using AutoFixture;
using AutoMapper;
using core.Notes;
using core.Results;
using FluentAssertions;
using NSubstitute;
using service.Notes;
using service.Notes.Queries;
using Xunit;

namespace unit.Notes.Handlers;

public class GetNoteByIdQueryHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly INoteService _noteService = Substitute.For<INoteService>();
    private readonly GetNoteByIdQueryHandler _handler;

    public GetNoteByIdQueryHandlerTests()
    {
        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new NoteProfile())));
        _handler = new GetNoteByIdQueryHandler(mapper, _noteService);
    }

    [Fact]
    public async Task Handle_IdExists_Note()
    {
        // Arrange
        var note = _fixture.Create<Note>();
        var request = new GetNoteByIdQuery(note.Id);

        _noteService
            .GetNoteById(Arg.Any<Guid>())
            .Returns(note);

        // Act
        var actual = await _handler.Handle(request);

        // Assert
        actual.IsError.Should().BeFalse();
        actual.Value.Should().BeEquivalentTo(note);
    }

    [Fact]
    public async Task Handle_IdDoesNotExist_NotFound()
    {
        // Arrange
        var note = _fixture.Create<Note>();
        var error = ResultTypes.NotFound($"Note with Id [{note.Id}] does not exist");
        var request = new GetNoteByIdQuery(Guid.NewGuid());

        _noteService
            .GetNoteById(Arg.Any<Guid>())
            .Returns(error);

        // Act
        var actual = await _handler.Handle(request);

        // Assert
        actual.IsError.Should().BeTrue();
        actual.Error.Should().Be(error);
    }
}