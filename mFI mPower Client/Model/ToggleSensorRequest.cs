using Jil;

namespace mFI_mPower_Client
{
    public sealed class ToggleSensorRequest
    {
        [JilDirective(Name = "output")]
        public int Output { get; set; }

        [JilDirective(Name = "port")]
        public int Port { get; set; }
    }
}
