using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace DiscordNewsBot.Models
{
    class WebhookSender : IWebhookSender
    {
        const int defInterval = 1000; //time in ms
        private readonly IWebhooks _webhooks;
        private readonly IMemory _memory;
        private readonly string[] webhookUrls;
        Queue<Article> articlesToSend = new Queue<Article>();
        Timer timer = new System.Timers.Timer(defInterval);


        public WebhookSender(IMemory memory, IWebhooks webhooks)
        {
            this._webhooks = webhooks;
            this._memory = memory;

            Config cfg = new Config("config.txt");
            try
            {
                this.webhookUrls = cfg.LoadWebhookUrls();
            }
            catch (Exception e)
            {
                Logger.Log(e.Message);
                Logger.Log("Loading Webhook urls failed. Make sure \"config.txt\" exist and is not empty");
            }
            timer.AutoReset = true;
        }

        public void EnqueueArticles(List<Article> articles)
        {
            articles = FilterOutSentArticles(articles);
            articles = SortArticlesByDate(articles);
            foreach (Article article in articles)
            {
                this.articlesToSend.Enqueue(article);
            }
            timer.Start();
            timer.Elapsed += async (source, e) => await SendEvent(source, e);
        }

        private async Task SendEvent(Object source, ElapsedEventArgs e)
        {
            if (articlesToSend.Count > 0)
            {
                await SendOneArticleAsync(this.articlesToSend.Dequeue());
            }

            if (articlesToSend.Count == 0)
            {
                this.timer.Stop();
            }
        }

        public async Task SendOneArticleAsync(Article article)
        {
            foreach (string url in webhookUrls)
            {
                await _webhooks.SendWebhook(url, article);
            }
            _memory.SaveUrl(article.url);
            _memory.FlushWriter();
        }

        private List<Article> FilterOutSentArticles(List<Article> articles)
        {
            List<Article> newArticles = new List<Article>();
            foreach (Article article in articles)
            {
                if (_memory.IsInArchive(article.url) == false)
                {
                    newArticles.Add(article);
                }
            }
            return newArticles;
        }

        private List<Article> SortArticlesByDate(List<Article> articles)
        {
            if (articles.Count >= 2)
            {
                ArticleDateComparer adc = new ArticleDateComparer();
                articles.Sort(adc);
            }
            return articles;
        }

    }
}
