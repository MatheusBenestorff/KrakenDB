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

            Console.WriteLine("\n[KrakenDB] Reading the disk...");
            Page pageLida = disk.ReadPage(0);

            Console.WriteLine($"\n--- RAIO-X DA PÁGINA {pageLida.PageId} ---");
            Console.WriteLine($"Tipo do Bloco: {pageLida.Type}");
            Console.WriteLine($"Espaço Livre: {pageLida.FreeSpace} bytes");
            Console.WriteLine($"Conteúdo lido dos bytes: {pageLida.ReadText()}");
            Console.WriteLine($"--------------------------");

            Console.WriteLine("\n[KrakenDB] File System (KFS) is ready!");
            Console.ReadLine();
        }
    }
}