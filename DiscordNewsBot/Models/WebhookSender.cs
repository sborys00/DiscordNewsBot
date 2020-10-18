using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DiscordNewsBot.Models
{
    class WebhookSender
    {      
        const int defInterval = 1000; //time in ms
        private readonly Webhooks _webhooks;
        private readonly string[] webhookUrls;
        Queue<Article> articlesToSend = new Queue<Article>();
        Timer timer = new System.Timers.Timer(defInterval);

        public WebhookSender(Webhooks webhooks, string[] webhookUrls)
        {
            this._webhooks = webhooks;
            this.webhookUrls = webhookUrls;
            timer.AutoReset = true;
        }

        public void EnqueueArticles(List<Article> articles)
        {
            foreach(Article article in articles)
            {
                this.articlesToSend.Enqueue(article);
            }
            timer.Start();
            timer.Elapsed += async (source, e) => await SendEvent(source, e);
        }

        private async Task SendEvent(Object source, ElapsedEventArgs e)
        {
            if(articlesToSend.Count > 0)
            {
                await SendOneArticleAsync(this.articlesToSend.Dequeue());
            }

            if(articlesToSend.Count == 0)
            {
                this.timer.Stop();
            }
        }

        public async Task SendOneArticleAsync(Article article)
        {
            foreach(string url in webhookUrls)
            {
                await _webhooks.SendWebhook(url, article);
            }
        }


    }
}
