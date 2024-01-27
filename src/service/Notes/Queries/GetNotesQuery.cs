using core.Notes;
using core.Results;
using FluentValidation;
using MediatR;

namespace service.Notes.Queries;

public record GetNotesQuery(int Page) : IRequest<Result<IEnumerable<Note>>>;

public sealed class GetNotesQueryHandler(INoteService noteService)
    : IRequestHandler<GetNotesQuery, Result<IEnumerable<Note>>>
{
    private readonly INoteService _noteService = noteService;

    public async Task<Result<IEnumerable<Note>>> Handle(
        GetNotesQuery request,
        CancellationToken cancellationToken = default)
    {
        return await _noteService.GetNotes(request.Page, cancellationToken);
    }
}

public sealed class GetNotesQueryValidator : AbstractValidator<GetNotesQuery>
{
    public GetNotesQueryValidator()
    {
        RuleFor(x => x.Page).NotNull().GreaterThanOrEqualTo(0);
    }
}