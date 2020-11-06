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

        private static System.Timers.Timer timer;
        private static IScraper _scraper;
        private static IWebhookSender _webhookSender;

        static void Main(string[] args)
        {
            using (var services = ConfigureServices())
            {
                _scraper = services.GetRequiredService<IScraper>();
                _webhookSender = services.GetRequiredService<IWebhookSender>();
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
                .AddTransient<IScraper, Scraper>()
                .AddTransient<IWebhooks, Webhooks>()
                .AddSingleton<IWebhookSender, WebhookSender>()
                .AddSingleton<IMemory, Memory>()
				.BuildServiceProvider();
		}

        private static async void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            try
            {
                List<Article> articles = new List<Article>();
                await Logger.LogAsync("Looking for news...");
                articles = await _scraper.GetAllArticlesAsync();
                _webhookSender.EnqueueArticles(articles);
                timer.Start();
            }
            catch(Exception exception)
            {
                await Logger.LogAsync(exception.Message);
            }
        }

	}
}
