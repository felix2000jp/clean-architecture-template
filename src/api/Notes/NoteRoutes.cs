using api.Extensions;
using api.Notes.Dtos;
using AutoMapper;
using MediatR;
using service.Notes.Commands;
using service.Notes.Queries;

namespace api.Notes;

public static class NoteRoutes
{
    public static void InjectNoteRoutes(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("api/notes")
            .WithTags("Notes")
            .WithOpenApi();

        group.MapGet("", GetNotes)
            .Produces<IEnumerable<NoteDto>>()
            .WithName("GetNotes");
        
        group.MapGet("{id:guid}", GetNoteById)
            .Produces<NoteDto>()
            .Produces(404)
            .WithName("GetNoteById");

        group.MapPost("", CreateNote)
            .Accepts<CreateNoteDto>("application/json")
            .Produces(201)
            .Produces(400)
            .Produces(409)
            .WithName("CreateNote");

        group.MapDelete("{id:guid}", DeleteNote)
            .Produces(204)
            .Produces(400)
            .Produces(404)
            .WithName("DeleteNote");

        group.MapPut("{id:guid}", UpdateNote)
            .Accepts<UpdateNoteDto>("application/json")
            .Produces(204)
            .Produces(400)
            .Produces(404)
            .Produces(409)
            .WithName("UpdateNote");
    }

    private static async Task<IResult> GetNotes(
        IMapper mapper,
        ISender sender,
        int page)
    {
        var request = new GetNotesQuery(page);
        var result = await sender.Send(request);

        return result.Match(
            value => value.ToOk<IEnumerable<NoteDto>>(mapper),
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
            value => value.ToOk<NoteDto>(mapper),
            error => error.ToProblemDetails());
    }

    private static async Task<IResult> CreateNote(
        ISender sender,
        CreateNoteDto dto)
    {
        var request = new CreateNoteCommand(dto.Title, dto.Description);
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
        UpdateNoteDto dto)
    {
        var request = new UpdateNoteCommand(id, dto.Title, dto.Description);
        var result = await sender.Send(request);

        return result.Match(
            _ => Results.NoContent(),
            error => error.ToProblemDetails());
    }
}