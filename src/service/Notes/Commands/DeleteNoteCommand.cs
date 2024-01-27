using core.Notes;
using core.Results;
using FluentValidation;
using MediatR;

namespace service.Notes.Commands;

public record DeleteNoteCommand(Guid Id) : IRequest<Result>;

public sealed class DeleteNoteCommandHandler(INoteService noteService) : IRequestHandler<DeleteNoteCommand, Result>
{
    private readonly INoteService _noteService = noteService;

    public async Task<Result> Handle(DeleteNoteCommand request, CancellationToken cancellationToken = default)
    {
        return await _noteService.DeleteNote(request.Id, cancellationToken);
    }
}

public sealed class DeleteNoteCommandValidator : AbstractValidator<DeleteNoteCommand>
{
    public DeleteNoteCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}