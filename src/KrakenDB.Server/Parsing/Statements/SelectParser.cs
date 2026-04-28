using System;
using System.Collections.Generic;
using KrakenDB.Server.Network;

namespace KrakenDB.Server.Parsing.Statements
{
    public class SelectParser : ISqlStatementParser
    {
        public KrakenCommand Parse(Queue<string> tokens)
        {
            var command = new KrakenCommand { Action = "SELECT" };

            if (tokens.Count == 0)
                throw new Exception("Syntax error. Columns not specified in the SELECT statement.");

            command.Columns.Add(tokens.Dequeue()); 

            if (tokens.Count == 0 || tokens.Dequeue().ToUpper() != "FROM")
                throw new Exception("Syntax error. The word ‘FROM’ was expected.");

            if (tokens.Count == 0)
                throw new Exception("Syntax error. Table name not specified.");

            command.Table = tokens.Dequeue();

            // if (tokens.Count > 0 && tokens.Peek().ToUpper() == "WHERE") { ... }

            return command;
        }
    }
}