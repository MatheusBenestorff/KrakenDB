namespace KrakenDB.Server.Network
{ 
    public class KrakenResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string[] Data { get; set; } 
    }
}