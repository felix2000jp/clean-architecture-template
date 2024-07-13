using AutoMapper;
using core.Notes;

namespace service.Notes;

public class NoteProfile : Profile
{
    public NoteProfile()
    {
        CreateMap<Note, NoteDto>();
    }
}