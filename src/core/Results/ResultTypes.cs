using System.Net;

namespace core.Results;

/// <summary>
/// A factory for <see cref="Result"/> and other needed extension methods.
/// </summary>
public static class ResultTypes
{
    /// <summary>
    /// Creates an <see cref="Result{TResultData}"/> instance with the given <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// This is only needed when in a method with a return type of <see cref="Result{TResultData}"/>,
    /// the returning value is an interface - implicit converters do not accept interfaces. 
    /// </remarks>
    public static Result<TResultData> ToResult<TResultData>(this TResultData value) => value;

    /// <summary>
    /// Creates an <see cref="Result{TResultData}"/> instance with the given <paramref name="error"/>.
    /// </summary>
    public static Result<TResultData> ToResult<TResultData>(this ResultError error) => error;

    /// <summary>
    /// Creates a <see cref="ResultValue"/>.
    /// </summary>
    /// <remarks>
    /// Is used to return when the method is <see cref="Result{TResultData}"/>
    /// and TResultData is of type <see cref="ResultValue"/>.
    /// </remarks>
    /// <returns>Returns a <see cref="ResultValue"/>.</returns>
    public static ResultValue Ok() => new();

    /// <summary>
    /// Creates an <see cref="ResultError"/> of type <see cref="HttpStatusCode.BadRequest"/>
    /// in the problem details format with a given detail message.
    /// </summary>
    /// <param name="detail">The detail message.</param>
    /// <returns>Returns a <see cref="ResultError"/>.</returns>
    public static ResultError BadRequest(string detail) => new(
        "Bad Request",
        detail,
        HttpStatusCode.BadRequest);

    /// <summary>
    /// Creates an <see cref="ResultError"/> of type <see cref="HttpStatusCode.NotFound"/>
    /// in the problem details format with a given detail message.
    /// </summary>
    /// <param name="detail">The detail message.</param>
    /// <returns>Returns a <see cref="ResultError"/>.</returns>
    public static ResultError NotFound(string detail) => new(
        "Not Found",
        detail,
        HttpStatusCode.NotFound);

    /// <summary>
    /// Creates an <see cref="ResultError"/> of type <see cref="HttpStatusCode.Conflict"/>
    /// in the problem details format with a given detail message.
    /// </summary>
    /// <param name="detail">The detail message.</param>
    /// <returns>Returns a <see cref="ResultError"/>.</returns>
    public static ResultError Conflict(string detail) => new(
        "Conflict",
        detail,
        HttpStatusCode.Conflict);

    /// <summary>
    /// Creates an <see cref="ResultError"/> of type <see cref="HttpStatusCode.InternalServerError"/>
    /// in the problem details format with a given detail message.
    /// </summary>
    /// <param name="detail">The detail message.</param>
    /// <returns>Returns a <see cref="ResultError"/>.</returns>
    public static ResultError InternalServerError(string detail) => new(
        "Internal Server Error",
        detail,
        HttpStatusCode.InternalServerError);

    /// <summary>
    /// Creates an <see cref="ResultError"/> of type <see cref="HttpStatusCode.InternalServerError"/>
    /// in the problem details format.
    /// </summary>
    /// <remarks>
    /// This is to be used only when an unexpected error occurs during the exception middleware.
    /// /// Otherwise use <see cref="InternalServerError"/>
    /// </remarks>
    /// <returns>Returns a <see cref="ResultError"/>.</returns>
    public static ResultError UnexpectedError() => new(
        "Unexpected Error",
        "An unexpected server error occurred",
        HttpStatusCode.InternalServerError);

    /// <summary>
    /// Creates an <see cref="ResultError"/> of type <see cref="HttpStatusCode.BadRequest"/>
    /// in the problem details format with the given validation errors.
    /// </summary>
    /// <remarks>
    /// This is to be used only when the validation of a request fails on the request validation step.
    /// Otherwise use <see cref="BadRequest"/>
    /// </remarks>
    /// <param name="validationErrors">The validation errors.</param>
    /// <returns>Returns a <see cref="ResultError"/>.</returns>
    public static ResultError ValidationError(Dictionary<string, string[]> validationErrors) => new(
        "Validation Error",
        "One or more validation errors occurred",
        HttpStatusCode.BadRequest,
        validationErrors);

    /// <summary>
    /// Creates an <see cref="ResultError"/> in the problem details format
    /// with a given title, detail message, status code and validation errors.
    /// </summary>
    /// <remarks>
    /// This is intended to be used when no other method in <see cref="ResultTypes"/> fits the situation.
    /// When possible use the other methods in <see cref="ResultTypes"/>.
    /// </remarks>
    /// <param name="title">the title.</param>
    /// <param name="detail">The detail message.</param>
    /// <param name="status">the status code</param>
    /// <param name="validationErrors">the validation errors.</param>
    /// <returns>Returns a <see cref="ResultError"/>.</returns>
    public static ResultError CustomError(
        string title,
        string detail,
        HttpStatusCode status,
        Dictionary<string, string[]>? validationErrors = default) => new(title, detail, status, validationErrors);
}