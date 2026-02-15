using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WeatherData2.Models;
using WeatherData2.Services;

namespace WeatherData2.Data
{
    internal class WeatherDataReader
    {
        private const string LogPattern = @"(\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2}),(Ute|Inne),(-?[0-9.]+),(\d+)";

        public static List<WeatherData> GetAllWeatherData(string filePath)
        {
            return ReadWeather(filePath, null);
        }

        public static List<WeatherData> GetDayWeatherData(string filePath, DateTime targetDate)
        {
            return ReadWeather(filePath, targetDate);
        }

        private static List<WeatherData> ReadWeather(string filePath, DateTime? targetDate)
        {
            List<WeatherData> resultList = new List<WeatherData>();

            double? outsideTemp = null, insideTemp = null;
            int? outsideMoisture = null, insideMoisture = null;
            DateTime? logTime = null;

            using (StreamReader reader = new StreamReader(filePath))
            {
                string row;
                while ((row = reader.ReadLine()) != null)
                {
                    Match match = Regex.Match(row, LogPattern);

                    if (match.Success)
                    {
                        try
                        {
                            DateTime currentTime = DateTime.ParseExact(
                                match.Groups[1].Value,
                                "yyyy-MM-dd HH:mm:ss",
                                CultureInfo.InvariantCulture
                                );

                            if ((currentTime.Year == 2016 && currentTime.Month == 5) ||
                                (currentTime.Year == 2017 && currentTime.Month == 1))
                            {
                                continue;
                            }

                            if (targetDate.HasValue && currentTime.Date != targetDate.Value.Date)
                            {
                                continue;
                            }

                            string place = match.Groups[2].Value;
                            double temp = double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
                            int moisture = int.Parse(match.Groups[4].Value);

                            if (place == "Ute")
                            {
                                outsideTemp = temp;
                                outsideMoisture = moisture;
                                logTime = currentTime;
                            }
                            else if (place == "Inne")
                            {
                                insideTemp = temp;
                                insideMoisture = moisture;
                                if (logTime == null) logTime = currentTime;
                            }

                            if (outsideTemp.HasValue && insideTemp.HasValue &&
                                outsideMoisture.HasValue && insideMoisture.HasValue)
                            {
                                resultList.Add(new WeatherData
                                {
                                    DateTime = logTime.Value,
                                    OutdoorTemp = outsideTemp.Value,
                                    OutdoorMoisture = outsideMoisture.Value,
                                    OutdoorMoldRisk = MoldCalc.MoldCalculator(outsideTemp.Value, outsideMoisture.Value),

                                    IndoorTemp = insideTemp.Value,
                                    IndoorMoisture = insideMoisture.Value,
                                    MoldRisk = MoldCalc.MoldCalculator(insideTemp.Value, insideMoisture.Value)
                                });

                                outsideTemp = insideTemp = null;
                                outsideMoisture = insideMoisture = null;
                                logTime = null;

                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
            }
            return resultList;
        }
    }
}
