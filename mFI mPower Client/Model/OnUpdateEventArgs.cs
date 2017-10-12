using System;

namespace mFI_mPower_Client
{
    public sealed class OnUpdateEventArgs : EventArgs
    {
        public SensorStatus Message { get; private set; }
        public string RawMessage { get; private set; }

        internal OnUpdateEventArgs(string rawMessage, SensorStatus status)
        {
            Message = status;
            RawMessage = rawMessage;
        }
    }
}
