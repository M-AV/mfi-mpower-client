using Jil;
using System;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace mFI_mPower_Client
{
    public sealed class MPowerWebSocketClient : IDisposable
    {
        public event EventHandler<OnUpdateEventArgs> OnStatusUpdate;

        public bool IsConnected { get; private set; }

        private readonly string _ipAddress;
        private readonly MPowerClient _webClient;
        private ClientWebSocket _webSocket;
        private CancellationTokenSource _cancellationTokenSource;

        public MPowerWebSocketClient(string ipAddress)
        {
            _webClient = new MPowerClient(ipAddress);
            _ipAddress = ipAddress;
        }

        private ClientWebSocket CreateClientWebSocket()
        {
            var webSocket = new ClientWebSocket();
            webSocket.Options.AddSubProtocol("mfi-protocol");
            webSocket.Options.Cookies = new CookieContainer();
            return webSocket;
        }

        public Task ConnectAsync(string username, string password) => ConnectAsync(username, password, CancellationToken.None);
        public async Task ConnectAsync(string username, string password, CancellationToken cancellationToken)
        {
            if (_webSocket?.State == WebSocketState.Open || _webSocket?.State == WebSocketState.Connecting) { return; }

            _webSocket = CreateClientWebSocket();
            _cancellationTokenSource = new CancellationTokenSource();

            await _webClient.LoginAsync(username, password);

            const int port = 7681;
            const int wssPort = 7682;

            var uri = new Uri($"ws://{_ipAddress}:{port}?c={_webClient.SessionId}");
            _webSocket.Options.Cookies.Add(uri, new Cookie("AIROS_SESSIONID", _webClient.SessionId));

            var combinedTokenSource = _cancellationTokenSource.Token.Link(cancellationToken);

            await _webSocket.ConnectAsync(uri, combinedTokenSource.Token);

            StartListening(combinedTokenSource.Token);

            IsConnected = true;
        }

        protected async void StartListening(CancellationToken cancellationToken)
        {
            while (_webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                WebSocketMessage webSocketMessage = null;
                string receivedMsg = null;
                try
                {
                    webSocketMessage = await _webSocket.ReceiveMessageAsync(cancellationToken);
                    receivedMsg = (string)webSocketMessage.Data;
                }
                catch (WebSocketException e) when (e.WebSocketErrorCode == WebSocketError.InvalidState)
                {
                    break;
                }
                
                SensorStatus parsedMsg;
                try
                {
                    parsedMsg = JSON.Deserialize<SensorStatus>(receivedMsg);
                }
                catch (DeserializationException)
                {
                    // Failed to parse message - Happens occasionally - We are currently ignoring this
                    continue;
                }

                OnStatusUpdate?.Invoke(this, new OnUpdateEventArgs(receivedMsg, parsedMsg));
            }
        }

        public async Task DisconnectAsync()
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "User disconnect", CancellationToken.None);
            _cancellationTokenSource?.Cancel();
            if (_webClient.IsConnected)
            {
                await _webClient.LogoutAsync();
            }
            _webSocket = null;
            
            IsConnected = false;
        }

        public async Task Send<T>(T request) where T : IMPowerModel
        {
            if (request == null) { throw new ArgumentNullException(nameof(request)); }
            if (!IsConnected) { throw new MPowerClientException("Client not connected. Cannot send request."); }

            var parsedCommand = JSON.Serialize(request);

            await _webSocket.SendMessageAsync(parsedCommand, CancellationToken.None);
        }

        public void Dispose()
        {
            _webClient?.Dispose();
            _webSocket?.Dispose();
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}
