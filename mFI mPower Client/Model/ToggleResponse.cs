using Jil;

namespace mFI_mPower_Client
{
    public sealed class ToggleResponse
    {
        [JilDirective(Name = "status")]
        public RequestStatus Status { get; set; }
    }
}
