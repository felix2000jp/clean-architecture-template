namespace core.Results;

public interface IResult
{
    public bool IsError { get; }
    public ResultError Error { get; }
}

public interface IResult<out TValue> : IResult
{
    public TValue Value { get; }
}