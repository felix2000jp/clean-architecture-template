namespace api.Notes.Contracts;

public record NoteResponse(Guid Id, string Title, string Description);