using System;
using System.Collections.Generic;
using System.Text;
using KrakenDB.Server.Network;
using KrakenDB.Server.Storage;

namespace KrakenDB.Server.Execution.Executors
{
    public class DeleteExecutor : IQueryExecutor
    {
        public KrakenResult Execute(KrakenCommand command, DiskManager disk)
        {
            int linhasDeletadas = 0;
            int currentPageId = 0;

            string prefixoBusca1 = $"{command.Table}|{command.ConditionValue} -";
            string prefixoBusca2 = $"{command.Table}|{command.ConditionValue}"; 

            while (currentPageId != -1)
            {
                Page page = disk.ReadPage(currentPageId);
                var records = page.GetAllRecords();
                bool paginaSofreuAlteracao = false;

                var registrosSobreviventes = new List<byte[]>();

                foreach (var rec in records)
                {
                    string text = Encoding.UTF8.GetString(rec);
                    
                    if (text.StartsWith(prefixoBusca1) || text == prefixoBusca2)
                    {
                        linhasDeletadas++;
                        paginaSofreuAlteracao = true;
                    }
                    else
                    {
                        registrosSobreviventes.Add(rec);
                    }
                }

                if (paginaSofreuAlteracao)
                {
                    page.Clear();
                    
                    foreach (var rec in registrosSobreviventes)
                    {
                        page.InsertRecord(rec);
                    }
                    
                    disk.WritePage(page);
                }

                currentPageId = page.NextPageId;
            }

            Console.WriteLine($"[Execution] Success: {linhasDeletadas} records have been physically deleted from the disk.");
            
            return new KrakenResult 
            { 
                Success = true, 
                Message = $"{linhasDeletadas} records deleted from the {command.Table} table." 
            };
        }
    }
}