using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherData2.Services
{
    internal class MoldCalc
    {
        public static double MoldCalculator(double temperature, double humidity)
        {
           
            // För torrt - under 78% humidity
            if (humidity < 78)
            {
                // Kalkylera risk 
                return Math.Min(10, humidity / 78.0 * 10);
            }

            
            double highRiskThreshold;

           // Bestäm gränsen för hög risk beroende på temperatur
           // ju varmare det är desto lägre luftfuktighet krävs för hög risk
           
            if (temperature <= 0)
            {
                highRiskThreshold = 95;
            }
            else if (temperature <= 10)
            {
                highRiskThreshold = 92;
            }
            else if (temperature <= 20)
            {
                highRiskThreshold = 89;
            }
            else if (temperature <= 30)
            {
                highRiskThreshold = 87;
            }
            else // temperatur > 30
            {
                highRiskThreshold = 86;
            }

            
            if (humidity >= highRiskThreshold)
            {
               
                double excessHumidity = humidity - highRiskThreshold;
                double rangeToMax = 100 - highRiskThreshold;
                double riskIncrease = excessHumidity / rangeToMax * 20; // 20% range (80-100%)
                return Math.Min(100, 80 + riskIncrease);
            }

            // Medium risk zon - mellan 78% och hög risk 
           
            double humidityRange = highRiskThreshold - 78;
            double humidityAbove78 = humidity - 78;
            double riskPercentage = 10 + humidityAbove78 / humidityRange * 70; //  10% to 80%

            return Math.Min(80, Math.Max(10, riskPercentage));
        }

        public static string GetMoldRiskCategory(double riskPercentage)
        {
            if (riskPercentage < 10)
                return "För torrt";

            else if (riskPercentage < 80)
                return "Mögel är möjligt";

            else
                return "Hög mögelrisk";
        }
    }
}
