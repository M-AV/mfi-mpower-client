using Jil;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace mFI_mPower_Client
{
    public sealed class MPowerClient : IDisposable
    {
        public bool IsConnected { get; private set; }

        internal string SessionId { get; private set; }

        private readonly Lazy<HttpClient> _client;
        private readonly Uri _ipAddress;

        public MPowerClient(string ipAddress)
        {
            if (!IPAddress.TryParse(ipAddress, out var address)) { throw new ArgumentException($"IP address not valid: {ipAddress}", nameof(ipAddress)); }
            _ipAddress = new Uri("http://" + ipAddress);

            _client = new Lazy<HttpClient>(() =>
            {
                SessionId = GenerateSessionId();
                var cookieContainer = new CookieContainer();
                cookieContainer.Add(_ipAddress, new Cookie("AIROS_SESSIONID", SessionId));
                var handler = new HttpClientHandler()
                {
                    CookieContainer = cookieContainer
                };
                var client = new HttpClient(handler)
                {
                    BaseAddress = _ipAddress
                };
                
                return client;
            });
        }

        public async Task LoginAsync(string username, string password)
        {
            var url = $"/login.cgi";

            var credentials = new Dictionary<string, string>()
            {
                { "username", username },
                { "password", password }
            };
            
            var content = new FormUrlEncodedContent(credentials);

            var responseMessage = await _client.Value.PostAsync(url, content);
            var response = await responseMessage.Content.ReadAsStringAsync();

            if (response.Contains("Invalid credentials."))
            {
                throw new InvalidCredentialException();
            }
            IsConnected = true;
        }

        public async Task<SensorStatus> GetStatusAsync(int port)
        {
            var url = $"/sensors/{port}";

            var response = await _client.Value.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            var result = JSON.Deserialize<SensorStatus>(json);

            return result;
        }
        public async Task<SensorStatus> GetStatusAsync()
        {
            var url = $"/sensors";

            var response = await _client.Value.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            var result = JSON.Deserialize<SensorStatus>(json);
            return result;
        }

        public async Task<ToggleResponse> Set(int port, bool status)
        {
            var url = $"/sensors/{port}";

            var credentials = new Dictionary<string, string>()
            {
                { "output", status ? "1" : "0" }
            };
            var content = new FormUrlEncodedContent(credentials);

            var responseMessage = await _client.Value.PostAsync(url, content);
            var response = await responseMessage.Content.ReadAsStringAsync();

            var result = JSON.Deserialize<ToggleResponse>(response);
            return result;
        }

        public async Task LogoutAsync()
        {
            var url = $"/logout.cgi";
            var content = new StringContent("");

            var response = await _client.Value.PostAsync(url, content);
        }

        public void Dispose()
        {
            if (_client.IsValueCreated)
            {
                _client.Value?.Dispose();
            }
        }

        /// <summary>
        /// TODO - Generate proper token
        /// </summary>
        /// <returns></returns>
        private string GenerateSessionId()
        {
            var sb = new StringBuilder();
            var buffer = new byte[4];
            using (var cryptoProvider = RandomNumberGenerator.Create())
            {
                for (int i = 0; i < 32; i++)
                {
                    var value = RandomNumber(cryptoProvider, buffer);
                    sb.Append(value);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generates a random number in the range [0-9] using the supplied random number generator.
        /// </summary>
        private static int RandomNumber(RandomNumberGenerator numberGenerator, byte[] buffer)
        {
            while (true)
            {
                numberGenerator.GetBytes(buffer);
                uint rand = BitConverter.ToUInt32(buffer, 0);

                long remainder = UInt32.MaxValue % 10;
                if (rand < UInt32.MaxValue - remainder) 
                {
                    return (int)(rand % 10);
                }
            }
        }
    }
}
