using api.Extensions;
using api.Notes.Contracts;
using AutoMapper;
using MediatR;
using service.Notes.Commands;
using service.Notes.Queries;

namespace api.Notes;

public static class NoteRoutes
{
    public static void MapNoteRoutes(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("api/notes").WithTags("Notes").WithOpenApi();

        group
            .MapGet("", GetNotes)
            .Produces<IEnumerable<NoteResponse>>()
            .Produces(500)
            .WithName(nameof(GetNotes));

        group
            .MapGet("{id:guid}", GetNoteById)
            .Produces<NoteResponse>()
            .Produces(404)
            .Produces(500)
            .WithName(nameof(GetNoteById));

        group
            .MapPost("", CreateNote)
            .Accepts<CreateNoteBody>("application/json")
            .Produces(201)
            .Produces(400)
            .Produces(409)
            .Produces(500)
            .WithName(nameof(CreateNote));

        group
            .MapDelete("{id:guid}", DeleteNote)
            .Produces(204)
            .Produces(400)
            .Produces(404)
            .Produces(500)
            .WithName(nameof(DeleteNote));

        group
            .MapPut("{id:guid}", UpdateNote)
            .Accepts<CreateNoteBody>("application/json")
            .Produces(204)
            .Produces(400)
            .Produces(404)
            .Produces(409)
            .Produces(500)
            .WithName(nameof(UpdateNote));
    }

    private static async Task<IResult> GetNotes(
        IMapper mapper,
        ISender sender,
        int page)
    {
        var request = new GetNotesQuery(page);
        var result = await sender.Send(request);

        return result.Match(
            value => Results.Ok(mapper.Map<IEnumerable<NoteResponse>>(value)),
            error => error.ToProblemDetails());
    }

    private static async Task<IResult> GetNoteById(
        IMapper mapper,
        ISender sender,
        Guid id)
    {
        var request = new GetNoteByIdQuery(id);
        var result = await sender.Send(request);

        return result.Match(
            value => Results.Ok(mapper.Map<NoteResponse>(value)),
            error => error.ToProblemDetails());
    }

    private static async Task<IResult> CreateNote(
        ISender sender,
        CreateNoteBody body)
    {
        var request = new CreateNoteCommand(body.Title, body.Description);
        var result = await sender.Send(request);

        return result.Match(
            _ => Results.Created(),
            error => error.ToProblemDetails());
    }

    private static async Task<IResult> DeleteNote(
        ISender sender,
        Guid id)
    {
        var request = new DeleteNoteCommand(id);
        var result = await sender.Send(request);

        return result.Match(
            _ => Results.NoContent(),
            error => error.ToProblemDetails());
    }

    private static async Task<IResult> UpdateNote(
        ISender sender,
        Guid id,
        UpdateNoteBody body)
    {
        var request = new UpdateNoteCommand(id, body.Title, body.Description);
        var result = await sender.Send(request);

        return result.Match(
            _ => Results.NoContent(),
            error => error.ToProblemDetails());
    }
}