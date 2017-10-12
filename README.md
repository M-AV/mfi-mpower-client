# mFI mPower Client

A client for the mFI mPower i quickly threw together. Currently, it is able to toggle the relays, as well as get the current relay status. Getting the status can be done on a per-request basis, but it is also possible to subscribe to the WebSocket on the device and get "live readings".

### WebSocketClient usage

    var websocketClient = new MPowerWebSocketClient("deviceIp")
    websocketClient.OnStatusUpdate += (o, e) =>
    {
        foreach (var sensor in e.Message.Sensors)
        {
          	Console.WriteLine("   - " + sensor.Port + " : " + sensor.Power + " : " + sensor.Relay + " : " + sensor.Output);
        }
    };
    await websocketClient.ConnectAsync("username", "password");
    
    
