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
        private readonly Memory _memory;
        private readonly string[] webhookUrls;
        Queue<Article> articlesToSend = new Queue<Article>();
        Timer timer = new System.Timers.Timer(defInterval);


        public WebhookSender(Webhooks webhooks, Memory memory, string[] webhookUrls)
        {
            this._webhooks = webhooks;
            this._memory = memory;
            this.webhookUrls = webhookUrls;
            timer.AutoReset = true;
        }

        public void EnqueueArticles(List<Article> articles)
        {
            articles = FilterOutSentArticles(articles);
            foreach(Article article in articles)
            {
                this.articlesToSend.Enqueue(article);
                _memory.SaveUrl(article.url);
            }
            _memory.FlushWriter();
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

        public List<Article> FilterOutSentArticles(List<Article> articles)
        {
            List<Article> newArticles = new List<Article>();
            foreach(Article article in articles)
            {
                if(_memory.IsInArchive(article.url) == false)
                {
                    newArticles.Add(article);
                }
            }
                return newArticles;
        }
    }
}
