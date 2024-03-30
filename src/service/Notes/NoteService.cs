using core;
using core.Notes;
using core.Results;
using Microsoft.Extensions.Logging;

namespace service.Notes;

public class NoteService(ILogger<INoteRepository> logger, IUnitOfWork unitOfWork, INoteRepository noteRepository)
    : INoteService
{
    private readonly ILogger<INoteRepository> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly INoteRepository _noteRepository = noteRepository;

    public async Task<Result<IEnumerable<Note>>> GetNotes(int page, CancellationToken cancellationToken = default)
    {
        var notes = (await _noteRepository.GetNotes(page, cancellationToken)).ToList();

        _logger.LogInformation("Found {size} notes at page {page}", notes.Count, page);
        return notes;
    }

    public async Task<Result<Note>> GetNoteById(Guid id, CancellationToken cancellationToken = default)
    {
        var note = await _noteRepository.GetNoteById(id, cancellationToken);

        if (note is null)
        {
            return ResultTypes.NotFound($"Note with id {id} does not exist");
        }

        _logger.LogInformation("Found note with id {noteId}", note.Id);
        return note;
    }

    public async Task<Result> CreateNote(
        string title,
        string description,
        CancellationToken cancellationToken = default)
    {
        var note = await _noteRepository.GetNoteByTitle(title, cancellationToken);

        if (note is not null)
        {
            return ResultTypes.Conflict($"Note with title {note.Title} already exists");
        }

        note = new Note(Guid.NewGuid(), title, description);

        _noteRepository.CreateNote(note);
        await _unitOfWork.CommitChanges();

        _logger.LogInformation("Created note with id {noteId}", note.Id);
        return ResultTypes.Ok();
    }

    public async Task<Result> DeleteNote(Guid id, CancellationToken cancellationToken = default)
    {
        var note = await _noteRepository.GetNoteById(id, cancellationToken);

        if (note is null)
        {
            return ResultTypes.NotFound($"Note with id {id} does not exist");
        }

        _noteRepository.DeleteNote(note);
        await _unitOfWork.CommitChanges();

        _logger.LogInformation("Deleted note with id {noteId}", note.Id);
        return ResultTypes.Ok();
    }

    public async Task<Result> UpdateNote(
        Guid id,
        string title,
        string description,
        CancellationToken cancellationToken = default)
    {
        var note = await _noteRepository.GetNoteById(id, cancellationToken);
        var noteWithTitle = await _noteRepository.GetNoteByTitle(title, cancellationToken);

        if (note is null)
        {
            return ResultTypes.NotFound($"Note with id {id} does not exist");
        }

        if (noteWithTitle is not null && noteWithTitle != note)
        {
            return ResultTypes.Conflict($"Note with title {title} already exists");
        }

        note.Title = title;
        note.Description = description;
        await _unitOfWork.CommitChanges();

        _logger.LogInformation("Updated note with {noteId}", note.Id);
        return ResultTypes.Ok();
    }
}