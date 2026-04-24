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
                CreateEmptyDatabase();
            }
            else
            {
                Console.WriteLine("[Storage] Database found and loaded.");
            }
        }

        private void CreateEmptyDatabase()
        {
            using (FileStream fs = new FileStream(_filePath, FileMode.Create, FileAccess.Write))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                byte[] pageZero = new byte[PAGE_SIZE];
                
                writer.Write(pageZero);
                
                Console.WriteLine($"[Storage] Page 0 allocated with {PAGE_SIZE} bytes.");
            }
        }
    }
}