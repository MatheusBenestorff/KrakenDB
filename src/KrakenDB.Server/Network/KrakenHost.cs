using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using KrakenDB.Server.Storage;
using KrakenDB.Server.Execution;
using KrakenDB.Server.Parsing;

namespace KrakenDB.Server.Network
{
    public class KrakenHost
    {
        private readonly int _port;
        private readonly DiskManager _disk;
        private readonly QueryEngine _queryEngine;
        private readonly SqlParser _sqlParser;

        public KrakenHost(DiskManager disk, int port = 5432)
        {
            _disk = disk;
            _port = port;
            _queryEngine = new QueryEngine();
            _sqlParser = new SqlParser();
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
                    string sqlRequest = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    Console.WriteLine($"\n[Network] SQL Command Received: {sqlRequest}");

                    KrakenResult result;

                    try
                    {
                        KrakenCommand command = _sqlParser.Parse(sqlRequest);

                        result = _queryEngine.Process(command, _disk);
                    }
                    catch (Exception ex)
                    {
                        result = new KrakenResult { Success = false, Message = ex.Message };
                        Console.WriteLine($"[Parser] Failure: {ex.Message}");
                    }

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