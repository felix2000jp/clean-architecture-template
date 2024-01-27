using core.Notes;
using core.Results;
using FluentValidation;
using MediatR;

namespace service.Notes.Queries;

public record GetNoteByIdQuery(Guid Id) : IRequest<Result<Note>>;

public sealed class GetNoteByIdQueryHandler(INoteService noteService) : IRequestHandler<GetNoteByIdQuery, Result<Note>>
{
    private readonly INoteService _noteService = noteService;

    public async Task<Result<Note>> Handle(GetNoteByIdQuery request, CancellationToken cancellationToken = default)
    {
        return await _noteService.GetNoteById(request.Id, cancellationToken);
    }
}

public sealed class GetNoteByIdQueryValidator : AbstractValidator<GetNoteByIdQuery>
{
    public GetNoteByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}