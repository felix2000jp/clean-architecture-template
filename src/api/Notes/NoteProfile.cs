using api.Notes.Dtos;
using AutoMapper;
using core.Notes;

namespace api.Notes;

public class NoteProfile : Profile
{
    public NoteProfile()
    {
        CreateMap<Note, NoteDto>();
    }
}