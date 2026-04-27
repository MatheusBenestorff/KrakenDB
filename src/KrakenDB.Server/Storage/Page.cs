using System;
using System.Collections.Generic;
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
        
        // Control
        public short RecordCount { get; set; } 
        public int NextPageId { get; set; }   

        // Payload
        public byte[] Payload { get; set; } 

        public Page(int pageId, PageType type)
        {
            PageId = pageId;
            Type = type;
            Payload = new byte[PAGE_SIZE - HEADER_SIZE];
            FreeSpace = Payload.Length;
            RecordCount = 0;
            NextPageId = -1;
        }

        public byte[] Serialize()
        {
            byte[] buffer = new byte[PAGE_SIZE];

            // Header
            BitConverter.GetBytes(PageId).CopyTo(buffer, 0);    
            buffer[4] = (byte)Type;                             
            BitConverter.GetBytes(FreeSpace).CopyTo(buffer, 5); 
            BitConverter.GetBytes(RecordCount).CopyTo(buffer, 9); 
            BitConverter.GetBytes(NextPageId).CopyTo(buffer, 11); 

            // Payload
            Payload.CopyTo(buffer, HEADER_SIZE);

            return buffer;
        }

        public static Page Deserialize(byte[] buffer)
        {
            if (buffer.Length != PAGE_SIZE)
                throw new ArgumentException("The buffer must be exactly 8 KB.");

            Page page = new Page(
                pageId: BitConverter.ToInt32(buffer, 0),
                type: (PageType)buffer[4]
            );

            page.FreeSpace = BitConverter.ToInt32(buffer, 5);
            page.RecordCount = BitConverter.ToInt16(buffer, 9);
            page.NextPageId = BitConverter.ToInt32(buffer, 11);

            Array.Copy(buffer, HEADER_SIZE, page.Payload, 0, PAGE_SIZE - HEADER_SIZE);

            return page;
        }

        // Register

        public bool InsertRecord(byte[] recordData)
        {
            int requiredSpace = recordData.Length + 2;

            if (requiredSpace > FreeSpace)
            {
                return false;
            }

            int writeOffset = Payload.Length - FreeSpace;

            short dataLength = (short)recordData.Length;
            BitConverter.GetBytes(dataLength).CopyTo(Payload, writeOffset);

            recordData.CopyTo(Payload, writeOffset + 2);

            FreeSpace -= requiredSpace;
            RecordCount++;

            return true; 
        }

        public List<byte[]> GetAllRecords()
        {
            List<byte[]> records = new List<byte[]>();
            int readOffset = 0;

            for (int i = 0; i < RecordCount; i++)
            {
                short dataLength = BitConverter.ToInt16(Payload, readOffset);
                
                byte[] recordData = new byte[dataLength];
                Array.Copy(Payload, readOffset + 2, recordData, 0, dataLength);
                
                records.Add(recordData);

                readOffset += (2 + dataLength);
            }

            return records;
        }
    }
}