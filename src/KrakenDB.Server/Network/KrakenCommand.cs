namespace KrakenDB.Server.Network
{
    public class KrakenCommand
    {
        public string Action { get; set; } 
        public string Table { get; set; } 
        public List<string> Columns { get; set; } = new List<string>(); 
        public string Data { get; set; }

        public string ConditionColumn { get; set; }
        public string ConditionValue { get; set; }
    }
}