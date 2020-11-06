using System.Threading.Tasks;

namespace DiscordNewsBot.Models
{
    public interface IWebhooks
    {
        Task SendWebhook(string url, Article article);
    }
}