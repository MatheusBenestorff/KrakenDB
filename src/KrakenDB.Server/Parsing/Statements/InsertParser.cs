using KrakenDB.Server.Network;

namespace KrakenDB.Server.Parsing.Statements
{
    public class InsertParser : ISqlStatementParser
    {
        public KrakenCommand Parse(Queue<string> tokens)
        {
            var command = new KrakenCommand { Action = "INSERT" };

            if (tokens.Count == 0 || tokens.Dequeue().ToUpper() != "INTO")
                throw new Exception("Syntax error. The word ‘INTO’ was expected.");

            if (tokens.Count == 0)
                throw new Exception("Syntax error. Table name not specified.");
                
            command.Table = tokens.Dequeue();

            if (tokens.Count == 0 || tokens.Dequeue().ToUpper() != "VALUES")
                throw new Exception("Syntax error. The word ‘VALUES’ was expected.");

            if (tokens.Count == 0)
                throw new Exception("Syntax error. Required values not provided.");
                
            command.Data = tokens.Dequeue(); 

            return command;
        }
    }
}