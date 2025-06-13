using System.ComponentModel;

namespace LasseVK.Extensions.Tasks;

public static class CancellationTokenExtensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static CancellationTokenAwaiter GetAwaiter(this CancellationToken cancellationToken) => new(cancellationToken);
}