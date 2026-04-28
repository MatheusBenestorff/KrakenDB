using System.Text;
using KrakenDB.Server.Network;
using KrakenDB.Server.Storage;

namespace KrakenDB.Server.Execution.Executors
{
    public class SelectExecutor : IQueryExecutor
    {
        public KrakenResult Execute(KrakenCommand command, DiskManager disk)
        {
            var foundRows = new List<string>();
            int currentPageId = 0;

            while (currentPageId != -1)
            {
                Page page = disk.ReadPage(currentPageId);
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

            Console.WriteLine($"[Execution] Success: {foundRows.Count} rows read from the disk.");
            return new KrakenResult 
            { 
                Success = true, 
                Message = $"{foundRows.Count} records found.", 
                Data = foundRows.ToArray() 
            };
        }
    }
}