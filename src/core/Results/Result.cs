namespace core.Results;

public readonly record struct Result : IResult
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

public readonly record struct Result<TValue> : IResult
{
    private readonly TValue? _value;
    private readonly ResultError? _error;

    private Result(bool isError, TValue? value = default, ResultError? error = default)
    {
        IsError = isError;
        _error = error;
        _value = value;
    }

    public bool IsError { get; }
    public TValue Value => _value ?? throw new InvalidOperationException("Value was expected but none were found");
    public ResultError Error => _error ?? throw new InvalidOperationException("Error was expected but none were found");


    public TResult Match<TResult>(Func<TValue, TResult> onValue, Func<ResultError, TResult> onError)
    {
        return IsError ? onError(Error) : onValue(Value);
    }

    public static implicit operator Result<TValue>(TValue value) => new(false, value: value);
    public static implicit operator Result<TValue>(ResultError error) => new(true, error: error);
}