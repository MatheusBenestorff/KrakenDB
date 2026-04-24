using System.Text;

namespace KrakenDB.Server.Storage
{
    public enum PageType : byte
    {
        Master = 1,  
        Data = 2     
    }

    public class Page
    {
        public const int PAGE_SIZE = 8192;
        public const int HEADER_SIZE = 64; 

        // Header
        public int PageId { get; set; }
        public PageType Type { get; set; }
        public int FreeSpace { get; set; }

        // Payload
        public byte[] Payload { get; set; } 

        public Page(int pageId, PageType type)
        {
            PageId = pageId;
            Type = type;
            Payload = new byte[PAGE_SIZE - HEADER_SIZE];
            FreeSpace = Payload.Length;
        }

        public byte[] Serialize()
        {
            byte[] buffer = new byte[PAGE_SIZE];

            BitConverter.GetBytes(PageId).CopyTo(buffer, 0);    
            buffer[4] = (byte)Type;                             
            BitConverter.GetBytes(FreeSpace).CopyTo(buffer, 5); 

            Payload.CopyTo(buffer, HEADER_SIZE);

            return buffer;
        }

        public static Page Deserialize(byte[] buffer)
        {
            if (buffer.Length != PAGE_SIZE)
                throw new ArgumentException("The buffer must be exactly 8 KB.");

            int pageId = BitConverter.ToInt32(buffer, 0); 
            PageType type = (PageType)buffer[4];          
            int freeSpace = BitConverter.ToInt32(buffer, 5); 

            Page page = new Page(pageId, type) { FreeSpace = freeSpace };

            Array.Copy(buffer, HEADER_SIZE, page.Payload, 0, PAGE_SIZE - HEADER_SIZE);

            return page;
        }

        public void WriteText(string text)
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            textBytes.CopyTo(Payload, 0);
            FreeSpace -= textBytes.Length;
        }

        public string ReadText()
        {
            return Encoding.UTF8.GetString(Payload).TrimEnd('\0');
        }
    }
}