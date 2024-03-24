using System.Net;

namespace core.Results;

public struct ResultTypes
{
    public static ResultValue Ok() => new();

    public static ResultError BadRequest(string detail) => new(
        "Bad Request",
        detail,
        HttpStatusCode.BadRequest);

    public static ResultError NotFound(string detail) => new(
        "Not Found",
        detail,
        HttpStatusCode.NotFound);

    public static ResultError Conflict(string detail) => new(
        "Conflict",
        detail,
        HttpStatusCode.Conflict);

    public static ResultError InternalServerError(string detail) => new(
        "Internal Server Error",
        detail,
        HttpStatusCode.InternalServerError);

    public static ResultError UnexpectedError() => new(
        "Unexpected Error",
        "An unexpected server error occurred",
        HttpStatusCode.InternalServerError);

    public static ResultError ValidationError(Dictionary<string, string[]> validationErrors) => new(
        "Validation Error",
        "One or more validation errors occurred",
        HttpStatusCode.BadRequest,
        validationErrors);

    public static ResultError CustomError(
        string title,
        string detail,
        HttpStatusCode status,
        Dictionary<string, string[]>? validationErrors = default) => new(title, detail, status, validationErrors);
}