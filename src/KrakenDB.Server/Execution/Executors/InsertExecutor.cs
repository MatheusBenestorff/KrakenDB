using System.Text;
using KrakenDB.Server.Network;
using KrakenDB.Server.Storage;

namespace KrakenDB.Server.Execution.Executors
{
    public class InsertExecutor : IQueryExecutor
    {
        public KrakenResult Execute(KrakenCommand command, DiskManager disk)
        {
            string recordText = $"{command.Table}|{command.Data}";
            byte[] recordBytes = Encoding.UTF8.GetBytes(recordText);

            int currentPageId = 0;
            Page currentPage = disk.ReadPage(currentPageId);
            
            while (currentPage.NextPageId != -1)
            {
                currentPage = disk.ReadPage(currentPage.NextPageId);
            }

            if (!currentPage.InsertRecord(recordBytes))
            {
                Page newPage = disk.AllocateNewPage(PageType.Data);
                currentPage.NextPageId = newPage.PageId;
                disk.WritePage(currentPage); 
                
                newPage.InsertRecord(recordBytes);
                disk.WritePage(newPage);     
            }
            else
            {
                disk.WritePage(currentPage); 
            }

            Console.WriteLine($"[Execution] Success: Saved to the physical disc.");
            return new KrakenResult { Success = true, Message = $"1 record inserted into the table{command.Table}." };
        }
    }
}