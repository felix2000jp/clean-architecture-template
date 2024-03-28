using System.Net;
using System.Net.Http.Json;
using api.Notes.Dtos;
using AutoFixture;
using core.Notes;
using core.Results;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace integration.Notes;

public class NoteRoutesTests(ApiFactory apiFactory) : IntegrationTests(apiFactory)
{
    private readonly Fixture _fixture = new();

    [Fact]
    public async Task GetNotes_PageIsFull_Notes()
    {
        // Arrange
        const int page = 0;
        var notes = _fixture.CreateMany<Note>(20).ToList();

        await DataContext.Notes.AddRangeAsync(notes);
        await DataContext.SaveChangesAsync();

        // Act
        var actual = await HttpClient.GetAsync($"api/notes?page={page}");

        // Assert
        actual.StatusCode.Should().Be(HttpStatusCode.OK);

        var expected = DataContext.Notes.Skip(0).Take(20).AsNoTracking();
        var content = await actual.Content.ReadFromJsonAsync<IEnumerable<NoteDto>>();
        content.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetNotes_PageIsEmpty_Notes()
    {
        // Arrange
        const int page = 1;
        var notes = _fixture.CreateMany<Note>(20).ToList();

        await DataContext.Notes.AddRangeAsync(notes);
        await DataContext.SaveChangesAsync();

        // Act
        var actual = await HttpClient.GetAsync($"api/notes?page={page}");

        // Assert
        actual.StatusCode.Should().Be(HttpStatusCode.OK);

        var expected = Enumerable.Empty<Note>();
        var content = await actual.Content.ReadFromJsonAsync<IEnumerable<Note>>();
        content.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetNoteById_IdtExists_Note()
    {
        // Arrange
        var note = _fixture.Create<Note>();

        DataContext.Notes.Add(note);
        await DataContext.SaveChangesAsync();

        // Act
        var actual = await HttpClient.GetAsync($"api/notes/{note.Id}");

        // Assert
        actual.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await actual.Content.ReadFromJsonAsync<NoteDto>();
        content.Should().BeEquivalentTo(note);
    }

    [Fact]
    public async Task GetNoteById_IdDoesNotExist_NotFound()
    {
        // Arrange
        var note = _fixture.Create<Note>();
        var error = ResultTypes.NotFound($"Note with Id [{note.Id}] does not exist");

        // Act
        var actual = await HttpClient.GetAsync($"api/notes/{note.Id}");

        // Assert
        actual.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var content = await actual.Content.ReadFromJsonAsync<ResultError>();
        content.Should().Be(error);
    }

    [Fact]
    public async Task CreateNote_TitleIsUnique_Created()
    {
        // Arrange
        var createNoteDto = _fixture.Create<CreateNoteDto>();

        // Act
        var actual = await HttpClient.PostAsJsonAsync("api/notes", createNoteDto);

        // Assert
        actual.StatusCode.Should().Be(HttpStatusCode.Created);

        DataContext.Notes.AsNoTracking().SingleOrDefault(x => x.Title == createNoteDto.Title).Should().NotBeNull();
    }

    [Fact]
    public async Task CreateNote_TitleIsNotUnique_Created()
    {
        // Arrange
        var note = _fixture.Create<Note>();
        var createNoteDto = _fixture.Build<CreateNoteDto>().With(x => x.Title, note.Title).Create();
        var error = ResultTypes.Conflict($"Note with Title [{createNoteDto.Title}] already exists");

        DataContext.Notes.Add(note);
        await DataContext.SaveChangesAsync();

        // Act
        var actual = await HttpClient.PostAsJsonAsync("api/notes", createNoteDto);

        // Assert
        actual.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var content = await actual.Content.ReadFromJsonAsync<ResultError>();
        content.Should().Be(error);
    }

    [Fact]
    public async Task DeleteNote_IdExists_NoContent()
    {
        // Arrange
        var note = _fixture.Create<Note>();

        DataContext.Notes.Add(note);
        await DataContext.SaveChangesAsync();

        // Act
        var actual = await HttpClient.DeleteAsync($"api/notes/{note.Id}");

        // Assert
        actual.StatusCode.Should().Be(HttpStatusCode.NoContent);

        DataContext.Notes.AsNoTracking().SingleOrDefault(x => x.Id == note.Id).Should().BeNull();
    }

    [Fact]
    public async Task DeleteNote_IdDoesNotExist_NotFound()
    {
        // Arrange
        var note = _fixture.Create<Note>();
        var error = ResultTypes.NotFound($"Note with Id [{note.Id}] does not exist");

        // Act
        var actual = await HttpClient.DeleteAsync($"api/notes/{note.Id}");

        // Assert
        actual.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var content = await actual.Content.ReadFromJsonAsync<ResultError>();
        content.Should().Be(error);
    }

    [Fact]
    public async Task UpdateNote_IdExistsAndTitleIsUnique_NoContent()
    {
        // Arrange
        var note = _fixture.Create<Note>();
        var updateNoteDto = _fixture.Create<UpdateNoteDto>();

        DataContext.Notes.Add(note);
        await DataContext.SaveChangesAsync();

        // Act
        var actual = await HttpClient.PutAsJsonAsync($"api/notes/{note.Id}", updateNoteDto);

        // Assert
        actual.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var updatedNote = DataContext.Notes.AsNoTracking().SingleOrDefault(x => x.Id == note.Id);
        updatedNote.Should().BeEquivalentTo(updateNoteDto);
    }

    [Fact]
    public async Task UpdateNote_IdExistsAndTitleIsNotUpdated_NoContent()
    {
        // Arrange
        var note = _fixture.Create<Note>();
        var updateNoteDto = _fixture.Build<UpdateNoteDto>().With(x => x.Title, note.Title).Create();

        DataContext.Notes.Add(note);
        await DataContext.SaveChangesAsync();

        // Act
        var actual = await HttpClient.PutAsJsonAsync($"api/notes/{note.Id}", updateNoteDto);

        // Assert
        actual.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var updatedNote = DataContext.Notes.AsNoTracking().SingleOrDefault(x => x.Id == note.Id);
        updatedNote.Should().BeEquivalentTo(updateNoteDto);
    }

    [Fact]
    public async Task UpdateNote_IdDoesNotExist_NotFound()
    {
        // Arrange
        var note = _fixture.Create<Note>();
        var updateNoteDto = _fixture.Create<UpdateNoteDto>();
        var error = ResultTypes.NotFound($"Note with Id [{note.Id}] does not exist");

        // Act
        var actual = await HttpClient.PutAsJsonAsync($"api/notes/{note.Id}", updateNoteDto);

        // Assert
        actual.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var content = await actual.Content.ReadFromJsonAsync<ResultError>();
        content.Should().Be(error);
    }

    [Fact]
    public async Task UpdateNote_TitleAlreadyExists_Conflict()
    {
        // Arrange
        var note = _fixture.Create<Note>();
        var noteWithConflict = _fixture.Create<Note>();
        var updateNoteDto = _fixture.Build<UpdateNoteDto>().With(x => x.Title, noteWithConflict.Title).Create();
        var error = ResultTypes.Conflict($"Note with Title [{updateNoteDto.Title}] already exists");

        DataContext.Notes.Add(note);
        DataContext.Notes.Add(noteWithConflict);
        await DataContext.SaveChangesAsync();

        // Act
        var actual = await HttpClient.PutAsJsonAsync($"api/notes/{note.Id}", updateNoteDto);

        // Assert
        actual.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var content = await actual.Content.ReadFromJsonAsync<ResultError>();
        content.Should().Be(error);
    }
}