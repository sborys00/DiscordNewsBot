using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordNewsBot.Models
{
    interface IWebhookSender
    {
        void EnqueueArticles(List<Article> articles);
        Task SendOneArticleAsync(Article article);
    }
}