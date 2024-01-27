namespace core.Notes;

public interface INoteRepository
{
    Task<IEnumerable<Note>> GetNotes(int page, CancellationToken cancellationToken = default);

    Task<Note?> GetNoteById(Guid id, CancellationToken cancellationToken = default);

    Task<Note?> GetNoteByTitle(string title, CancellationToken cancellationToken = default);

    void CreateNote(Note note);

    void DeleteNote(Note note);
}