using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherData2.Models
{
    internal class MonthlyStatistics
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public double AvgOutdoorTemp { get; set; }
        public double AvgIndoorTemp { get; set; }
        public double MinOutdoorTemp { get; set; }
        public double MaxOutdoorTemp { get; set; }
        public double AvgOutdoorMoisture { get; set; }
        public double AvgIndoorMoisture { get; set; }
        public double AvgMoldRisk { get; set; }
        public double AvgOutdoorMoldRisk { get; set; }
        public int TotalRecords { get; set; }
    }
}
