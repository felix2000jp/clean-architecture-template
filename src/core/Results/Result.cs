namespace core.Results;

/// <summary>
/// A discriminated union of a value of type <see cref="ResultValue"/>
/// and an error of type <see cref="ResultError"/>.
/// </summary>
public readonly record struct Result : IResult<ResultValue>
{
    private readonly ResultValue? _value;
    private readonly ResultError? _error;

    private Result(bool isError, ResultValue? value = default, ResultError? error = default)
    {
        IsError = isError;
        _error = error;
        _value = value;
    }

    /// <summary>
    /// Gets a value indicating whether the state is error.
    /// </summary>
    public bool IsError { get; }

    /// <summary>
    /// Gets the value in case of success.
    /// </summary>
    public ResultValue Value => _value ?? throw new InvalidOperationException("Value was expected but none were found");

    /// <summary>
    /// Gets the error in case of failure.
    /// </summary>
    public ResultError Error => _error ?? throw new InvalidOperationException("Error was expected but none were found");

    /// <summary>
    /// Executes the appropriate function based on the state of the <see cref="Result"/>.
    /// If the state is a value, the provided function <paramref name="onValue"/>
    /// is executed and its result is returned.
    /// If the state is an error, the provided function <paramref name="onError"/>
    /// is executed and its result is returned.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="onValue">The function to execute if the state is a value.</param>
    /// <param name="onError">The function to execute if the state is an error.</param>
    /// <returns>The result of the executed function.</returns>
    public TResult Match<TResult>(Func<ResultValue, TResult> onValue, Func<ResultError, TResult> onError)
    {
        return IsError ? onError(Error) : onValue(Value);
    }

    /// <summary>
    /// Creates an <see cref="Result"/> from a value of type <see cref="ResultValue"/>.
    /// </summary>
    public static implicit operator Result(ResultValue value) => new(false, value: value);

    /// <summary>
    /// Creates an <see cref="Result"/> from a value of type <see cref="ResultError"/>.
    /// </summary>
    public static implicit operator Result(ResultError error) => new(true, error: error);
}

/// <summary>
/// A discriminated union of a value of type <typeparamref name="TResultData"/>
/// and an error of type <see cref="ResultError"/>.
/// </summary>
public readonly record struct Result<TResultData> : IResult<TResultData>
{
    private readonly TResultData? _value;
    private readonly ResultError? _error;

    private Result(bool isError, TResultData? value = default, ResultError? error = default)
    {
        IsError = isError;
        _error = error;
        _value = value;
    }

    /// <summary>
    /// Gets a value indicating whether the state is error.
    /// </summary>
    public bool IsError { get; }

    /// <summary>
    /// Gets the value in case of success.
    /// </summary>
    public TResultData Value => _value ?? throw new InvalidOperationException("Value was expected but none were found");

    /// <summary>
    /// Gets the error in case of failure.
    /// </summary>
    public ResultError Error => _error ?? throw new InvalidOperationException("Error was expected but none were found");

    /// <summary>
    /// If the state is a value, the provided function <paramref name="onValue"/> is executed and
    /// its result is returned.
    /// </summary>
    /// <typeparam name="TNextValue">The type of the result.</typeparam>
    /// <param name="onValue">The function to execute if the state is a value.</param>
    /// <returns>The result from <paramref name="onValue"/> if state is value; else the <see cref="Error"/>.</returns>
    public Result<TNextValue> Then<TNextValue>(Func<TResultData, TNextValue> onValue)
    {
        return IsError ? Error : onValue(Value);
    }

    /// <summary>
    /// Executes the appropriate function based on the state of the <see cref="Result{TResultData}"/>.
    /// If the state is a value, the provided function <paramref name="onValue"/>
    /// is executed and its result is returned.
    /// If the state is an error, the provided function <paramref name="onError"/>
    /// is executed and its result is returned.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="onValue">The function to execute if the state is a value.</param>
    /// <param name="onError">The function to execute if the state is an error.</param>
    /// <returns>The result of the executed function.</returns>
    public TResult Match<TResult>(Func<TResultData, TResult> onValue, Func<ResultError, TResult> onError)
    {
        return IsError ? onError(Error) : onValue(Value);
    }

    /// <summary>
    /// Creates an <see cref="Result{TResultData}"/> from a value of type <typeparamref name="TResultData"/>.
    /// </summary>
    public static implicit operator Result<TResultData>(TResultData value) => new(false, value: value);

    /// <summary>
    /// Creates an <see cref="Result{TResultData}"/> from a value of type <see cref="ResultError"/>.
    /// </summary>
    public static implicit operator Result<TResultData>(ResultError error) => new(true, error: error);
}