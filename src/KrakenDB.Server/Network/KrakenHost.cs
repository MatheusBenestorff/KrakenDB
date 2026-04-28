using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using KrakenDB.Server.Storage;

namespace KrakenDB.Server.Network
{
    public class KrakenHost
    {
        private readonly int _port;
        private readonly DiskManager _disk;

        public KrakenHost(DiskManager disk, int port = 5432)
        {
            _disk = disk;
            _port = port;
        }

        public async Task StartAsync()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, _port);
            listener.Start();
            
            Console.WriteLine($"\n[Network] 🐙 KrakenHost is up and running! Listening for connections on port {_port}...");

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                _ = ProcessConnectionAsync(client);
            }
        }

        private async Task ProcessConnectionAsync(TcpClient client)
        {
            try
            {
                using NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[8192];

                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead > 0)
                {
                    string jsonRequest = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"\n[Network] Command Received: {jsonRequest}");

                    KrakenCommand command = JsonSerializer.Deserialize<KrakenCommand>(jsonRequest);

                    
                    KrakenResult result = new KrakenResult
                    {
                        Success = true,
                        Message = $"The {command?.Action} command was successfully received and interpreted by KrakenDB!"
                    };

                    string jsonResponse = JsonSerializer.Serialize(result);
                    byte[] responseBytes = Encoding.UTF8.GetBytes(jsonResponse);

                    await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Network] Communication error: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }
    }
}