using AutoMapper;
using core.Results;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace api.Extensions;

public static class ResultExtensions
{
    public static IResult ToOk<TDto>(this object value, IMapper mapper) => Results.Ok(mapper.Map<TDto>(value));

    public static IResult ToProblemDetails(this ResultError error) => Results.Problem(
        title: error.Title,
        detail: error.Detail,
        statusCode: (int)error.Status,
        extensions: new Dictionary<string, object?> { { "validationErrors", error.ValidationErrors } });
}