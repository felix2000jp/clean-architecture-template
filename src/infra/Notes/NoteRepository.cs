using core.Notes;
using infra.Context;
using Microsoft.EntityFrameworkCore;

namespace infra.Notes;

public class NoteRepository(DataContext dataContext) : INoteRepository
{
    private readonly DataContext _dataContext = dataContext;
    private const int ItemsPerPage = 20;

    public async Task<IEnumerable<Note>> GetNotes(int page, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Notes.Skip(ItemsPerPage * page).Take(ItemsPerPage).ToListAsync(cancellationToken);
    }

    public async Task<Note?> GetNoteById(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Notes.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Note?> GetNoteByTitle(string title, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Notes.SingleOrDefaultAsync(x => x.Title == title, cancellationToken);
    }

    public void CreateNote(Note note)
    {
        _dataContext.Notes.Add(note);
    }

    public void DeleteNote(Note note)
    {
        _dataContext.Notes.Remove(note);
    }
}