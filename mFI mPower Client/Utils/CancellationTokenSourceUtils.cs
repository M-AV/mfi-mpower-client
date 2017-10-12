using System.Threading;

namespace mFI_mPower_Client
{
    public static class CancellationTokenSourceUtils
    {
        public static CancellationTokenSource Link(this CancellationToken first, CancellationToken second)
        {
            return CancellationTokenSource.CreateLinkedTokenSource(first, second);
        }
    }
}
