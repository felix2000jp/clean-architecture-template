using AutoMapper;
using core.Notes;
using core.Results;
using FluentValidation;
using MediatR;

namespace service.Notes.Queries;

public record GetNotesQuery(int Page) : IRequest<Result<IEnumerable<NoteDto>>>;

public sealed class GetNotesQueryHandler(
    IMapper mapper,
    INoteService noteService)
    : IRequestHandler<GetNotesQuery, Result<IEnumerable<NoteDto>>>
{
    private readonly IMapper _mapper = mapper;
    private readonly INoteService _noteService = noteService;

    public async Task<Result<IEnumerable<NoteDto>>> Handle(
        GetNotesQuery request,
        CancellationToken cancellationToken = default)
    {
        var result = await _noteService.GetNotes(request.Page, cancellationToken);
        return result.Then(_mapper.Map<IEnumerable<NoteDto>>);
    }
}

public sealed class GetNotesQueryValidator : AbstractValidator<GetNotesQuery>
{
    public GetNotesQueryValidator()
    {
        RuleFor(x => x.Page).NotNull().GreaterThanOrEqualTo(0);
    }
}