namespace core.Results;

public interface IResult
{
    public bool IsError { get; }
    public ResultError Error { get; }
}