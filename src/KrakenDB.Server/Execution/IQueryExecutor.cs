using KrakenDB.Server.Network;
using KrakenDB.Server.Storage;

namespace KrakenDB.Server.Execution
{
    public interface IQueryExecutor
    {
        KrakenResult Execute(KrakenCommand command, DiskManager disk);
    }
}