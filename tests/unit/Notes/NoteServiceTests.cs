using AutoFixture;
using core;
using core.Notes;
using core.Results;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Serilog;
using service.Notes;
using Xunit;

namespace unit.Notes;

public class NoteServiceTests
{
    private readonly Fixture _fixture = new();
    private readonly IUnitOfWork _unitOfWorkMock = Substitute.For<IUnitOfWork>();
    private readonly INoteRepository _noteRepositoryMock = Substitute.For<INoteRepository>();
    private readonly INoteService _noteService;

    public NoteServiceTests()
    {
        _noteService = new NoteService(Substitute.For<ILogger>(), _unitOfWorkMock, _noteRepositoryMock);
    }

    [Fact]
    public async Task GetNotes_NotesExist_Notes()
    {
        // Arrange
        const int page = 0;
        var notes = _fixture.CreateMany<Note>(3).ToList();

        _noteRepositoryMock.GetNotes(Arg.Any<int>()).Returns(notes);

        // Act
        var actual = await _noteService.GetNotes(page);

        // Assert
        actual.IsError.Should().BeFalse();
        actual.Value.Should().BeEquivalentTo(notes);
    }

    [Fact]
    public async Task GetNoteById_IdExists_Note()
    {
        // Arrange
        var note = _fixture.Create<Note>();

        _noteRepositoryMock.GetNoteById(Arg.Any<Guid>()).Returns(note);

        // Act
        var actual = await _noteService.GetNoteById(note.Id);

        // Assert
        actual.IsError.Should().BeFalse();
        actual.Value.Should().BeEquivalentTo(note);
    }

    [Fact]
    public async Task GetNoteById_NoteIsNull_NotFound()
    {
        // Arrange
        var note = _fixture.Create<Note>();
        var error = ResultTypes.NotFound($"Note with id {note.Id} does not exist");

        _noteRepositoryMock.GetNoteById(Arg.Any<Guid>()).ReturnsNull();

        // Act
        var actual = await _noteService.GetNoteById(note.Id);

        // Assert
        actual.IsError.Should().BeTrue();
        actual.Error.Should().Be(error);
    }

    [Fact]
    public async Task CreateNote_TitleIsUnique_Ok()
    {
        // Arrange
        var note = _fixture.Create<Note>();
        var value = ResultTypes.Ok();

        _noteRepositoryMock.GetNoteByTitle(Arg.Any<string>()).ReturnsNull();

        // Act
        var actual = await _noteService.CreateNote(note.Title, note.Description);

        // Assert
        actual.IsError.Should().BeFalse();
        actual.Value.Should().Be(value);

        await _unitOfWorkMock.Received(1).CommitChanges();
    }

    [Fact]
    public async Task CreateNote_TitleIsNotUnique_Conflict()
    {
        // Arrange
        var note = _fixture.Create<Note>();
        var error = ResultTypes.Conflict($"Note with title {note.Title} already exists");

        _noteRepositoryMock.GetNoteByTitle(Arg.Any<string>()).Returns(note);

        // Act
        var actual = await _noteService.CreateNote(note.Title, note.Description);

        // Assert
        actual.IsError.Should().BeTrue();
        actual.Error.Should().Be(error);
    }

    [Fact]
    public async Task DeleteNote_IdExists_Ok()
    {
        // Arrange
        var note = _fixture.Create<Note>();
        var value = ResultTypes.Ok();

        _noteRepositoryMock.GetNoteById(Arg.Any<Guid>()).Returns(note);

        // Act
        var actual = await _noteService.DeleteNote(note.Id);

        // Assert
        actual.IsError.Should().BeFalse();
        actual.Value.Should().Be(value);

        await _unitOfWorkMock.Received(1).CommitChanges();
    }

    [Fact]
    public async Task DeleteNote_IdDoesNotExist_NotFound()
    {
        // Arrange
        var note = _fixture.Create<Note>();
        var error = ResultTypes.NotFound($"Note with id {note.Id} does not exist");

        _noteRepositoryMock.GetNoteById(Arg.Any<Guid>()).ReturnsNull();

        // Act
        var actual = await _noteService.DeleteNote(note.Id);

        // Assert
        actual.IsError.Should().BeTrue();
        actual.Error.Should().Be(error);
    }

    [Fact]
    public async Task UpdateNote_IdExistsAndTitleIsUnique_Ok()
    {
        // Arrange
        var note = _fixture.Create<Note>();
        var value = ResultTypes.Ok();

        _noteRepositoryMock.GetNoteById(Arg.Any<Guid>()).Returns(note);
        _noteRepositoryMock.GetNoteByTitle(Arg.Any<string>()).ReturnsNull();

        // Act
        var actual = await _noteService.UpdateNote(note.Id, note.Title, note.Description);

        // Assert
        actual.IsError.Should().BeFalse();
        actual.Value.Should().Be(value);

        await _unitOfWorkMock.Received(1).CommitChanges();
    }

    [Fact]
    public async Task UpdateNote_IdExistsAndTitleIsNotUpdated_Ok()
    {
        // Arrange
        var note = _fixture.Create<Note>();
        var value = ResultTypes.Ok();

        _noteRepositoryMock.GetNoteById(Arg.Any<Guid>()).Returns(note);
        _noteRepositoryMock.GetNoteByTitle(Arg.Any<string>()).Returns(note);

        // Act
        var actual = await _noteService.UpdateNote(note.Id, note.Title, note.Description);

        // Assert
        actual.IsError.Should().BeFalse();
        actual.Value.Should().Be(value);

        await _unitOfWorkMock.Received(1).CommitChanges();
    }

    [Fact]
    public async Task UpdateNote_IdDoesNotExist_NotFound()
    {
        // Arrange
        var note = _fixture.Create<Note>();
        var error = ResultTypes.NotFound($"Note with id {note.Id} does not exist");

        _noteRepositoryMock.GetNoteById(Arg.Any<Guid>()).ReturnsNull();
        _noteRepositoryMock.GetNoteByTitle(Arg.Any<string>()).ReturnsNull();

        // Act
        var actual = await _noteService.UpdateNote(note.Id, note.Title, note.Description);

        // Assert
        actual.IsError.Should().BeTrue();
        actual.Error.Should().Be(error);
    }

    [Fact]
    public async Task UpdateNote_TitleIsNotUnique_Conflict()
    {
        // Arrange
        var note = _fixture.Create<Note>();
        var noteWithTitle = _fixture.Build<Note>().With(x => x.Title, note.Title).Create();
        var error = ResultTypes.Conflict($"Note with title {noteWithTitle.Title
        } already exists");

        _noteRepositoryMock.GetNoteById(Arg.Any<Guid>()).Returns(note);
        _noteRepositoryMock.GetNoteByTitle(Arg.Any<string>()).Returns(noteWithTitle);

        // Act
        var actual = await _noteService.UpdateNote(note.Id, note.Title, note.Description);

        // Assert
        actual.IsError.Should().BeTrue();
        actual.Error.Should().Be(error);
    }
}