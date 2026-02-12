using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WeatherData.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace WeatherData
{
    internal class SaveToFileService
    {
        public static void SaveToFile(string content, string defaultFileName)
        {
            Console.WriteLine("\nVill du spara resultatet till en fil? (j/n): ");
            string saveChoice = Console.ReadLine();

            if (saveChoice?.ToLower() == "j")
            {
                Console.WriteLine("Ange filnamn (tryck Enter för standard): ");
                string customName = Console.ReadLine();

                string fileName = string.IsNullOrWhiteSpace(customName) ? defaultFileName : customName;

                // Add .txt if not present
                if (!fileName.EndsWith(".txt"))
                {
                    fileName += ".txt";
                }

                File.WriteAllText(fileName, content, Encoding.UTF8);
                Console.WriteLine($"\nFilen sparad som: {fileName}");
                Console.WriteLine($"Plats: {Path.GetFullPath(fileName)}");
            }
        }

        public static void SaveWeatherReport(List<MeteorologicalSeason> seasons)
        {
           
            Console.Clear();
            Console.WriteLine("Genererar rapport");

            string filePath = "tempdata5-med fel.txt";

            var allData = WeatherDataReader.GetAllWeatherData(filePath);
            var monthlyData = allData
                 .GroupBy(d => new { d.DateTime.Year, d.DateTime.Month })
                 .OrderBy(g => g.Key.Year)
                 .ThenBy(g => g.Key.Month);

            StringBuilder output = new StringBuilder();

            output.AppendLine("=========================");
            output.AppendLine("     MÅNADS RAPPORT      ");
            output.AppendLine("=========================");

            //Tar fram medelvärden för varje månad.
            foreach (var month in monthlyData)
            {
                string monthName = new DateTime(month.Key.Year, month.Key.Month, 1)
                    .ToString("MMMM yyyy", new CultureInfo("sv-SE"));

                output.AppendLine($"--- {monthName.ToUpper()} ---");
                output.AppendLine($"Medeltemperatur ute: {month.Average(d => d.OutdoorTemp):F1}°C");
                output.AppendLine($"Medeltemperatur inne: {month.Average(d => d.IndoorTemp):F1}°C");
                output.AppendLine($"Medelluftfuktighet ute: {month.Average(d => d.OutdoorMoisture):F1}%");
                output.AppendLine($"Medelluftfuktighet inne: {month.Average(d => d.IndoorMoisture):F1}%");
                output.AppendLine($"Medelmögelrisk ute: {month.Average(d => d.OutdoorMoldRisk):F1}%");
                output.AppendLine($"Medelmögelrisk inne: {month.Average(d => d.MoldRisk):F1}%");
                output.AppendLine();
            }

            var autumnDate = seasons.FirstOrDefault(s => s.Season == "Höst")?.StartDate;
            var winterDate = seasons.FirstOrDefault(s => s.Season == "Mild Vinter")?.StartDate;
            output.AppendLine($"Höststart: {autumnDate?.ToString("yyyy-MM-dd") ?? "N/A"}");
            output.AppendLine($"Vinterstart: {winterDate?.ToString("yyyy-MM-dd") ?? "N/A"}");
            output.AppendLine("=====================");
            output.AppendLine("    Mögelalgoritm    ");
            output.AppendLine("=====================");
            output.AppendLine("Mögelrisken beräknas mellan tre zoner: torr zon (0–10%), mellanriskzon (10–80%) och högriskzon (80–100%), där gränsen för högrisk beror på temperaturen.");
            Console.WriteLine(output);
            SaveToFile(output.ToString(), "Månadsrapport");
           
            Console.WriteLine("\nTryck på valfri tangent för att återgå till menyn...");
            Console.ReadKey();
        }
    }
}
