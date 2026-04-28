using System.Text;
using KrakenDB.Server.Network;
using KrakenDB.Server.Storage;

namespace KrakenDB.Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("🐙 Starting KrakenDB Server...\n");

            DiskManager disk = new DiskManager("master");
            disk.InitializeDatabase();

            KrakenHost host = new KrakenHost(disk, 5432);
            await host.StartAsync();
        }
    }
}