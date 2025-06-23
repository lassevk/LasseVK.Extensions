using System.ComponentModel;

namespace LasseVK.Extensions.Tasks;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class CancellationTokenExtensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static CancellationTokenAwaiter GetAwaiter(this CancellationToken cancellationToken) => new(cancellationToken, true);

    public static CancellationTokenAwaiter AwaitThrowsTaskCancelledException(this CancellationToken cancellationToken) => new(cancellationToken, false);
}