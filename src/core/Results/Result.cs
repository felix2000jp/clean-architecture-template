namespace core.Results;

public readonly struct Result<TResultData> : IResult<TResultData>
{
    private readonly TResultData? _value;
    private readonly ResultError? _error;

    private Result(bool isError, TResultData? value = default, ResultError? error = default)
    {
        IsError = isError;
        _error = error;
        _value = value;
    }

    public bool IsError { get; }
    public TResultData Value => _value ?? throw new InvalidOperationException("Value was expected but none were found");
    public ResultError Error => _error ?? throw new InvalidOperationException("Error was expected but none were found");


    public TResult Match<TResult>(Func<TResultData, TResult> onValue, Func<ResultError, TResult> onError)
    {
        return IsError ? onError(Error) : onValue(Value);
    }

    public static implicit operator Result<TResultData>(TResultData value) => new(false, value: value);
    public static implicit operator Result<TResultData>(ResultError error) => new(true, error: error);
}

public readonly struct Result : IResult<ResultValue>
{
    private readonly ResultValue? _value;
    private readonly ResultError? _error;

    private Result(bool isError, ResultValue? value = default, ResultError? error = default)
    {
        IsError = isError;
        _error = error;
        _value = value;
    }

    public bool IsError { get; }
    public ResultValue Value => _value ?? throw new InvalidOperationException("Value was expected but none were found");
    public ResultError Error => _error ?? throw new InvalidOperationException("Error was expected but none were found");


    public TResult Match<TResult>(Func<ResultValue, TResult> onValue, Func<ResultError, TResult> onError)
    {
        return IsError ? onError(Error) : onValue(Value);
    }

    public static implicit operator Result(ResultValue value) => new(false, value: value);
    public static implicit operator Result(ResultError error) => new(true, error: error);
}