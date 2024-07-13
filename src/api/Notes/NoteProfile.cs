using api.Notes.Contracts;
using AutoMapper;
using service.Notes;

namespace api.Notes;

public class NoteProfile : Profile
{
    public NoteProfile()
    {
        CreateMap<NoteDto, NoteResponse>();
    }
}