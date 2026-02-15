using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherData2.Data;
using WeatherData2.Models;

namespace WeatherData2.Services
{
    internal class SearchService
    {
        
        public static void SearchByDate()
        {
            try
            {
                Console.WriteLine("Ange år (t.ex. 2016): ");
                int year = int.Parse(Console.ReadLine());

                Console.WriteLine("Ange månad (1-12): ");
                int month = int.Parse(Console.ReadLine());

                Console.WriteLine("Ange dag (1-31): ");
                int day = int.Parse(Console.ReadLine());

                DateTime searchDate = new DateTime(year, month, day);

                // Få all väderdata för dagen
                List<WeatherData> dayData = WeatherDataReader.GetDayWeatherData(AppConfig.FilePath, searchDate);

                if (dayData.Count > 0)
                {
                    DisplayDayData(dayData, searchDate);
                }
                else
                {
                    Console.WriteLine($"Ingen data hittades för {searchDate:yyyy-MM-dd}");
                }

                Console.WriteLine("\nTryck på valfri tangent för att fortsätta...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid sökning: {ex.Message}");
                Console.WriteLine("Tryck på valfri tangent för att fortsätta...");
                Console.ReadKey();
            }
        }

        private static void DisplayDayData(List<WeatherData> dayData, DateTime searchDate)
        {
            Console.WriteLine($"\n=== Väderdata för {searchDate:yyyy-MM-dd} ===");
            Console.WriteLine($"Totalt antal mätningar: {dayData.Count}\n");

            // Kalkylera mögel risk statistik
            double avgMoldRisk = dayData.Average(d => d.MoldRisk);
            double maxMoldRisk = dayData.Max(d => d.MoldRisk);
            double minMoldRisk = dayData.Min(d => d.MoldRisk);

            var highRiskCount = dayData.Count(d => d.MoldRisk >= 80);
            var mediumRiskCount = dayData.Count(d => d.MoldRisk >= 10 && d.MoldRisk < 80);
            var lowRiskCount = dayData.Count(d => d.MoldRisk < 10);

            Console.WriteLine("=== Mögelrisk Inne ===");
            Console.WriteLine($"Genomsnitt: {avgMoldRisk:F1}%");
            Console.WriteLine($"Max: {maxMoldRisk:F1}%");
            Console.WriteLine($"Min: {minMoldRisk:F1}%");
            Console.WriteLine($"\nHög risk (≥80%): {highRiskCount} mätningar ({highRiskCount * 100.0 / dayData.Count:F1}%)");
            Console.WriteLine($"Medel risk (10-79%): {mediumRiskCount} mätningar ({mediumRiskCount * 100.0 / dayData.Count:F1}%)");
            Console.WriteLine($"Låg risk (<10%): {lowRiskCount} mätningar ({lowRiskCount * 100.0 / dayData.Count:F1}%)");

            Console.WriteLine("\n=== Mögelrisk Ute ===");
            double avgOutdoorMoldRisk = dayData.Average(d => d.OutdoorMoldRisk);
            double maxOutdoorMoldRisk = dayData.Max(d => d.OutdoorMoldRisk);
            double minOutdoorMoldRisk = dayData.Min(d => d.OutdoorMoldRisk);

            var outdoorHighRiskCount = dayData.Count(d => d.OutdoorMoldRisk >= 80);
            var outdoorMediumRiskCount = dayData.Count(d => d.OutdoorMoldRisk >= 10 && d.OutdoorMoldRisk < 80);
            var outdoorLowRiskCount = dayData.Count(d => d.OutdoorMoldRisk < 10);

            Console.WriteLine($"Genomsnitt: {avgOutdoorMoldRisk:F1}%");
            Console.WriteLine($"Max: {maxOutdoorMoldRisk:F1}%");
            Console.WriteLine($"Min: {minOutdoorMoldRisk:F1}%");
            Console.WriteLine($"\nHög risk (≥80%): {outdoorHighRiskCount} mätningar ({outdoorHighRiskCount * 100.0 / dayData.Count:F1}%)");
            Console.WriteLine($"Medel risk (10-79%): {outdoorMediumRiskCount} mätningar ({outdoorMediumRiskCount * 100.0 / dayData.Count:F1}%)");
            Console.WriteLine($"Låg risk (<10%): {outdoorLowRiskCount} mätningar ({outdoorLowRiskCount * 100.0 / dayData.Count:F1}%)");

            
            Console.WriteLine("\n=== Temperatur & Fuktighet ===");

            Console.WriteLine($"Inomhus temp - Min: {dayData.Min(d => d.IndoorTemp):F1}°C," +
                $" Max: {dayData.Max(d => d.IndoorTemp):F1}°C," +
                $" Medel: {dayData.Average(d => d.IndoorTemp):F1}°C");

            Console.WriteLine($"Utomhus temp - Min: {dayData.Min(d => d.OutdoorTemp):F1}°C," +
                $" Max: {dayData.Max(d => d.OutdoorTemp):F1}°C," +
                $" Medel: {dayData.Average(d => d.OutdoorTemp):F1}°C");

            Console.WriteLine($"Inomhus fukt - Min: {dayData.Min(d => d.IndoorMoisture):F0}%," +
                $" Max: {dayData.Max(d => d.IndoorMoisture):F0}%," +
                $" Medel: {dayData.Average(d => d.IndoorMoisture):F0}%");

            Console.WriteLine($"Utomhus fukt - Min: {dayData.Min(d => d.OutdoorMoisture):F0}%," +
                $" Max: {dayData.Max(d => d.OutdoorMoisture):F0}%," +
                $" Medel: {dayData.Average(d => d.OutdoorMoisture):F0}%");

            
            Console.WriteLine("\n=== Första 10 mätningarna ===");
            foreach (var data in dayData.Take(10))
            {
                string category = MoldCalc.GetMoldRiskCategory(data.MoldRisk);
                string outdoorCategory = MoldCalc.GetMoldRiskCategory(data.OutdoorMoldRisk);
                Console.WriteLine($"{data.DateTime:HH:mm:ss} - Inne: {data.IndoorTemp:F1}°C ({data.IndoorMoisture}%) Risk: {data.MoldRisk:F1}% ({category}) | Ute: {data.OutdoorTemp:F1}°C ({data.OutdoorMoisture}%) Risk: {data.OutdoorMoldRisk:F1}% ({outdoorCategory})");
            }
        }
    }
}
