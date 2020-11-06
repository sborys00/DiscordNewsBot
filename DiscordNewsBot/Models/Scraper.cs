using System;
using System.Collections.Generic;
using System.Net;
using HtmlAgilityPack;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace DiscordNewsBot.Models
{
    class Scraper : IScraper
    {
        private readonly IConfiguration _config;

        public Scraper(IConfiguration config)
        {
            this._config = config;
        }

        public async Task<List<Article>> GetAllArticlesAsync()
        {
            string[] urls = _config.GetSection("NewsWebsiteUrls").Get<string[]>();

            List<Task<List<Article>>> tasks = new List<Task<List<Article>>>();
            foreach (var url in urls)
            {
                tasks.Add(GetArticlesAsync(url));
            }

            var results = await Task.WhenAll(tasks);

            //Merge all lists into one
            List<Article> articles = new List<Article>();
            foreach (var task in results)
            {
                articles.AddRange(task);
            }

            articles.Reverse();

            return articles;
        }

        private async Task<List<Article>> GetArticlesAsync(string url)
        {
            List<Article> articles;

            HtmlDocument html = new HtmlDocument();

            using (WebClient client = new WebClient())
            {
                string content = await client.DownloadStringTaskAsync(url);
                html.LoadHtml(content);
                if (html.Text.Length == 0)
                    throw new NullReferenceException(url + " is empty");
            }
            articles = ParseArticles(html);
            return articles;
        }

        private List<Article> ParseArticles(HtmlDocument html)
        {
            List<Article> articles = new List<Article>();
            var nodes = html.DocumentNode.SelectNodes("//article");
            for (int i = 0; i < nodes.Count; i++)
            {
                Article article = new Article();
                article.url = nodes[i].SelectSingleNode(".//a").Attributes["href"].Value;
                article.title = nodes[i].SelectSingleNode(".//h3/a").InnerText;
                article.content = nodes[i].GetDirectInnerText().Trim() + "...";
                article.thumbnail = nodes[i].SelectSingleNode(".//img").Attributes["src"].Value;
                article.date = nodes[i].SelectSingleNode(".//span").InnerText.Replace(" ", "");
                articles.Add(article);
            }
            return articles;
        }

    }
}
