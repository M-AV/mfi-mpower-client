using System;

namespace mFI_mPower_Client
{
    // Login: curl -X POST -d "username=<username>&password=<password>" -b "AIROS_SESSIONID=<cookie of 32 random digits>" <ip>/login.cgi

    // GET: curl -b "AIROS_SESSIONID=01234567890123456789012345678901" 10.0.0.1/sensors
    // GET: curl -b "AIROS_SESSIONID=01234567890123456789012345678901" 10.0.0.1/sensors/1
    // GET: curl -b "AIROS_SESSIONID=01234567890123456789012345678901" 10.0.0.1/sensors/1/output

    // SET: curl -X PUT -d output=0 -b "AIROS_SESSIONID=01234567890123456789012345678901" 10.0.0.1/sensors/1
    // SET: curl -X POST -d dimmer_level = 50 - b "AIROS_SESSIONID=01234567890123456789012345678901" 10.1.7.9/sensors/1 
    // SET: curl -X POST -d "output=1&dimmer_level=50" -b "AIROS_SESSIONID=01234567890123456789012345678901" 10.1.7.9/sensors/1

    // Logout: curl -b "AIROS_SESSIONID=01234567890123456789012345678901" 10.0.1/logout.cgi

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
