using Jil;

namespace mFI_mPower_Client
{
    public sealed class ThrottleRequest : IMPowerModel
    {
        [JilDirective(Name = "time")]
        public int Time { get; set; }
    }
}
