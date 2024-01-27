using System.Net;

namespace core.Results;

public readonly record struct ResultValue;

public readonly record struct ResultError(
    string Title,
    string Detail,
    HttpStatusCode Status,
    Dictionary<string, string[]>? ValidationErrors = default);