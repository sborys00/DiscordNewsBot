namespace DiscordNewsBot.Models
{
    interface IEmbed
    {
        Author author { get; set; }
        int color { get; set; }
        string description { get; set; }
        Footer footer { get; set; }
        Thumbnail thumbnail { get; set; }
        string title { get; set; }
        string url { get; set; }
    }
}