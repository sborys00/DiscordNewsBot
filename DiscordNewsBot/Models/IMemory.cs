namespace DiscordNewsBot.Models
{
    public interface IMemory
    {
        public bool IsInArchive(string url);
        public void SaveUrl(string url);
        public void FlushWriter();
    }
}
