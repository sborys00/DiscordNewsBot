using System.Text;
using System.Net;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Serilog;

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
                    var httpResponse = await httpClient.PostAsync(url, httpContent);

                    if (httpResponse.Content != null)
                    {
                        string responseContent = await httpResponse.Content.ReadAsStringAsync();
                        if (responseContent.Length > 0)
                        {
                            Log.Logger.Warning(responseContent);
                        }
                    }
                }
            }
        }
    }

}
