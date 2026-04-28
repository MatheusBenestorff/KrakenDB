using KrakenDB.Server.Network;

namespace KrakenDB.Server.Parsing
{
    public interface ISqlStatementParser
    {
        KrakenCommand Parse(Queue<string> tokens);
    }
}