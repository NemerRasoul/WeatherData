using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherData2.Models
{
    internal class WeatherData
    {
        public DateTime DateTime { get; set; }
        public double IndoorTemp { get; set; }
        public double OutdoorTemp { get; set; }
        public double IndoorMoisture { get; set; }
        public double OutdoorMoisture { get; set; }
        public double MoldRisk { get; set; }
        public double OutdoorMoldRisk { get; set; }
    }
}
