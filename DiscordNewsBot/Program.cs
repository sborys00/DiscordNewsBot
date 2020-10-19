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
        private static readonly string[] webhookUrls = {"https://discordapp.com/api/webhooks/688377806010449993/CfDoijYes_1G3wP9yei32A2Gbf0NP1i7zbnMQ8gSqdojXesK6BuLs1P5M5gK4D7V9BuG", "https://discordapp.com/api/webhooks/767451587542777906/vVDrx-UptCCPyCry6TAcg3hGaFl2EQMfgK3hQ9rrSx-v3Pdtw31jWbJFuvZZrT_8a-9s"};

        private static Scraper scraper;
        private static Webhooks webhooks;
        private static System.Timers.Timer timer;

        private static WebhookSender webhookSender;

        static void Main(string[] args)
        {
            using (var services = ConfigureServices())
            {
                scraper = services.GetRequiredService<Scraper>();
                webhooks = services.GetRequiredService<Webhooks>();
                webhookSender = new WebhookSender(webhooks, services.GetRequiredService<Memory>(), webhookUrls);
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
                .AddSingleton<Memory>()
				.BuildServiceProvider();
		}

        private static async void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            List<Article> articles = new List<Article>();
            await Logger.LogAsync("Looking for news...");
            articles = await scraper.GetAllArticlesAsync();
            webhookSender.EnqueueArticles(articles);
            timer.Start();
        }
	}
}
