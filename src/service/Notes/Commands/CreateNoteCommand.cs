using core.Notes;
using core.Results;
using FluentValidation;
using MediatR;

namespace service.Notes.Commands;

public record CreateNoteCommand(string Title, string Description) : IRequest<Result>;

public sealed class CreateNoteCommandHandler(INoteService noteService) : IRequestHandler<CreateNoteCommand, Result>
{
    private readonly INoteService _noteService = noteService;

    public async Task<Result> Handle(CreateNoteCommand request, CancellationToken cancellationToken = default)
    {
        return await _noteService.CreateNote(request.Title, request.Description, cancellationToken);
    }
}

public sealed class CreateNoteCommandValidator : AbstractValidator<CreateNoteCommand>
{
    public CreateNoteCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
    }
}