using System;
using System.Collections.Generic;
using System.Text;
using DiscordNewsBot.Models;

namespace DiscordNewsBot.Models
{
    class ArticleDateComparer : IComparer<Article>
    {
        public int Compare(Article a, Article b)
        {
            DateTime aDate = DateTime.Parse(a.date);
            DateTime bDate = DateTime.Parse(b.date);

            if (aDate > bDate)
                return 1;
            else if (aDate < bDate)
                return -1;
            return 0;
            //return DateTime.Compare(aDate, bDate);
        }
    }
}
