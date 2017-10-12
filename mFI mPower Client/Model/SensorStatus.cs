using Jil;

namespace mFI_mPower_Client
{
    public sealed class SensorStatus
    {
        [JilDirective(Name = "sensors")]
        public Sensor[] Sensors { get; set; }

        [JilDirective(Name = "status")]
        public RequestStatus Status { get; set; }
    }
}
