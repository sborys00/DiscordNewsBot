using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using DiscordNewsBot.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordNewsBot
{
    public class Program
    {
        //loop interval in ms
        private static int interval = 60000;
        private static string[] webhookUrls;

        private static Scraper scraper;
        private static Webhooks webhooks;
        private static System.Timers.Timer timer;

        private static WebhookSender webhookSender;

        static void Main(string[] args)
        {
            Config cfg = new Config("config.txt");
            using (var services = ConfigureServices())
            {
                scraper = services.GetRequiredService<Scraper>();
                webhooks = services.GetRequiredService<Webhooks>();
                try
                {
                    webhookUrls = cfg.LoadWebhookUrls();
                }
                catch (Exception e)
                {
                    Logger.Log(e.Message);
                    Logger.Log("Loading Webhook urls failed. Make sure \"config.txt\" exist and is not empty");
                    return;
                }
                webhookSender = new WebhookSender(webhooks, webhookUrls);
                Task.Run(() => Logger.LogAsync("Program starting..."));
            }
            
            timer = new System.Timers.Timer(interval);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = false;
            timer.Enabled = true;
            OnTimedEvent(null, null);

            //Keep the program running
            Task.Run(() => Task.Delay(Timeout.Infinite)).GetAwaiter().GetResult();
        }

		private static ServiceProvider ConfigureServices()
		{
			return new ServiceCollection()
                .AddSingleton<Scraper>()
                .AddSingleton<Webhooks>()
                .AddSingleton<WebhookSender>()
                .AddSingleton<IMemory, Memory>()
				.BuildServiceProvider();
		}

        private static async void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            try
            {
                List<Article> articles = new List<Article>();
                await Logger.LogAsync("Looking for news...");
                articles = await scraper.GetAllArticlesAsync();
                webhookSender.EnqueueArticles(articles);
                timer.Start();
            }
            catch(Exception exception)
            {
                await Logger.LogAsync(exception.Message);
            }
        }

	}
}
