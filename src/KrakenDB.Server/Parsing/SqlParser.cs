using KrakenDB.Server.Network;

namespace KrakenDB.Server.Parsing
{
    public class SqlParser
    {
        public KrakenCommand Parse(string sqlQuery)
        {
            if (string.IsNullOrWhiteSpace(sqlQuery))
                throw new Exception("Empty SQL command.");

            Queue<string> tokens = Tokenize(sqlQuery);

            if (tokens.Count == 0)
                throw new Exception("Invalid SQL command.");

            string action = tokens.Dequeue().ToUpper(); 

            switch (action)
            {
                case "SELECT":
                    return ParseSelect(tokens);
                case "INSERT":
                    return ParseInsert(tokens);
                default:
                    throw new Exception($"Command not supported: {action}");
            }
        }

        // SUB-PARSERS

        private KrakenCommand ParseSelect(Queue<string> tokens)
        {
            var command = new KrakenCommand { Action = "SELECT" };

            string columns = tokens.Dequeue();
            command.Columns.Add(columns); 

            if (tokens.Dequeue().ToUpper() != "FROM")
                throw new Exception("Syntax error. The word ‘FROM’ was expected.");

            command.Table = tokens.Dequeue();

            if (tokens.Count > 0)
            {
                string nextWord = tokens.Peek().ToUpper();
                if (nextWord == "WHERE")
                {
                    // ParseWhere(tokens, command); 
                }
            }

            return command;
        }

        private KrakenCommand ParseInsert(Queue<string> tokens)
        {
            var command = new KrakenCommand { Action = "INSERT" };

            if (tokens.Dequeue().ToUpper() != "INTO")
                throw new Exception("Syntax error. The word ‘INTO’ was expected.");

            command.Table = tokens.Dequeue();

            if (tokens.Dequeue().ToUpper() != "VALUES")
                throw new Exception("Syntax error. The word ‘VALUES’ was expected.");

            command.Data = tokens.Dequeue(); 

            return command;
        }

        // LEXER
        private Queue<string> Tokenize(string sql)
        {
            var tokens = new Queue<string>();
            string currentWord = "";
            bool insideQuotes = false;

            for (int i = 0; i < sql.Length; i++)
            {
                char c = sql[i];

                if (c == '\'')
                {
                    insideQuotes = !insideQuotes; 
                    continue;
                }

                if (char.IsWhiteSpace(c) && !insideQuotes)
                {
                    if (!string.IsNullOrWhiteSpace(currentWord))
                    {
                        tokens.Enqueue(currentWord);
                        currentWord = "";
                    }
                    continue;
                }

                if ((c == '(' || c == ')' || c == ';') && !insideQuotes)
                {
                    continue; 
                }

                currentWord += c;
            }

            if (!string.IsNullOrWhiteSpace(currentWord))
            {
                tokens.Enqueue(currentWord);
            }

            return tokens;
        }
    }
}