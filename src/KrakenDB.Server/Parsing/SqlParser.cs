using KrakenDB.Server.Network;
using KrakenDB.Server.Parsing.Statements;

namespace KrakenDB.Server.Parsing
{
    public class SqlParser
    {
        private readonly Dictionary<string, ISqlStatementParser> _statementParsers;
        
        public SqlParser()
        {
            _statementParsers = new Dictionary<string, ISqlStatementParser>(StringComparer.OrdinalIgnoreCase)
            {
                { "SELECT", new SelectParser() },
                { "INSERT", new InsertParser() }
            };
        }

        public KrakenCommand Parse(string sqlQuery)
        {
            if (string.IsNullOrWhiteSpace(sqlQuery))
                throw new Exception("Empty SQL command.");

            Queue<string> tokens = Tokenize(sqlQuery);

            if (tokens.Count == 0)
                throw new Exception("Invalid SQL command.");

            string action = tokens.Dequeue().ToUpper(); 

            if (_statementParsers.TryGetValue(action, out ISqlStatementParser parser))
            {
                return parser.Parse(tokens);
            }
            
            throw new Exception($"Command not supported: {action}");
            
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