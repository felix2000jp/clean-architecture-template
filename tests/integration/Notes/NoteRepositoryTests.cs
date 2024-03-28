using AutoFixture;
using core;
using core.Notes;
using FluentAssertions;
using infra;
using infra.Notes;
using Xunit;

namespace integration.Notes;

public class NoteRepositoryTests : IntegrationTests
{
    private readonly Fixture _fixture = new();
    private readonly IUnitOfWork _unitOfWork;
    private readonly INoteRepository _noteRepository;

    public NoteRepositoryTests(ApiFactory apiFactory) : base(apiFactory)
    {
        _unitOfWork = new UnitOfWork(DataContext);
        _noteRepository = new NoteRepository(DataContext);
    }

    [Fact]
    public async Task GetNotes_PageIsFull_Notes()
    {
        // Arrange
        const int page = 0;
        var notes = _fixture.CreateMany<Note>(20).ToList();

        DataContext.Notes.AddRange(notes);
        await DataContext.SaveChangesAsync();

        // Act
        var actual = await _noteRepository.GetNotes(page);

        // Assert
        actual.Should().BeEquivalentTo(notes);
    }

    [Fact]
    public async Task GetNotes_PageIsEmpty_Notes()
    {
        // Arrange
        const int page = 1;
        var notes = _fixture.CreateMany<Note>(20);

        DataContext.Notes.AddRange(notes);
        await DataContext.SaveChangesAsync();

        // Act
        var actual = await _noteRepository.GetNotes(page);

        // Assert
        var expected = Enumerable.Empty<Note>();
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetNoteById_IdExists_Note()
    {
        // Arrange
        var note = _fixture.Create<Note>();

        DataContext.Notes.Add(note);
        await DataContext.SaveChangesAsync();

        // Act
        var actual = await _noteRepository.GetNoteById(note.Id);

        // Assert
        actual.Should().BeEquivalentTo(note);
    }

    [Fact]
    public async Task GetNoteById_IdDoesNotExist_Null()
    {
        // Arrange
        var note = _fixture.Create<Note>();

        // Act
        var actual = await _noteRepository.GetNoteById(note.Id);

        // Assert
        actual.Should().BeNull();
    }

    [Fact]
    public async Task GetNoteByTitle_TitleExists_Note()
    {
        // Arrange
        var note = _fixture.Create<Note>();

        DataContext.Notes.Add(note);
        await DataContext.SaveChangesAsync();

        // Act
        var actual = await _noteRepository.GetNoteByTitle(note.Title);

        // Assert
        actual.Should().BeEquivalentTo(note);
    }

    [Fact]
    public async Task GetNoteByTitle_TitleDoesNotExist_Null()
    {
        // Arrange
        var note = _fixture.Create<Note>();

        // Act
        var actual = await _noteRepository.GetNoteByTitle(note.Title);

        // Assert
        actual.Should().BeNull();
    }

    [Fact]
    public async Task CreateNote_Void()
    {
        // Arrange
        var note = _fixture.Create<Note>();

        // Act
        _noteRepository.CreateNote(note);
        await _unitOfWork.CommitChanges();
        var actual = DataContext.Notes.SingleOrDefault(x => x.Id == note.Id);

        // Assert
        actual.Should().BeEquivalentTo(note);
    }

    [Fact]
    public async Task DeleteNote_Void()
    {
        // Arrange
        var note = _fixture.Create<Note>();

        DataContext.Notes.Add(note);
        await DataContext.SaveChangesAsync();

        // Act
        _noteRepository.DeleteNote(note);
        await _unitOfWork.CommitChanges();
        var actual = DataContext.Notes.SingleOrDefault(x => x.Id == note.Id);

        // Assert
        actual.Should().BeNull();
    }
}