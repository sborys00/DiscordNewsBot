using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DiscordNewsBot.Models
{
    public class Config
    {
        private readonly string configFileName;

        public Config(string configFileName)
        {
            this.configFileName = configFileName;
        }

        public string[] LoadWebhookUrls()
        {
            List<string> urls = new List<string>();
            using(StreamReader sr = new StreamReader(configFileName))
            {
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    urls.Add(line);
                }
            }
            if(urls.Count < 1)
            {
                throw new Exception("Atleast one webhook urls is required!");
            }
            return urls.ToArray();
        }
    }
}
