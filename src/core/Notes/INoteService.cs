using core.Results;

namespace core.Notes;

public interface INoteService
{
    Task<Result<IEnumerable<Note>>> GetNotes(int page, CancellationToken cancellationToken = default);

    Task<Result<Note>> GetNoteById(Guid id, CancellationToken cancellationToken = default);

    Task<Result> CreateNote(string title, string description, CancellationToken cancellationToken = default);

    Task<Result> DeleteNote(Guid id, CancellationToken cancellationToken = default);

    Task<Result> UpdateNote(Guid id, string title, string description, CancellationToken cancellationToken = default);
}