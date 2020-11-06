using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiscordNewsBot.Models
{
    public class Webhooks : IWebhooks
    {
        public async Task SendWebhook(string url, Article article)
        {
            using (WebClient webClient = new WebClient())
            {
                Embed embed = new Embed(article);
                string json = "{\"embeds\" : [" + JsonSerializer.Serialize(embed) + "]}";

                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {

                    // Do the actual request and await the response
                    var httpResponse = await httpClient.PostAsync(url, httpContent);

                    // If the response contains content we want to read it!
                    if (httpResponse.Content != null)
                    {
                        string responseContent = await httpResponse.Content.ReadAsStringAsync();
                        if (responseContent.Length > 0)
                        {
                            await Logger.LogAsync(responseContent);
                        }
                    }
                }
            }
        }
    }

}
