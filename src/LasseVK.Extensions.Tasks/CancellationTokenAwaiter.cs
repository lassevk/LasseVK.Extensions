using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LasseVK.Extensions.Tasks;

[EditorBrowsable(EditorBrowsableState.Never)]
public readonly struct CancellationTokenAwaiter : ICriticalNotifyCompletion
{
    private readonly CancellationToken _cancellationToken;
    private readonly bool _taskCancelledExceptionIsOk;

    public CancellationTokenAwaiter(CancellationToken cancellationToken, bool taskCancelledExceptionIsOk)
    {
        _cancellationToken = cancellationToken;
        _taskCancelledExceptionIsOk = taskCancelledExceptionIsOk;
    }

    public bool IsCompleted => _cancellationToken.IsCancellationRequested;

    public object GetResult()
    {
        if (_cancellationToken.IsCancellationRequested && !_taskCancelledExceptionIsOk)
        {
            _cancellationToken.ThrowIfCancellationRequested();
            throw new InvalidOperationException("The cancellation token has not yet completed");
        }

        return null!;
    }

    public void OnCompleted(Action continuation) => _cancellationToken.Register(continuation);
    public void UnsafeOnCompleted(Action continuation) => _cancellationToken.Register(continuation);

    public CancellationTokenAwaiter GetAwaiter() => this;
}