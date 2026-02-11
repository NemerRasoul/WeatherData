using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherData2;


namespace WeatherData.Services
{
    internal class StatsService
    {
        private const string FilePath = "tempdata5-med fel.txt";

        public static void ShowStatistics()
        {
            bool backToMenu = false;

            while (!backToMenu)
            {
                Console.WriteLine("=== Statistik ===");
                Console.WriteLine("1. Månadsstatistik");
                Console.WriteLine("2. Meteorologiska årstider (Höst/Vinter)");
                Console.WriteLine("3. Sortera Dagar");
                Console.WriteLine("4. "); // Exportera till textfil
                Console.WriteLine("5. Tillbaka");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowMonthlyStatistics();
                        break;
                    case "2":
                        ShowMeteorologicalSeasons();
                        break;
                    case "3":
                        ShowSortingMenu();
                        break;
                    case "4":
                        //
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
            Console.WriteLine("\n=== Beräknar månadsstatistik... ===\n");
            var monthlyStats = CalculateMonthlyStats(FilePath);

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

        public static List<MonthlyStatistics> CalculateMonthlyStats(string filePath)
        {
            var allData = WeatherDataReader.GetAllWeatherData(filePath);
            var monthlyStats = new List<MonthlyStatistics>();

            var groupedByMonth = allData
                .GroupBy(d => new { d.DateTime.Year, d.DateTime.Month })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month);

            foreach (var group in groupedByMonth)
            {
                var stats = new MonthlyStatistics
                {
                    Year = group.Key.Year,
                    Month = group.Key.Month,
                    AvgOutdoorTemp = group.Average(d => d.OutdoorTemp),
                    AvgIndoorTemp = group.Average(d => d.IndoorTemp),
                    MinOutdoorTemp = group.Min(d => d.OutdoorTemp),
                    MaxOutdoorTemp = group.Max(d => d.OutdoorTemp),
                    AvgOutdoorMoisture = group.Average(d => d.OutdoorMoisture),
                    AvgIndoorMoisture = group.Average(d => d.IndoorMoisture),
                    AvgMoldRisk = group.Average(d => d.MoldRisk),
                    AvgOutdoorMoldRisk = group.Average(d => d.OutdoorMoldRisk),
                    TotalRecords = group.Count()
                };

                monthlyStats.Add(stats);
            }

            return monthlyStats;
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

            var dailyStats = CalculateDailyStats(FilePath);
            List<DailyStatistics> sortedStats = null;

            switch (sortType)
            {
                case "1":
                    sortedStats = SortByTemperature(dailyStats, isOutdoor, ascending);
                    break;
                case "2":
                    sortedStats = SortByMoisture(dailyStats, isOutdoor, ascending);
                    break;
                case "3":
                    sortedStats = SortByMoldRisk(dailyStats, isOutdoor, ascending);
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

        private static List<DailyStatistics> SortByTemperature(List<DailyStatistics> stats, bool outdoor, bool ascending)
        {
            if (outdoor)
            {
                return ascending
                    ? stats.OrderBy(s => s.AvgOutdoorTemp).ToList()
                    : stats.OrderByDescending(s => s.AvgOutdoorTemp).ToList();
            }
            else
            {
                return ascending
                    ? stats.OrderBy(s => s.AvgIndoorTemp).ToList()
                    : stats.OrderByDescending(s => s.AvgIndoorTemp).ToList();
            }
        }

        private static List<DailyStatistics> SortByMoisture(List<DailyStatistics> stats, bool outdoor, bool ascending)
        {
            if (outdoor)
            {
                return ascending
                    ? stats.OrderBy(s => s.AvgOutdoorMoisture).ToList()
                    : stats.OrderByDescending(s => s.AvgOutdoorMoisture).ToList();
            }
            else
            {
                return ascending
                    ? stats.OrderBy(s => s.AvgIndoorMoisture).ToList()
                    : stats.OrderByDescending(s => s.AvgIndoorMoisture).ToList();
            }
        }

        private static List<DailyStatistics> SortByMoldRisk(List<DailyStatistics> stats, bool outdoor, bool ascending)
        {
            if (outdoor)
            {
                return ascending
                    ? stats.OrderBy(s => s.AvgOutdoorMoldRisk).ToList()
                    : stats.OrderByDescending(s => s.AvgOutdoorMoldRisk).ToList();
            }
            else
            {
                return ascending
                    ? stats.OrderBy(s => s.AvgIndoorMoldRisk).ToList()
                    : stats.OrderByDescending(s => s.AvgIndoorMoldRisk).ToList();
            }
        }
        internal static List<DailyStatistics> CalculateDailyStats(string filePath) 
        {
            var allData = WeatherDataReader.GetAllWeatherData(filePath);
            var dailyStats = new List<DailyStatistics>();

            var groupedByDay = allData
                .GroupBy(d => d.DateTime.Date)
                .OrderBy(g => g.Key);

            foreach (var g in groupedByDay)
            {
                var stats = new DailyStatistics
                {
                    Date = g.Key,
                    AvgOutdoorTemp = g.Average(d => d.OutdoorTemp),
                    AvgOutdoorMoisture = g.Average(d => d.OutdoorMoisture),
                    AvgOutdoorMoldRisk = g.Average(d => d.OutdoorMoldRisk),

                    AvgIndoorTemp = g.Average(d => d.IndoorTemp),
                    AvgIndoorMoisture = g.Average(d => d.IndoorMoisture),
                    AvgIndoorMoldRisk = g.Average(d => d.MoldRisk),
                    TotalRecords = g.Count()
                };

                dailyStats.Add(stats);
            }
            

            return dailyStats;
        }

        private static void ShowMeteorologicalSeasons()
        {
            Console.WriteLine("\n=== Meteorologiska årstider ===\n");
            var seasons = FindMeteorologicalSeasons(FilePath);
            DisplaySeasons(seasons);

           

            Console.WriteLine("Tryck på valfri tangent för att fortsätta...");
            Console.ReadKey();
        }

        public static List<MeteorologicalSeason> FindMeteorologicalSeasons(string filePath)
        {
            var allData = WeatherDataReader.GetAllWeatherData(filePath);
            var seasons = new List<MeteorologicalSeason>();

            // Gruppera per dag och beräkna dygnsmedeltemperatur (utomhus)
            var dailyAvgs = allData
                .GroupBy(d => d.DateTime.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    AvgTemp = g.Average(d => d.OutdoorTemp)
                })
                .OrderBy(d => d.Date)
                .ToList();

            // Hitta höstar
            for (int i = 0; i < dailyAvgs.Count - 4; i++)
            {
                var currentDay = dailyAvgs[i];

                // Kontrollera datumsregler för höst
                // Tidigast 1 augusti, senast 14 februari
                int month = currentDay.Date.Month;
                int day = currentDay.Date.Day;

                // Kolla om datumet är inom tillåtet intervall för höst
                bool isValidAutumnDate = false;
                if (month >= 8) // Augusti till december
                {
                    isValidAutumnDate = true;
                }
                else if (month <= 2) // Januari till februari
                {
                    if (month == 1 || (month == 2 && day <= 14))
                    {
                        isValidAutumnDate = true;
                    }
                }

                if (!isValidAutumnDate)
                    continue;

                var next5Days = dailyAvgs.Skip(i).Take(5).ToList();

                // Meteorologisk höst: dygnsmedeltemp < 10.0°C i 5 dygn i följd
                if (next5Days.Count == 5 && next5Days.All(d => d.AvgTemp < 10.0))
                {
                    // Kontrollera att vi inte redan har registrerat denna höst
                    bool alreadyRegistered = seasons.Any(s =>
                        s.Season == "Höst" &&
                        s.StartDate.Year == currentDay.Date.Year);

                    if (!alreadyRegistered)
                    {
                        var avg5Days = next5Days.Average(d => d.AvgTemp);
                        seasons.Add(new MeteorologicalSeason
                        {
                            Season = "Höst",
                            StartDate = currentDay.Date,
                            AvgTemp = avg5Days
                        });
                    }
                }
            }

            // Hitta vintrar
            for (int i = 0; i < dailyAvgs.Count - 4; i++)
            {
                var currentDay = dailyAvgs[i];
                var next5Days = dailyAvgs.Skip(i).Take(5).ToList();

                int month = currentDay.Date.Month;
                int day = currentDay.Date.Day;

                bool isValidWinterDate = false;
                if (month >= 11 && day >= 15) // Vinter kan endast börja från 15 november
                {
                    isValidWinterDate = true;
                }
                

                if (!isValidWinterDate)
                    continue;

                // Meteorologisk vinter: dygnsmedeltemp <= 5 grader i 5 dygn i följd
                if (next5Days.Count == 5 && next5Days.All(d => d.AvgTemp <= 5.0))
                {
                    // Kontrollera att vi inte redan har registrerat denna vinter
                    // En vinter kan sträcka sig över årsskiftet så vi kollar på både året för startdatum
                    bool alreadyRegistered = seasons.Any(s =>
                        s.Season == "Mild Vinter" &&
                        Math.Abs((s.StartDate - currentDay.Date).TotalDays) < 30);

                    if (!alreadyRegistered)
                    {
                        var avg5Days = next5Days.Average(d => d.AvgTemp);
                        seasons.Add(new MeteorologicalSeason
                        {
                            Season = "Mild Vinter",
                            StartDate = currentDay.Date,
                            AvgTemp = avg5Days
                        });
                    }
                }
            }

            // Beräkna slutdatum och genomsnittstemp för varje säsong
            foreach (var season in seasons.OrderBy(s => s.StartDate))
            {
                // Hitta slutdatum: första dagen då säsongen tar slut
                // För höst: når vintertemp eller går tillbaka över 10°C i 5 dagar
                // För vinter: temperatur går över 0°C i 5 dagar

                var seasonStartIndex = dailyAvgs.FindIndex(d => d.Date == season.StartDate);

                if (season.Season == "Höst")
                {
                    // Höst slutar vid vinterstart eller när temp går över 10°C i 5 dagar
                    for (int i = seasonStartIndex + 5; i < dailyAvgs.Count - 4; i++)
                    {
                        var check5Days = dailyAvgs.Skip(i).Take(5).ToList();

                        // Höst slutar om vinter börjar (≤0°C i 5 dagar)
                        if (check5Days.All(d => d.AvgTemp <= 5.0))
                        {
                            season.EndDate = dailyAvgs[i].Date.AddDays(-1);
                            break;
                        }
                        // Eller om temp går över 10°C i 5 dagar
                        else if (check5Days.All(d => d.AvgTemp >= 10.0))
                        {
                            season.EndDate = dailyAvgs[i].Date.AddDays(-1);
                            break;
                        }
                    }
                }
                else if (season.Season == "Mild Vinter")
                {
                    // Vinter slutar när temp går över 0°C i 5 dagar
                    for (int i = seasonStartIndex + 5; i < dailyAvgs.Count - 4; i++)
                    {
                        var check5Days = dailyAvgs.Skip(i).Take(5).ToList();

                        if (check5Days.All(d => d.AvgTemp > 4.0))
                        {
                            season.EndDate = dailyAvgs[i].Date.AddDays(-1);
                            break;
                        }
                    }
                }

                // Beräkna exakt medeltemp för perioden och antal dagar
                if (season.EndDate.HasValue)
                {
                    var seasonData = allData
                        .Where(d => d.DateTime.Date >= season.StartDate && d.DateTime.Date <= season.EndDate.Value)
                        .ToList();

                    if (seasonData.Any())
                    {
                        season.AvgTemp = seasonData.Average(d => d.OutdoorTemp);
                        season.DayCount = (season.EndDate.Value - season.StartDate).Days + 1;
                    }
                }
                else
                {
                    // Säsongen pågår fortfarande eller data tar slut
                    var seasonData = allData
                        .Where(d => d.DateTime.Date >= season.StartDate)
                        .ToList();

                    if (seasonData.Any())
                    {
                        season.AvgTemp = seasonData.Average(d => d.OutdoorTemp);
                        var lastDate = dailyAvgs.Last().Date;
                        season.DayCount = (lastDate - season.StartDate).Days + 1;
                    }
                }
            }

            return seasons.OrderBy(s => s.StartDate).ToList();
        }

        public static void DisplaySeasons(List<MeteorologicalSeason> seasons)
        {
            if (seasons.Count == 0)
            {
                Console.WriteLine("Inga meteorologiska årstider hittades i datan.");
                return;
            }

            var autumns = seasons.Where(s => s.Season == "Höst").ToList();
            var winters = seasons.Where(s => s.Season == "Mild Vinter").ToList();

            Console.WriteLine("=== METEOROLOGISKA HÖSTAR ===");
            if (autumns.Any())
            {
                foreach (var autumn in autumns)
                {
                    Console.WriteLine($"\nHöst {autumn.StartDate.Year}:");
                    Console.WriteLine($"  Ankomstdatum: {autumn.StartDate:yyyy-MM-dd} (första dygnet av 5 med temp < 10°C)");
                    if (autumn.EndDate.HasValue)
                    {
                        Console.WriteLine($"  Slutdatum: {autumn.EndDate.Value:yyyy-MM-dd}");
                        Console.WriteLine($"  Längd: {autumn.DayCount} dagar");
                    }
                    else
                    {
                        Console.WriteLine($"  Slutdatum: Pågående eller data saknas");
                        Console.WriteLine($"  Längd: {autumn.DayCount} dagar (hittills)");
                    }
                    Console.WriteLine($"  Medeltemperatur: {autumn.AvgTemp:F1}°C");
                }
            }
            else
            {
                Console.WriteLine("Inga meteorologiska höstar hittades i datan.");
            }

            Console.WriteLine("\n=== METEOROLOGISKA VINTRAR ===");
            if (winters.Any())
            {
                foreach (var winter in winters)
                {
                    Console.WriteLine($"\nMild Vinter {winter.StartDate.Year}/{winter.StartDate.Year + 1}:");
                    Console.WriteLine($"  Ankomstdatum: {winter.StartDate:yyyy-MM-dd} (första dygnet av 5 med temp ≤ 0°C)");
                    if (winter.EndDate.HasValue)
                    {
                        Console.WriteLine($"  Slutdatum: {winter.EndDate.Value:yyyy-MM-dd}");
                        Console.WriteLine($"  Längd: {winter.DayCount} dagar");
                    }
                    else
                    {
                        Console.WriteLine($"  Slutdatum: Pågående eller data saknas");
                        Console.WriteLine($"  Längd: {winter.DayCount} dagar (hittills)");
                    }
                    Console.WriteLine($"  Medeltemperatur: {winter.AvgTemp:F1}°C");
                }
            }
            else
            {
                Console.WriteLine("Inga meteorologiska vintrar hittades i datan.");
            }

            Console.WriteLine("\n=== REGLER ===");
            Console.WriteLine("Meteorologisk höst: Dygnsmedeltemp < 10.0°C i 5 dygn i följd");
            Console.WriteLine("  - Tidigast: 1 augusti");
            Console.WriteLine("  - Senast: 14 februari");
            Console.WriteLine("Meteorologisk mild vinter: Dygnsmedeltemp ≤ 5.0°C i 5 dygn i följd");
        }
    }
}