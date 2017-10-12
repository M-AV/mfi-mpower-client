using mFI_mPower_Client;
using System;
using System.Threading.Tasks;

namespace mFI_mPower.ConsoleClient
{
    class Program
    {
        private static bool _printSingle;

        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            Console.WriteLine("IP Address: ");
            var ip = Console.ReadLine();
            Console.WriteLine("Username: ");
            var username = Console.ReadLine();
            Console.WriteLine("Password: ");
            var password = Console.ReadLine();

            using (var websocketClient = new MPowerWebSocketClient(ip))
            {
                websocketClient.OnStatusUpdate += Client_OnStatusUpdate;
                while (true)
                {
                    Console.WriteLine();
                    Console.WriteLine("L = Login. G = Get status. S = Set status. O = Logout.");
                    Console.WriteLine();
                    var option = Console.ReadLine();
                    switch (option)
                    {
                        case "L":
                        case "l":
                            await websocketClient.ConnectAsync(username, password);
                            break;
                        case "G":
                        case "g":
                            //var result = await client.GetStatusAsync();
                            //Console.WriteLine("Result: ");
                            //foreach (var sensor in result.Sensors)
                            //{
                            //    Console.WriteLine(sensor.Port + ". Relay: " + sensor.Relay + " Power: " + sensor.Power + " Output: " + sensor.Output + " Voltage: " + sensor.Voltage + " PowerFactor: " + sensor.PowerFactor);
                            //}
                            break;

                        case "G1":
                        case "g1":
                        case "G2":
                        case "g2":
                        case "G3":
                        case "g3":
                            //var result2 = await client.GetStatusAsync(int.Parse(option[1].ToString()));
                            //Console.WriteLine("Result: " + Environment.NewLine);
                            //foreach (var sensor in result2.Sensors)
                            //{
                            //    Console.WriteLine(sensor.Port + ". Relay: " + sensor.Relay + " Power: " + sensor.Power + " Output: " + sensor.Output + " Voltage: " + sensor.Voltage + " PowerFactor: " + sensor.PowerFactor);
                            //}
                            break;

                        case "S1":
                        case "S2":
                        case "S3":
                        case "S1+":
                        case "S2+":
                        case "S3+":
                        case "s1":
                        case "s2":
                        case "s3":
                        case "s1+":
                        case "s2+":
                        case "s3+":
                            var port = int.Parse(option[1].ToString());
                            bool status = option.Contains("+");
                            if (websocketClient.IsConnected)
                            {
                                await websocketClient.Send(new ToggleRequest()
                                {
                                    Sensors = new[] 
                                    {
                                        new ToggleSensorRequest()
                                        {
                                            Output = status ? 1 : 0,
                                            Port = port
                                        }
                                    }
                                });
                                break;
                            }
                            
                            break;
                        case "O":
                        case "o":
                            await websocketClient.DisconnectAsync();
                            break;
                        case "print":
                            _printSingle = true;
                            break;
                    }
                }
            }
        }

        private static void Client_OnStatusUpdate(object sender, OnUpdateEventArgs e)
        {
            foreach (var sensor in e.Message.Sensors)
            {
                Console.WriteLine("   - " + sensor.Port + " : " + sensor.Power + " : " + sensor.Relay + " : " + sensor.Output);
            }
            _printSingle = false;
        }
    }
}