using System.Net;

namespace core.Results;

/// <summary>
/// Represents a value.
/// </summary>
/// <remarks>
/// This is to be used to represent a default return type.
/// </remarks>
public readonly record struct ResultValue;

/// <summary>
/// Represents an error.
/// </summary>
public readonly record struct ResultError(
    HttpStatusCode Status,
    string Title,
    string? Detail = default,
    Dictionary<string, string[]>? ValidationErrors = default);