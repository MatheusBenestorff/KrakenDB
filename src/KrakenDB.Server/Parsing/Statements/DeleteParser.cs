using System;
using System.Collections.Generic;
using KrakenDB.Server.Network;

namespace KrakenDB.Server.Parsing.Statements
{
    public class DeleteParser : ISqlStatementParser
    {
        public KrakenCommand Parse(Queue<string> tokens)
        {
            var command = new KrakenCommand { Action = "DELETE" };

            if (tokens.Count == 0 || tokens.Dequeue().ToUpper() != "FROM")
                throw new Exception("Syntax error. The word ‘FROM’ was expected.");

            if (tokens.Count == 0)
                throw new Exception("Syntax error. Table name not specified.");

            command.Table = tokens.Dequeue();

            if (tokens.Count > 0 && tokens.Dequeue().ToUpper() == "WHERE")
            {
                if (tokens.Count < 3)
                    throw new Exception("Syntax error. Incomplete WHERE clause.");

                command.ConditionColumn = tokens.Dequeue(); 
                
                string operador = tokens.Dequeue(); 
                if (operador != "=")
                    throw new Exception("KrakenDB currently only supports the ‘=’ operator in the WHERE clause.");

                command.ConditionValue = tokens.Dequeue(); 
            }

            return command;
        }
    }
}