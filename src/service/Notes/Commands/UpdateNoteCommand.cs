using core.Notes;
using core.Results;
using FluentValidation;
using MediatR;

namespace service.Notes.Commands;

public record UpdateNoteCommand(Guid Id, string Title, string Description) : IRequest<Result>;

public sealed class UpdateNoteCommandHandler(INoteService noteService) : IRequestHandler<UpdateNoteCommand, Result>
{
    private readonly INoteService _noteService = noteService;

    public async Task<Result> Handle(UpdateNoteCommand request, CancellationToken cancellationToken = default)
    {
        return await _noteService.UpdateNote(request.Id, request.Title, request.Description, cancellationToken);
    }
}

public sealed class UpdateNoteCommandValidator : AbstractValidator<UpdateNoteCommand>
{
    public UpdateNoteCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
    }
}