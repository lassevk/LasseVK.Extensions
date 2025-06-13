using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LasseVK.Extensions.Tasks;

[EditorBrowsable(EditorBrowsableState.Never)]
public readonly struct CancellationTokenAwaiter : INotifyCompletion, ICriticalNotifyCompletion
{
    private readonly CancellationToken _cancellationToken;

    public CancellationTokenAwaiter(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
    }

    public bool IsCompleted => _cancellationToken.IsCancellationRequested;

    public object GetResult()
    {
        _cancellationToken.ThrowIfCancellationRequested();
        throw new InvalidOperationException("The cancellation token has not yet completed");
    }

    public void OnCompleted(Action continuation) => _cancellationToken.Register(continuation);
    public void UnsafeOnCompleted(Action continuation) => _cancellationToken.Register(continuation);
}