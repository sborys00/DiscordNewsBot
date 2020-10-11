using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordNewsBot.Models
{
    class Embed
    {
        public string title { get; set; }
        public string description { get; set; }
        public string url { get; set; }

        public Author author { get; set; }
        public Thumbnail thumbnail { get; set; }

        public Embed (Article article)
        {
            this.title = article.title;
            this.description = article.content;
            this.url = article.url;

            this.thumbnail = new Thumbnail(article.thumbnail);
        }
    }

    class Author
    {
        public string name { get; set; }

        public Author(string name)
        { 
            this.name = name;
        }
    }

    class Footer
    {
        public string text { get; set; }

        public Footer(string text)
        {
            this.text = text;
        }
    }

    class Thumbnail
    {
        public string url { get; set; }

        public Thumbnail(string url)
        {
            this.url = url;
        }
    }
}
