using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherData2
{
    internal class DailyStatistics
    {
        public DateTime Date { get; set; }
        
        //Utomhus
        public double AvgOutdoorTemp { get; set; }
        public double AvgOutdoorMoisture { get; set; }
        public double AvgOutdoorMoldRisk { get; set; }

        //Inomhus
        public double AvgIndoorTemp { get; set; }
        public double AvgIndoorMoisture { get; set; }
        public double AvgIndoorMoldRisk { get; set; }

        public int TotalRecords { get; set; }
    }
}
