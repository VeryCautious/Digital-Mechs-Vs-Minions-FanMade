namespace Utilities;

public static class WaitHandleExtensions
{
    public static bool WaitOne(this WaitHandle handle, TimeSpan timeout, CancellationToken cancellationToken)
    {
        var n = WaitHandle.WaitAny(new[] { handle, cancellationToken.WaitHandle }, timeout);
        switch (n)
        {
            case WaitHandle.WaitTimeout:
                return false;
            case 0:
                return true;
            default:
                cancellationToken.ThrowIfCancellationRequested();
                return false; // never reached
        }
    }
}