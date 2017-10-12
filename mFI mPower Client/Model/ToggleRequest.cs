using Jil;

namespace mFI_mPower_Client
{
    public sealed class ToggleRequest : IMPowerModel
    {
        [JilDirective(Name = "sensors")]
        public ToggleSensorRequest[] Sensors { get; set; }
    }
}
