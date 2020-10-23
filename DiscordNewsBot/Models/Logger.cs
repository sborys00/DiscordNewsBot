using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordNewsBot.Models
{
    class Logger
    {
        public static async Task LogAsync(string message)
        {
            string timestamp = $"[{DateTime.UtcNow.ToString()}] ";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(timestamp);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(message + '\n');
        }        
        public static void Log(string message)
        {
            string timestamp = $"[{DateTime.UtcNow.ToString()}] ";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(timestamp);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(message + '\n');
        }

    }
}
