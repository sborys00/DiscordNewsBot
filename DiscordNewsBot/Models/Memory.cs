using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace DiscordNewsBot.Models
{
    public class Memory
    {
        private const string fileName = "archive.txt";
        private const int fileCleaningThreshold = 50;
        private FileStream fs;
        private StreamReader reader;
        private StreamWriter writer;
        private List<string> archivedUrls = new List<string>();

        public Memory()
        {
            ClearOldUrls();
            string url;
            while((url = reader.ReadLine()) != null)
            {
                archivedUrls.Add(url);
            }
        }

        ~Memory()
        {
            CloseFile();
        }

        public void SaveUrl(string url)
        {
            if(archivedUrls.Count > fileCleaningThreshold)
            {
                ClearOldUrls();
            }
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

        public int ClearOldUrls()
        {
            int removedItems = 0;

            CloseFile();

            var lines = System.IO.File.ReadAllLines(fileName);
            if(lines.Length > fileCleaningThreshold)
            {
                int filesToRemove = lines.Count() - (int)(fileCleaningThreshold * 1.25);
                File.WriteAllLines(fileName, lines.Skip(filesToRemove).ToArray());
                removedItems += filesToRemove;
            }
            OpenFile();
            return removedItems;
        }

        public void CloseFile()
        {
            if (fs != null)
                fs.Close();
        }
        public void OpenFile()
        {
            this.fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            this.reader = new StreamReader(fs);
            this.writer = new StreamWriter(fs);
            writer.Flush();
        }
    }
}
