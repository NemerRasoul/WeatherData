using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherData2.Data;
using WeatherData2.Models;


namespace WeatherData2.Services
{
    internal class StatsService
    {
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
    }
}