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
                    KrakenResult result = new KrakenResult { Success = true };

                    // Execution Engine
                    if (command != null)
                    {
                        if (command.Action.ToUpper() == "INSERT")
                        {
                            string recordText = $"{command.Table}|{command.Data}";
                            byte[] recordBytes = Encoding.UTF8.GetBytes(recordText);

                            int currentPageId = 0;
                            Page currentPage = _disk.ReadPage(currentPageId);
                            
                            while (currentPage.NextPageId != -1)
                            {
                                currentPage = _disk.ReadPage(currentPage.NextPageId);
                            }

                            if (!currentPage.InsertRecord(recordBytes))
                            {
                                Page newPage = _disk.AllocateNewPage(PageType.Data);
                                currentPage.NextPageId = newPage.PageId;
                                _disk.WritePage(currentPage); 
                                
                                newPage.InsertRecord(recordBytes);
                                _disk.WritePage(newPage);  
                            }
                            else
                            {
                                _disk.WritePage(currentPage); 
                            }

                            result.Message = $"1 registro inserido na tabela {command.Table}.";
                            Console.WriteLine($"[Execution] Sucesso: Gravado no disco físico.");
                        }
                        else if (command.Action.ToUpper() == "SELECT")
                        {
                            var foundRows = new System.Collections.Generic.List<string>();
                            int currentPageId = 0;

                            while (currentPageId != -1)
                            {
                                Page page = _disk.ReadPage(currentPageId);
                                var records = page.GetAllRecords();
                                
                                foreach (var rec in records)
                                {
                                    string text = Encoding.UTF8.GetString(rec);
                                    
                                    if (text.StartsWith(command.Table + "|"))
                                    {
                                        foundRows.Add(text.Substring(command.Table.Length + 1));
                                    }
                                }
                                currentPageId = page.NextPageId;
                            }

                            result.Message = $"{foundRows.Count} registros encontrados.";
                            result.Data = foundRows.ToArray();
                            Console.WriteLine($"[Execution] Sucesso: {foundRows.Count} linhas lidas do disco.");
                        }
                        else
                        {
                            result.Success = false;
                            result.Message = "Comando desconhecido. Use INSERT ou SELECT.";
                        }
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