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
        private static System.Timers.Timer timer;
        private static IScraper _scraper;
        private static IWebhookSender _webhookSender;
        private static IConfiguration _config;

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            //BuildConfig(builder);
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
            
            var host = Host.CreateDefaultBuilder().ConfigureAppConfiguration((hostingContext, config) =>
                {
                    BuildConfig(config);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<IScraper, Scraper>()
                    .AddTransient<IWebhooks, Webhooks>()

                    .AddSingleton<IWebhookSender, WebhookSender>()
                    .AddSingleton<IMemory, Memory>();
                })
                .UseSerilog()
                .Build();
            _scraper = host.Services.GetRequiredService<IScraper>();
            _webhookSender = host.Services.GetRequiredService<IWebhookSender>();
            _config = host.Services.GetRequiredService<IConfiguration>();

            timer = new System.Timers.Timer(_config.GetValue<int>("ScanningInterval"));
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = false;
            timer.Enabled = true;
            OnTimedEvent(null, null);

            //Keeps the program running
            Task.Run(() => Task.Delay(Timeout.Infinite)).GetAwaiter().GetResult();
        }

        private static async void OnTimedEvent(Object source, ElapsedEventArgs args)
        {
            try
            {
                timer.Stop();
                List<Article> articles = new List<Article>();
                Log.Logger.Information("Looking for news..");
                articles = await _scraper.GetAllArticlesAsync();
                _webhookSender.EnqueueArticles(articles);

                //keeps interval times updated to current config value
                timer.Interval = _config.GetValue<int>("ScanningInterval");

                timer.Start();
            }
            catch(Exception e)
            {
                Log.Logger.Error(e.Message);
            }
        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        }
	}
}
