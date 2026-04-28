using KrakenDB.Server.Network;
using KrakenDB.Server.Storage;
using KrakenDB.Server.Execution.Executors;

namespace KrakenDB.Server.Execution
{
    public class QueryEngine
    {
        private readonly Dictionary<string, IQueryExecutor> _executors;

        public QueryEngine()
        {
            _executors = new Dictionary<string, IQueryExecutor>(StringComparer.OrdinalIgnoreCase)
            {
                { "INSERT", new InsertExecutor() },
                { "SELECT", new SelectExecutor() }
            };
        }

        public KrakenResult Process(KrakenCommand command, DiskManager disk)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.Action))
                return new KrakenResult { Success = false, Message = "Invalid command." };

            if (_executors.TryGetValue(command.Action, out IQueryExecutor executor))
            {
                return executor.Execute(command, disk);
            }

            return new KrakenResult { Success = false, Message = $"The ‘{command.Action}’ command is not supported." };
        }
    }
}