using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherData
{
    internal class MoldCalc
    {
        public static double MoldCalculator(double temperature, double humidity)
        {
            // Based on the Swedish mold risk chart
            // Returns 0-100 where 100 = highest mold risk

            // Too dry zone (För torrt) - below ~78% humidity
            if (humidity < 78)
            {
                // Calculate risk from 0% at 0% humidity to ~10% at 78% humidity
                return Math.Min(10, (humidity / 78.0) * 10);
            }

            // Calculate the high risk threshold based on temperature
            double highRiskThreshold;
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
            else // temperature > 30
            {
                highRiskThreshold = 86;
            }

            // If humidity is at or above high risk threshold
            if (humidity >= highRiskThreshold)
            {
                // Scale from 80% risk at threshold to 100% at 100% humidity
                double excessHumidity = humidity - highRiskThreshold;
                double rangeToMax = 100 - highRiskThreshold;
                double riskIncrease = (excessHumidity / rangeToMax) * 20; // 20% range (80-100%)
                return Math.Min(100, 80 + riskIncrease);
            }

            // Medium risk zone - between 78% and high risk threshold
            // Scale from 10% at 78% humidity to 80% at threshold
            double humidityRange = highRiskThreshold - 78;
            double humidityAbove78 = humidity - 78;
            double riskPercentage = 10 + (humidityAbove78 / humidityRange) * 70; // Scale from 10% to 80%

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
