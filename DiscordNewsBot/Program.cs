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
        private static int interval = 30000;

        private static Scraper scraper;
        private static Webhooks webhooks;
        private static System.Timers.Timer timer;

        private static Scheduler scheduler;

        static void Main(string[] args)
        {
            using (var services = ConfigureServices())
            {
                scraper = services.GetRequiredService<Scraper>();
                webhooks = services.GetRequiredService<Webhooks>();
                scheduler = new Scheduler(webhooks);
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
				.BuildServiceProvider();
		}

        private static async void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            List<Article> articles = new List<Article>();
            await Logger.LogAsync("Looking for news...");
            articles = await scraper.GetAllArticlesAsync();
            //webhooks.SendWebhook("https://discordapp.com/api/webhooks/688377806010449993/CfDoijYes_1G3wP9yei32A2Gbf0NP1i7zbnMQ8gSqdojXesK6BuLs1P5M5gK4D7V9BuG", articles[0]);
            //webhooks.SendWebhook("https://discordapp.com/api/webhooks/764523413858942977/1pynVlbb7shV5w04zET_Le-yXWf8-uRymuHAus86l06qhFd_K73Y6wlwrzEZ_zgaB9_J", articles[0]);
            await scheduler.SendArticlesAsync(articles, "https://discordapp.com/api/webhooks/688377806010449993/CfDoijYes_1G3wP9yei32A2Gbf0NP1i7zbnMQ8gSqdojXesK6BuLs1P5M5gK4D7V9BuG");
            timer.Start();
        }
	}
}
