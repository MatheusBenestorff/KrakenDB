namespace KrakenDB.Server.Storage
{
    public class DiskManager
    {
        public const int PAGE_SIZE = 8192; 
        
        private readonly string _filePath;

        public DiskManager(string databaseName)
        {
            _filePath = $"{databaseName}.kdb";
        }

        public void InitializeDatabase()
        {
            Console.WriteLine($"[Storage] Checking the physical file: {_filePath}");

            if (!File.Exists(_filePath))
            {
                Console.WriteLine("[Storage] Database not found. Creating new file...");
                File.Create(_filePath).Close();

                Page masterPage = new Page(0, PageType.Master);
                masterPage.WriteText("CATALOGO_MESTRE: NENHUMA_TABELA_CRIADA_AINDA");

                WritePage(masterPage);
                Console.WriteLine($"[Storage] Page 0 (Master Catalog) has been allocated and saved.");
            }
            else
            {
                Console.WriteLine("[Storage] Database found and loaded.");
            }
        }

        public void WritePage(Page page)
        {
            using (FileStream fs = new FileStream(_filePath, FileMode.Open, FileAccess.Write))
            {
                fs.Seek(page.PageId * PAGE_SIZE, SeekOrigin.Begin);
                
                byte[] data = page.Serialize();
                fs.Write(data, 0, data.Length);
            }
        }

        public Page ReadPage(int pageId)
        {
            using (FileStream fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
            {
                fs.Seek(pageId * PAGE_SIZE, SeekOrigin.Begin);
                
                byte[] buffer = new byte[PAGE_SIZE];
                fs.Read(buffer, 0, buffer.Length);
                
                return Page.Deserialize(buffer);
            }
        }
    }
}