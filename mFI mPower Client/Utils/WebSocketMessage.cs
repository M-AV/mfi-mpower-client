using System;
using System.Net.WebSockets;

namespace mFI_mPower_Client
{
    public sealed class WebSocketMessage
    {
        public static readonly WebSocketMessage EmptyMessage = new WebSocketMessage(String.Empty, WebSocketMessageType.Text);
        public static readonly WebSocketMessage CloseMessage = new WebSocketMessage(null, WebSocketMessageType.Close);

        public string Data { get; private set; }
        public WebSocketMessageType MessageType { get; private set; }

        public WebSocketMessage(string data, WebSocketMessageType messageType)
        {
            Data = data;
            MessageType = messageType;
        }
    }
}
