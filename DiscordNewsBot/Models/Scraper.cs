using System;
using System.Collections.Generic;
using System.Net;
using HtmlAgilityPack;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection.PortableExecutable;

namespace DiscordNewsBot.Models
{
    class Scraper : IScraper
    {
        private readonly IConfiguration _config;
        private readonly NewsWebsite[] newsWebsites;

        public Scraper(IConfiguration config)
        {
            this._config = config;
            this.newsWebsites = _config.GetSection("NewsWebsites").Get<NewsWebsite[]>();
        }

        public async Task<List<Article>> GetAllArticlesAsync()
        {
            NewsWebsite[] websites = _config.GetSection("NewsWebsites").Get<NewsWebsite[]>();
            string[] urls = websites.Select(o => o.Url).ToArray();

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
            HtmlWeb web = new HtmlWeb();
            web.AutoDetectEncoding = false;
            web.OverrideEncoding = Encoding.UTF8;

            html = await web.LoadFromWebAsync(url);
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
                article.title = HtmlEntity.DeEntitize(nodes[i].SelectSingleNode(".//h3/a").InnerText.Trim());
                article.content = HtmlEntity.DeEntitize(nodes[i].GetDirectInnerText().Trim()) + "...";
                article.thumbnail = nodes[i].SelectSingleNode(".//img").Attributes["src"].Value;
                article.date = nodes[i].SelectSingleNode(".//span").InnerText.Replace(" ", "");
                articles.Add(article);

                string rgx = @"(\d.../).*";
                string directory = article.url;
                directory = Regex.Replace(directory, rgx, "");

                foreach (NewsWebsite website in newsWebsites)
                {
                    if (website.NewsUrlDirectory == directory)
                    {
                        article.author = website.Name;
                        article.color = website.Color;
                    }
                }
            }
            return articles;
        }

    }
}
