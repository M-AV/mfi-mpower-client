using Jil;

namespace mFI_mPower_Client
{

    public sealed class Sensor : IMPowerModel
    {
        [JilDirective(Name = "port")]
        public int Port { get; set; }

        [JilDirective(Name = "output")]
        public int Output { get; set; }

        [JilDirective(Name = "power")]
        public double Power { get; set; }

        [JilDirective(Name = "enabled")]
        public int Enabled { get; set; }

        [JilDirective(Name = "current")]
        public double Current { get; set; }

        [JilDirective(Name = "voltage")]
        public double Voltage { get; set; }

        [JilDirective(Name = "powerfactor")]
        public double PowerFactor { get; set; }

        [JilDirective(Name = "relay")]
        public int Relay { get; set; }

        [JilDirective(Name = "lock")]
        public int Lock { get; set; }

        [JilDirective(Name = "thismonth")]
        public int ThisMonth { get; set; }
    }
}
