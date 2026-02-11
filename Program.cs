using System.Globalization;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WeatherData.Services;

namespace WeatherData
{
    internal class Program
    {
        public static void Main()
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                var choice = StartPageService.ShowStartPage();

                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        Console.WriteLine("=== Sök efter datum ===");
                        SearchService.SearchByDate();
                        break;

                    case "2":
                        Console.Clear();
                        StatsService.ShowStatistics();
                        break;

                    case "3":
                        Console.WriteLine("Avslutar programmet...");
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Ogiltigt val! Tryck på valfri tangent för att fortsätta...");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}
