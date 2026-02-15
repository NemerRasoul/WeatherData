using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherData2.Data;
using WeatherData2.Models;

namespace WeatherData2.Services
{
    internal class StatsMenuService
    {
        public static void ShowStatistics()
        {
            bool backToMenu = false;

            while (!backToMenu)
            {
                Console.Clear();
                Console.WriteLine("=== Statistik ===");
                Console.WriteLine("1. Månadsstatistik");
                Console.WriteLine("2. Meteorologiska årstider (Höst/Vinter)");
                Console.WriteLine("3. Sortera Dagar");
                Console.WriteLine("4. Generera rapport");
                Console.WriteLine("5. Tillbaka");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowMonthlyStatistics();
                        break;
                    case "2":
                        SeasonService.ShowMeteorologicalSeasons();
                        break;
                    case "3":
                        ShowSortingMenu();
                        break;
                    case "4":
                        SaveToFileService.SaveWeatherReport(SeasonService.FindMeteorologicalSeasons(AppConfig.FilePath));
                        break;
                    case "5":
                        backToMenu = true;
                        break;
                    default:
                        Console.WriteLine("Ogiltigt val!");
                        break;
                }
            }
        }

        private static void ShowMonthlyStatistics()
        {
            Console.Clear();
            Console.WriteLine("\n=== Beräknar månadsstatistik... ===\n");
            var monthlyStats = StatsService.CalculateMonthlyStats(AppConfig.FilePath);

            foreach (var stat in monthlyStats)
            {

                Console.WriteLine($"=== {stat.Year}-{stat.Month:00} ===");
                Console.WriteLine($"Utomhus temp: Medel {stat.AvgOutdoorTemp:F1}°C, Min {stat.MinOutdoorTemp:F1}°C, Max {stat.MaxOutdoorTemp:F1}°C");
                Console.WriteLine($"Inomhus temp: Medel {stat.AvgIndoorTemp:F1}°C");
                Console.WriteLine($"Utomhus fuktighet: Medel {stat.AvgOutdoorMoisture:F0}%");
                Console.WriteLine($"Inomhus fuktighet: Medel {stat.AvgIndoorMoisture:F0}%");
                Console.WriteLine($"Mögelrisk inne: {stat.AvgMoldRisk:F1}%");
                Console.WriteLine($"Mögelrisk ute: {stat.AvgOutdoorMoldRisk:F1}%");
                Console.WriteLine($"Antal mätningar: {stat.TotalRecords}");
                Console.WriteLine();
            }

            Console.WriteLine("Tryck på valfri tangent för att fortsätta...");
            Console.ReadKey();
        }

        private static void ShowSortingMenu()
        {
            Console.WriteLine("=== SORTERA DAGAR ===");
            Console.WriteLine("Vad vill du sortera efter?");
            Console.WriteLine("1. Temperatur");
            Console.WriteLine("2. Fuktighet");
            Console.WriteLine("3. Mögelrisk");
            Console.Write("\nVälj (1-3): ");

            var sortType = Console.ReadLine();

            Console.Write("\nInomhus eller Utomhus? (I/U): ");
            var location = Console.ReadLine()?.ToUpper();

            Console.Write("Stigande eller Fallande? (S/F): ");
            var order = Console.ReadLine()?.ToUpper();

            Console.Write("Hur många resultat? (tryck Enter för alla): ");
            var countInput = Console.ReadLine();
            int? topCount = string.IsNullOrWhiteSpace(countInput) ? null : int.Parse(countInput);

            bool isOutdoor = location == "U";
            bool ascending = order == "S";

            var dailyStats = StatsService.CalculateDailyStats(AppConfig.FilePath);
            List<DailyStatistics> sortedStats = null;

            switch (sortType)
            {
                case "1":
                    sortedStats = SortingService.SortByTemperature(dailyStats, isOutdoor, ascending);
                    break;
                case "2":
                    sortedStats = SortingService.SortByMoisture(dailyStats, isOutdoor, ascending);
                    break;
                case "3":
                    sortedStats = SortingService.SortByMoldRisk(dailyStats, isOutdoor, ascending);
                    break;
                default:
                    Console.WriteLine("Ogiltigt val!");
                    Console.ReadKey();
                    return;
            }

            if (topCount.HasValue && topCount.Value < sortedStats.Count)
            {
                sortedStats = sortedStats.Take(topCount.Value).ToList();
            }

            DisplaySortedResults(sortedStats, sortType, isOutdoor, ascending);

            Console.WriteLine("\nTryck på valfri tangent för att fortsätta...");
            Console.ReadKey();
        }

        private static void DisplaySortedResults(List<DailyStatistics> stats, string sortType, bool isOutdoor, bool ascending)
        {
            Console.WriteLine("\n=== RESULTAT ===");

            string sortTypeName = sortType switch
            {
                "1" => "Temperatur",
                "2" => "Fuktighet",
                "3" => "Mögelrisk",
                _ => "?"
            };

            string locationName = isOutdoor ? "Utomhus" : "Inomhus";
            string orderName = ascending ? "Lägsta till högsta" : "Högsta till lägsta";

            Console.WriteLine($"Sorterat efter: {sortTypeName} ({locationName}) - {orderName}");
            Console.WriteLine($"Antal dagar: {stats.Count}\n");
            //Rankar resultat
            int rank = 1;
            foreach (var stat in stats)
            {
                string value = "";

                switch (sortType)
                {
                    case "1":
                        double temp = isOutdoor ? stat.AvgOutdoorTemp : stat.AvgIndoorTemp;
                        value = $"{temp:F1}°C";
                        break;
                    case "2":
                        double moisture = isOutdoor ? stat.AvgOutdoorMoisture : stat.AvgIndoorMoisture;
                        value = $"{moisture:F0}%";
                        break;
                    case "3":
                        double moldRisk = isOutdoor ? stat.AvgOutdoorMoldRisk : stat.AvgIndoorMoldRisk;
                        value = $"{moldRisk:F1}%";
                        break;
                }

                Console.WriteLine($"{rank}. {stat.Date:yyyy-MM-dd} - {value}");
                rank++;
            }
        }


    }
}
