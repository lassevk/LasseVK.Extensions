using System.Runtime.CompilerServices;

namespace LasseVK.Extensions;

public readonly struct Result<TValue, TError>
{
    private readonly TValue _value;
    private readonly TError _error;

    private Result(TValue value)
    {
        IsSuccess = true;
        _value = value;
        _error = default!;
    }

    private Result(TError error)
    {
        IsSuccess = false;
        _value = default!;
        _error = error;
    }

    public bool IsSuccess { get; }

    public TValue Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => IsSuccess ? _value : throw new InvalidOperationException();
    }

    public TError Error
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => IsSuccess ? throw new InvalidOperationException() : _error;
    }

    public static implicit operator Result<TValue, TError>(TValue value) => new(value);
    public static implicit operator Result<TValue, TError>(TError error) => new(error);

    public static explicit operator TValue(Result<TValue, TError> result) => result.Value;
    public static explicit operator TError(Result<TValue, TError> result) => result.Error;
}