using KrakenDB.Server.Storage;

namespace KrakenDB.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("🐙 Starting KrakenDB Server...\n");

            DiskManager disk = new DiskManager("master");
            disk.InitializeDatabase();

            Console.WriteLine("\n[KrakenDB] System ready and awaiting commands...");
            Console.ReadLine(); 
        }
    }
}