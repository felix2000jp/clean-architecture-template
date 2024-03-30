using core;
using core.Notes;
using core.Results;
using Serilog;

namespace service.Notes;

public class NoteService(ILogger logger, IUnitOfWork unitOfWork, INoteRepository noteRepository) : INoteService
{
    private readonly ILogger _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly INoteRepository _noteRepository = noteRepository;

    public async Task<Result<IEnumerable<Note>>> GetNotes(int page, CancellationToken cancellationToken = default)
    {
        var notes = (await _noteRepository.GetNotes(page, cancellationToken)).ToList();

        _logger.Information("Found {size} notes at page {page}", notes.Count, page);
        return notes;
    }

    public async Task<Result<Note>> GetNoteById(Guid id, CancellationToken cancellationToken = default)
    {
        var note = await _noteRepository.GetNoteById(id, cancellationToken);

        if (note is null)
        {
            _logger.Warning("Note with Id {id} does not exist", id);
            return ResultTypes.NotFound();
        }

        _logger.Information("Found note with id {noteId}", note.Id);
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
            _logger.Warning("Note with title {noteTitle} already exists", note.Title);
            return ResultTypes.Conflict();
        }

        note = new Note(Guid.NewGuid(), title, description);

        _noteRepository.CreateNote(note);
        await _unitOfWork.CommitChanges();

        _logger.Information("Created note with id {noteId}", note.Id);
        return ResultTypes.Ok();
    }

    public async Task<Result> DeleteNote(Guid id, CancellationToken cancellationToken = default)
    {
        var note = await _noteRepository.GetNoteById(id, cancellationToken);

        if (note is null)
        {
            _logger.Warning("Note with id {noteId} does not exist", id);
            return ResultTypes.NotFound();
        }

        _noteRepository.DeleteNote(note);
        await _unitOfWork.CommitChanges();

        _logger.Information("Deleted note with id {noteId}", note.Id);
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
            _logger.Warning("Note with id {noteId} does not exist", id);
            return ResultTypes.NotFound();
        }

        if (noteWithTitle is not null && noteWithTitle != note)
        {
            _logger.Warning("Note with title {noteTitle} already exists", title);
            return ResultTypes.Conflict();
        }

        note.Title = title;
        note.Description = description;
        await _unitOfWork.CommitChanges();

        _logger.Information("Updated note with {noteId}", note.Id);
        return ResultTypes.Ok();
    }
}