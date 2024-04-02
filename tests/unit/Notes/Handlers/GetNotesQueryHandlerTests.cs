using AutoFixture;
using AutoMapper;
using core.Notes;
using FluentAssertions;
using NSubstitute;
using service.Notes;
using service.Notes.Queries;
using Xunit;

namespace unit.Notes.Handlers;

public class GetNotesQueryHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly INoteService _noteService = Substitute.For<INoteService>();
    private readonly GetNotesQueryHandler _handler;

    public GetNotesQueryHandlerTests()
    {
        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new NoteProfile())));
        _handler = new GetNotesQueryHandler(mapper, _noteService);
    }

    [Fact]
    public async Task Handle_NotesAreNotEmpty_Note()
    {
        // Arrange
        const int page = 0;
        var notes = _fixture.CreateMany<Note>(3).ToList();
        var request = new GetNotesQuery(page);

        _noteService
            .GetNotes(Arg.Any<int>())
            .Returns(notes);

        // Act
        var actual = await _handler.Handle(request);

        // Assert
        actual.IsError.Should().BeFalse();
        actual.Value.Should().BeEquivalentTo(notes);
    }
}