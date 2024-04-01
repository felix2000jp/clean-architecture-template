using core.Results;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace api.Extensions;

public static class ResultExtensions
{
    public static IResult ToProblemDetails(this ResultError error) => Results.Problem(
        statusCode: (int)error.Status,
        title: error.Title,
        detail: error.Detail,
        extensions: new Dictionary<string, object?> { { "validationErrors", error.ValidationErrors } });
}