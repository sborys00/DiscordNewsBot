using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DiscordNewsBot.Models
{
    class Scheduler
    {
        //in ms
        const int defInterval = 1000;
        private readonly Webhooks _webhooks;
        public Scheduler(Webhooks webhooks)
        {
            this._webhooks = webhooks;
        }

        public async Task SendArticlesAsync(List<Article> articles, string webhookUrl)
        {
            Timer timer = new System.Timers.Timer(defInterval);
            timer.AutoReset = false;
            timer.Enabled = true;
            foreach(Article article in articles)
            {
                timer.Elapsed += async (source, e) => await SendTimer(source, e, article, webhookUrl);
            }
        }

        private async Task SendTimer(Object source, ElapsedEventArgs e, Article article, string webhookUrl)
        {
            await SendArticleAsync(article, webhookUrl);
        }

        public async Task SendArticleAsync(Article article, string webhookUrl)
        {
            await _webhooks.SendWebhook(webhookUrl, article);
        }


    }
}
