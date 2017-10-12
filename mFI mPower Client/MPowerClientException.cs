using System;

namespace mFI_mPower_Client
{
    internal class MPowerClientException : Exception
    {
        public MPowerClientException()
        {
        }

        public MPowerClientException(string message) : base(message)
        {
        }

        public MPowerClientException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}