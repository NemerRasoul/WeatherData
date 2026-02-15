using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherData2.Models;

namespace WeatherData2.Services
{
    internal class SortingService
    {
        internal static List<DailyStatistics> SortByTemperature(List<DailyStatistics> stats, bool outdoor, bool ascending)
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

        internal static List<DailyStatistics> SortByMoisture(List<DailyStatistics> stats, bool outdoor, bool ascending)
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

        internal static List<DailyStatistics> SortByMoldRisk(List<DailyStatistics> stats, bool outdoor, bool ascending)
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
    }
}
