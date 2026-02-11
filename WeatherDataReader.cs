using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WeatherData
{
    internal class WeatherDataReader
    {
        public static List<WeatherData> GetDayWeatherData(string filePath, DateTime targetDate)
        {
            List<WeatherData> dayData = new List<WeatherData>();

            // Pattern: 2016-12-23 03:22:09,Ute,6.5,73
            string pattern = @"(\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2}),(Ute|Inne),([0-9.]+),(\d+)";

            double? outdoorTemp = null;
            double? indoorTemp = null;
            int? outdoorMoisture = null;
            int? indoorMoisture = null;
            DateTime? recordDateTime = null;

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Match match = Regex.Match(line, pattern);

                    if (match.Success)
                    {
                        try
                        {
                            DateTime lineDateTime = DateTime.ParseExact(
                                match.Groups[1].Value,
                                "yyyy-MM-dd HH:mm:ss",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.None
                            );

                            // Skip May 2016 and January 2017
                            if ((lineDateTime.Year == 2016 && lineDateTime.Month == 5) ||
                                (lineDateTime.Year == 2017 && lineDateTime.Month == 1))
                            {
                                continue;
                            }

                            // Check if this is the target date
                            if (lineDateTime.Date != targetDate.Date)
                            {
                                continue;
                            }

                            string location = match.Groups[2].Value;
                            double temperature = double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
                            int moisture = int.Parse(match.Groups[4].Value);

                            if (location == "Ute")
                            {
                                outdoorTemp = temperature;
                                outdoorMoisture = moisture;
                                recordDateTime = lineDateTime;
                            }
                            else if (location == "Inne")
                            {
                                indoorTemp = temperature;
                                indoorMoisture = moisture;
                                if (recordDateTime == null)
                                    recordDateTime = lineDateTime;
                            }

                            // If we have both readings, save the data
                            if (outdoorTemp.HasValue && indoorTemp.HasValue &&
                                outdoorMoisture.HasValue && indoorMoisture.HasValue)
                            {
                                double indoorMoldRisk = MoldCalc.MoldCalculator(indoorTemp.Value, indoorMoisture.Value);
                                double outdoorMoldRisk = MoldCalc.MoldCalculator(outdoorTemp.Value, outdoorMoisture.Value);

                                dayData.Add(new WeatherData
                                {
                                    DateTime = recordDateTime.Value,
                                    OutdoorTemp = outdoorTemp.Value,
                                    OutdoorMoisture = outdoorMoisture.Value,
                                    IndoorTemp = indoorTemp.Value,
                                    IndoorMoisture = indoorMoisture.Value,
                                    MoldRisk = indoorMoldRisk,
                                    OutdoorMoldRisk = outdoorMoldRisk
                                });

                                // Reset for next reading
                                outdoorTemp = null;
                                indoorTemp = null;
                                outdoorMoisture = null;
                                indoorMoisture = null;
                                recordDateTime = null;
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
            }

            return dayData;
        }

        public static List<WeatherData> GetAllWeatherData(string filePath)
        {
            List<WeatherData> allData = new List<WeatherData>();

            // Pattern: 2016-12-23 03:22:09,Ute,6.5,73
            string pattern = @"(\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2}),(Ute|Inne),([0-9.]+),(\d+)";

            double? outdoorTemp = null;
            double? indoorTemp = null;
            int? outdoorMoisture = null;
            int? indoorMoisture = null;
            DateTime? recordDateTime = null;

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Match match = Regex.Match(line, pattern);

                    if (match.Success)
                    {
                        try
                        {
                            DateTime lineDateTime = DateTime.ParseExact(
                                match.Groups[1].Value,
                                "yyyy-MM-dd HH:mm:ss",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.None
                            );

                            // Skippa Maj 2016 och Januari 2017
                            if ((lineDateTime.Year == 2016 && lineDateTime.Month == 5) ||
                                (lineDateTime.Year == 2017 && lineDateTime.Month == 1))
                            {
                                continue;
                            }

                            string location = match.Groups[2].Value;
                            double temperature = double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
                            int moisture = int.Parse(match.Groups[4].Value);

                            if (location == "Ute")
                            {
                                outdoorTemp = temperature;
                                outdoorMoisture = moisture;
                                recordDateTime = lineDateTime;
                            }
                            else if (location == "Inne")
                            {
                                indoorTemp = temperature;
                                indoorMoisture = moisture;
                                if (recordDateTime == null)
                                    recordDateTime = lineDateTime;
                            }

                            // If we have both readings, save the data
                            if (outdoorTemp.HasValue && indoorTemp.HasValue &&
                                outdoorMoisture.HasValue && indoorMoisture.HasValue)
                            {
                                double indoorMoldRisk = MoldCalc.MoldCalculator(indoorTemp.Value, indoorMoisture.Value);
                                double outdoorMoldRisk = MoldCalc.MoldCalculator(outdoorTemp.Value, outdoorMoisture.Value);

                                allData.Add(new WeatherData
                                {
                                    DateTime = recordDateTime.Value,
                                    OutdoorTemp = outdoorTemp.Value,
                                    OutdoorMoisture = outdoorMoisture.Value,
                                    IndoorTemp = indoorTemp.Value,
                                    IndoorMoisture = indoorMoisture.Value,
                                    MoldRisk = indoorMoldRisk,
                                    OutdoorMoldRisk = outdoorMoldRisk
                                });

                                // Reset for next reading
                                outdoorTemp = null;
                                indoorTemp = null;
                                outdoorMoisture = null;
                                indoorMoisture = null;
                                recordDateTime = null;
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
            }

            return allData;
        }
    }
}
