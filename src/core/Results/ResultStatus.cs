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
    Dictionary<string, string[]>? ValidationErrors = default)
{
    /// <summary>
    /// Gets a message that describes the error.
    /// </summary>
    /// <returns>Returns either the detail (if not null) or the title.</returns>
    public string GetMessage() => Detail ?? Title;
};