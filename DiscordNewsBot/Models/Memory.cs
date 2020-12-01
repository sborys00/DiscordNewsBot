using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DiscordNewsBot.Models
{

    public class Memory : IMemory
    {
        private readonly string fileName;
        private readonly int fileCleaningThreshold;
        private readonly IConfiguration _config;
        private FileStream fs;
        private StreamReader reader;
        private StreamWriter writer;
        private List<string> archivedUrls = new List<string>();

        public Memory(IConfiguration config)
        {
            this._config = config;
            this.fileName = _config.GetValue<string>("MemoryFileName");
            this.fileCleaningThreshold = _config.GetValue <int>("FileCleaningThreshold");
            //To make sure file exists
            OpenFile();
            CloseFile();

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

        private int ClearOldUrls()
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

        private void CloseFile()
        {
            if (fs != null)
                fs.Close();
        }
        private void OpenFile()
        {
            this.fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            this.reader = new StreamReader(fs);
            this.writer = new StreamWriter(fs);
            writer.Flush();
        }

        private string ShortenUrl(string url)
        {
            int startIndex = url.Substring(0, url.Length - 1).LastIndexOf('/');
            return url.Substring(startIndex + 1, url.Length - startIndex);
        }
    }
}
