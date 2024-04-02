using AutoMapper;
using core.Notes;
using core.Results;
using FluentValidation;
using MediatR;

namespace service.Notes.Queries;

public record GetNoteByIdQuery(Guid Id) : IRequest<Result<NoteDto>>;

public sealed class GetNoteByIdQueryHandler(
    IMapper mapper,
    INoteService noteService) 
    : IRequestHandler<GetNoteByIdQuery, Result<NoteDto>>
{
    private readonly IMapper _mapper = mapper;
    private readonly INoteService _noteService = noteService;

    public async Task<Result<NoteDto>> Handle(GetNoteByIdQuery request, CancellationToken cancellationToken = default)
    {
        var result = await _noteService.GetNoteById(request.Id, cancellationToken);
        return result.Then(_mapper.Map<NoteDto>);
    }
}

public sealed class GetNoteByIdQueryValidator : AbstractValidator<GetNoteByIdQuery>
{
    public GetNoteByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}