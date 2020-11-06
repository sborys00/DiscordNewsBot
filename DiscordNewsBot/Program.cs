using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using DiscordNewsBot.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DiscordNewsBot
{
    public class Program
    {
        //loop interval in ms
        private static int interval = 60000;
        private static System.Timers.Timer timer;
        private static IScraper _scraper;
        private static IWebhookSender _webhookSender;

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<IWebhookSender, WebhookSender>()
                    .AddTransient<IScraper, Scraper>()
                    .AddSingleton<IMemory, Memory>()
                    .AddSingleton<IWebhookSender, WebhookSender>();
                })
                .UseSerilog()
                .Build();

            timer = new System.Timers.Timer(interval);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = false;
            timer.Enabled = true;
            OnTimedEvent(null, null);

            //Keeps the program running
            Task.Run(() => Task.Delay(Timeout.Infinite)).GetAwaiter().GetResult();
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

        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables();
        }
	}
}
