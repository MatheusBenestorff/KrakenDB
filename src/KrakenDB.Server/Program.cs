using System.Text;
using KrakenDB.Server.Storage;

namespace KrakenDB.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("🐙 Starting KrakenDB Server...\n");

            if (File.Exists("master.kdb")) File.Delete("master.kdb");

            DiskManager disk = new DiskManager("master");
            disk.InitializeDatabase(); 

            Console.WriteLine("\n[KrakenDB] Inserindo 3 registros no Catálogo Mestre...");
            
            Page masterPage = disk.ReadPage(0);
            
            masterPage.InsertRecord(Encoding.UTF8.GetBytes("TABELA_USUARIOS"));
            masterPage.InsertRecord(Encoding.UTF8.GetBytes("TABELA_PRODUTOS"));
            masterPage.InsertRecord(Encoding.UTF8.GetBytes("ESTE_EH_UM_REGISTRO_MUITO_MAIOR_PARA_TESTAR_O_TAMANHO_DINAMICO"));

            disk.WritePage(masterPage);
            Console.WriteLine("[KrakenDB] Registros gravados no disco físico com sucesso.");


            Console.WriteLine("\n[KrakenDB] Lendo registros direto do disco...");
            Page paginaLidaDoDisco = disk.ReadPage(0);

            Console.WriteLine($"\n--- RAIO-X DA PÁGINA {paginaLidaDoDisco.PageId} ---");
            Console.WriteLine($"Total de Linhas: {paginaLidaDoDisco.RecordCount}");
            Console.WriteLine($"Espaço Livre Restante: {paginaLidaDoDisco.FreeSpace} bytes");
            Console.WriteLine($"Próxima Página: {paginaLidaDoDisco.NextPageId}");
            
            Console.WriteLine("\nConteúdo das Linhas:");
            List<byte[]> registros = paginaLidaDoDisco.GetAllRecords();
            
            for (int i = 0; i < registros.Count; i++)
            {
                string texto = Encoding.UTF8.GetString(registros[i]);
                Console.WriteLine($"  -> Linha {i + 1} (Tamanho: {registros[i].Length} bytes): {texto}");
            }
            Console.WriteLine("--------------------------\n");
        }
    }
}