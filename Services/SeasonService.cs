using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherData2.Data;
using WeatherData2.Models;

namespace WeatherData2.Services
{
    internal class SeasonService
    {
        internal static void ShowMeteorologicalSeasons()
        {
            Console.Clear();
            Console.WriteLine("\n=== Meteorologiska årstider ===\n");
            var seasons = FindMeteorologicalSeasons(AppConfig.FilePath);
            DisplaySeasons(seasons);



            Console.WriteLine("Tryck på valfri tangent för att fortsätta...");
            Console.ReadKey();
        }

        public static List<MeteorologicalSeason> FindMeteorologicalSeasons(string filePath)
        {
            var allData = WeatherDataReader.GetAllWeatherData(filePath);
            var seasons = new List<MeteorologicalSeason>();

            //Grupperar data per dag, beräknar medeltemp.
            var dailyAvgs = allData
                .GroupBy(d => d.DateTime.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    AvgTemp = g.Average(d => d.OutdoorTemp)
                })
                .OrderBy(d => d.Date)
                .ToList();

            //Hittar höst
            for (int i = 0; i < dailyAvgs.Count - 4; i++)
            {
                var currentDay = dailyAvgs[i];

                //Höst börjar tidigast 1 agusti och senast 14e februari.
                int month = currentDay.Date.Month;
                int day = currentDay.Date.Day;

                //Kollar om det är augusti.
                bool isValidAutumnDate = false;
                if (month >= 8)
                {
                    isValidAutumnDate = true;
                }//Kollar om det är januari eller februari
                else if (month <= 2)
                {
                    if (month == 1 || month == 2 && day <= 14)
                    {
                        isValidAutumnDate = true;
                    }
                }

                if (!isValidAutumnDate)
                    continue;

                var next5Days = dailyAvgs.Skip(i).Take(5).ToList();

                //Höst börjar vid mindre än 10 grader i 5 dagar.
                if (next5Days.Count == 5 && next5Days.All(d => d.AvgTemp < 10.0))
                {
                    //Kollar om det redan finns höst.
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

            int longestStreak = 0;
            DateTime? longestStart = null;

            int currentStreak = 0;
            DateTime? currentStart = null;


            //Kollar vinter.
            for (int i = 0; i < dailyAvgs.Count; i++)
            {
                if (dailyAvgs[i].AvgTemp <= 0.0)
                {
                    if (currentStreak == 0)
                    {
                        currentStart = dailyAvgs[i].Date;

                        currentStreak++;

                        if (currentStreak > longestStreak)
                        {
                            longestStreak = currentStreak;
                            longestStart = currentStart;
                        }
                    }
                    else
                    {
                        currentStreak = 0;
                        currentStart = null;
                    }
                }
            }
            //Om det finns en streak på 5 dagar i rad får vi vinter.
            if (longestStart.HasValue)
            {
                seasons.Add(new MeteorologicalSeason
                {
                    Season = longestStreak >= 5 ? "Vinter" : "Mild Vinter",
                    StartDate = longestStart.Value,
                    EndDate = longestStart.Value.AddDays(longestStreak - 1),
                    DayCount = longestStreak
                });

            }
            //Omd det inte blir vinter visar vi den närmaste streaken
            else if (longestStreak > 0 && longestStart.HasValue)
            {
                seasons.Add(new MeteorologicalSeason
                {
                    Season = "Mild Vinter",
                    StartDate = longestStart.Value,
                    EndDate = longestStart.Value.AddDays(longestStreak - 1),
                    DayCount = longestStreak
                });
            }
            // Beräkna slutdatum och genomsnittstemp för varje säsong. Hittar slutdatum baserat på temperatur och vilken säsong det är.
            foreach (var season in seasons.OrderBy(s => s.StartDate))
            {
                var seasonStartIndex = dailyAvgs.FindIndex(d => d.Date == season.StartDate);

                if (season.Season == "Höst")
                {
                    //Kollar temperaturen och agerar därefter
                    for (int i = seasonStartIndex + 5; i < dailyAvgs.Count - 4; i++)
                    {
                        var check5Days = dailyAvgs.Skip(i).Take(5).ToList();

                        //Om vinternbörjar slutar hösten
                        if (check5Days.All(d => d.AvgTemp <= 0.0))
                        {
                            season.EndDate = dailyAvgs[i].Date.AddDays(-1);
                            break;
                        }
                        //Eller om temperaturen överstiger 10 grader i 5 dagar så avslutas hösten
                        else if (check5Days.All(d => d.AvgTemp >= 10.0))
                        {
                            season.EndDate = dailyAvgs[i].Date.AddDays(-1);
                            break;
                        }
                    }
                }
                else if (season.Season == "Vinter")
                {
                    //Kollar om temperaturen går över 4 grader i 5 dagar, avslutar vintern.
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

                //Beräkna exakt medeltemp för perioden och antal dagar
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
                    //Säsongen pågår fortfarande eller data tar slut
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
            var winters = seasons.Where(s => s.Season == "Vinter" || s.Season == "Mild Vinter").ToList();

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
                    Console.WriteLine($"\nVinter {winter.StartDate.Year}/{winter.StartDate.Year + 1}:");
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
