namespace core.Notes;

public class Note(Guid id, string title, string description)
{
    public Guid Id { get; init; } = id;
    public string Title { get; set; } = title;
    public string Description { get; set; } = description;
}