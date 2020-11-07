using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;

namespace DiscordNewsBot.Models
{
    class WebhookSender : IWebhookSender
    {
        private readonly int sendInterval; //time in ms
        private readonly IWebhooks _webhooks;
        private readonly IMemory _memory;
        private readonly IConfiguration _config;
        private readonly string[] webhookUrls;
        Queue<Article> articlesToSend = new Queue<Article>();
        Timer timer;


        public WebhookSender(IMemory memory, IWebhooks webhooks, IConfiguration config)
        {
            this._webhooks = webhooks;
            this._memory = memory;
            this._config = config;
            this.sendInterval = _config.GetValue<int>("WebhookSendInterval");
            timer = new System.Timers.Timer(sendInterval);
            this.webhookUrls = _config.GetSection("WebhookUrls").Get<string[]>();

            if (this.webhookUrls == null)
            {
                throw new Exception("Loading webhook urls failed. Make sure WebhookUrls array in config exist and is not empty");
            }
            timer.AutoReset = true;
        }

        public void EnqueueArticles(List<Article> articles)
        {
            articles = FilterOutSentArticles(articles);
            articles = SortArticlesByDate(articles);
            Log.Logger.Information($"{articles.Count} new articles found");
            foreach (Article article in articles)
            {
                this.articlesToSend.Enqueue(article);
            }
            if(timer.Enabled == false)
            {
                timer.Start();
                timer.Elapsed += async (source, e) => await SendEvent(source, e);
            }
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
            Log.Logger.Information($"Sending {article.title}");
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
                if (_memory.IsInArchive(article.url) == false && articlesToSend.Any(a => a.url == article.url) == false)
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
