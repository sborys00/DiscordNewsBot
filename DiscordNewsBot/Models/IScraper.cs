using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordNewsBot.Models
{
    interface IScraper
    {
        Task<List<Article>> GetAllArticlesAsync();
    }
}