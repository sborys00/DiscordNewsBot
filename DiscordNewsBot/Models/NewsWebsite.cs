namespace DiscordNewsBot.Models
{
    class NewsWebsite
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string NewsUrlDirectory { get; set; }
        public int Color { get; set; }
        public string ArticleSelector { get; set; }
        public string TitleSelector { get; set; }
        public string ThumbnailSelector { get; set; }
        public string UrlSelector { get; set; }
        public string ContentSelector { get; set; }
        public string DateSelector { get; set; }
    }
}
