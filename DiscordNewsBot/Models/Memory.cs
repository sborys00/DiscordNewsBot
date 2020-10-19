using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DiscordNewsBot.Models
{
    public class Memory
    {
        private FileStream fs = new FileStream("archive.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
        private StreamReader reader;
        private StreamWriter writer;
        private List<string> archivedUrls = new List<string>();

        public Memory()
        {
            this.reader = new StreamReader(fs);
            this.writer = new StreamWriter(fs);
            string url;
            while((url = reader.ReadLine()) != null)
            {
                archivedUrls.Add(url);
            }
        }

        ~Memory()
        {
            reader.Close();
            writer.Close();
            fs.Close();
        }

        public void SaveUrl(string url)
        {
            writer.WriteLine(url);
            this.archivedUrls.Add(url);
        }

        public bool IsInArchive(string url)
        {
            return archivedUrls.Contains(url);
        }

        public void FlushWriter()
        {
            writer.Flush();
        }
    }
}
