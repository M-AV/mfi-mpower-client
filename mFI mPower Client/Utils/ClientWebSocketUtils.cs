using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mFI_mPower_Client
{
    public static class WebSocketExtensions
    {
        private const int _bufferSize = 1024;

        public static async Task SendMessageAsync(this ClientWebSocket webSocket, string message, CancellationToken cancellationToken)
        {
            if (message == null) { throw new ArgumentNullException(nameof(message)); }
            WebSocketState webSocketState;
            if ((webSocketState = webSocket.State) != WebSocketState.Open)
            {
                throw new InvalidOperationException("WebSocket connection is not open. Was: " + webSocketState);
            }

            var encodedMessage = Encoding.UTF8.GetBytes(message);
            var messageCount = (int)Math.Ceiling((double)encodedMessage.Length / _bufferSize);

            for (var i = 0; i < messageCount; i++)
            {
                var offset = _bufferSize * i;
                var bytesToSend = _bufferSize;
                var lastMessage = messageCount == i + 1;
            
                if ((bytesToSend * (i + 1)) > encodedMessage.Length)
                {
                    bytesToSend = encodedMessage.Length - offset;
                }
             
                await webSocket.SendAsync(new ArraySegment<byte>(encodedMessage, offset, bytesToSend), WebSocketMessageType.Text, lastMessage, cancellationToken);
            }
        }

        public static async Task<WebSocketMessage> ReceiveMessageAsync(this ClientWebSocket webSocket, CancellationToken cancellationToken)
        {
            var buffer = new byte[_bufferSize];
            var arraySegment = new ArraySegment<byte>(buffer);
            var stringResult = new List<byte>();

            int resultCount = 0;

            WebSocketReceiveResult result = null;
            do
            {
                result = await webSocket.ReceiveAsync(arraySegment, cancellationToken);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                else
                {
                    stringResult.AddRange(buffer);
                }
                resultCount += result.Count;
            } while (!result.EndOfMessage);

            return new WebSocketMessage(Encoding.UTF8.GetString(stringResult.ToArray(), 0, resultCount), WebSocketMessageType.Text);
        }
    }
}
