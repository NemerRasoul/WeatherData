using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherData
{
    internal class MeteorologicalSeason
    {
        public string Season { get; set; } // "Höst", "Vinter", "Vår", "Sommar"
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double AvgTemp { get; set; }
        public int DayCount { get; set; }
    }
}
