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
            string[] aDate = a.date.Split("-");
            string[] bDate = b.date.Split("-");

            //0-day 1-month 2-year
            int[] aDateInt = new int[3];
            int[] bDateInt = new int[3];
            
            for (int i = 0; i < 3; i++)
            {
                aDateInt[i] = Int32.Parse(aDate[i]);
                bDateInt[i] = Int32.Parse(bDate[i]);
            }

            for (int i = 2; i >= 0; i--)
            {
                if (aDateInt[i] > bDateInt[i])
                    return 1;
                else if (aDateInt[i] < bDateInt[i])
                    return -1;
            }

            return 0;
        }
    }
}
