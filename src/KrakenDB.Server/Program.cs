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

            Console.WriteLine("\n[KrakenDB] Iniciando inserção em massa de 500 registros...");
            
            Page currentPage = disk.ReadPage(0);
            int totalInseridos = 0;

            for (int i = 1; i <= 500; i++)
            {
                byte[] data = Encoding.UTF8.GetBytes($"REGISTRO_DE_TESTE_NUMERO_{i}_COM_BASTANTE_TEXTO_PARA_ENCHER_O_VAGAO_RAPIDO");
                
                if (!currentPage.InsertRecord(data))
                {
                    Page newPage = disk.AllocateNewPage(PageType.Data);
                    
                    currentPage.NextPageId = newPage.PageId;
                    
                    disk.WritePage(currentPage);
                    
                    currentPage = newPage;
                    
                    currentPage.InsertRecord(data);
                }
                totalInseridos++;
            }
            
            disk.WritePage(currentPage);
            Console.WriteLine($"[KrakenDB] {totalInseridos} registros gravados com sucesso!");

            Console.WriteLine("\n[KrakenDB] Lendo todo o banco de dados seguindo os ponteiros...");
            
            int paginaAtualId = 0;
            int totalLido = 0;

            while (paginaAtualId != -1) 
            {
                Page page = disk.ReadPage(paginaAtualId);
                
                Console.WriteLine($"\n--- LENDO PÁGINA {page.PageId} ---");
                Console.WriteLine($"Linhas aqui: {page.RecordCount} | Espaço Livre: {page.FreeSpace} bytes | Próxima Página: {page.NextPageId}");
                
                totalLido += page.RecordCount;
                
                paginaAtualId = page.NextPageId; 
            }

            Console.WriteLine($"\n[KrakenDB] Leitura finalizada! Total de linhas encontradas: {totalLido}");
            Console.WriteLine("--------------------------\n");
        }
    }
}