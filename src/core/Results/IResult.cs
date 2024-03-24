namespace core.Results;

/// <summary>
/// Interface for the <see cref="Result"/> object. This is a type-less interface.
/// </summary>
/// <remarks>
/// This interface is intended for use when the underlying type of the <see cref="Result"/> object is unknown.
/// </remarks>
public interface IResult
{
    public bool IsError { get; }
    public ResultError Error { get; }
}

/// <summary>
/// Interface for the <see cref="Result{TResultData}"/> object.
/// </summary>
public interface IResult<out TResultData> : IResult
{
    public TResultData Value { get; }
}