using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WeatherData2.Services;
using WeatherData2.Data;
using WeatherData2.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace WeatherData2.Services
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

            var allData = WeatherDataReader.GetAllWeatherData(AppConfig.FilePath);
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
            // output.AppendLine("Mögelrisken beräknas mellan tre zoner: torr zon (0–10%), mellanriskzon (10–80%) och högriskzon (80–100%), där gränsen för högrisk beror på temperaturen.");
            output.AppendLine("Mogelriksen beräknas utifrån relativ luftfuktighet och temperatur");
            output.AppendLine();
            output.AppendLine("Algoritmen delar in risken i tre zoner:");
            output.AppendLine("1. Torr zon (0-10%)");
            output.AppendLine("- Gäller vid luftfuktighet under 78%");
            output.AppendLine("- Risken skalas linjärt från 0 till 10%");
            output.AppendLine();
            output.AppendLine("2. Mellanriskzon (10-80%)");
            output.AppendLine("- Gäller mellan 78% och temperaturberoende högriskgräns");
            output.AppendLine("- Risken ökar linjärt");
            output.AppendLine();
            output.AppendLine("3. Högriskzon (80-100%)");
            output.AppendLine("- Aktiveras när luftfuktigheten passerar högriskgränsen");
            output.AppendLine("- Ju högre temperatur, desto lägre luftfuktighet krävs för högrisk");
            output.AppendLine("- Risken skalas linjärt från 80 upp till 100%");
            output.AppendLine();
            output.AppendLine("Temperatur påverkan:");
            output.AppendLine("- Vid högre temperatur krävs lägre luftfuktighet för att mögelrisk ska bli hög.");
            output.AppendLine("- Vilket betyder att Mögel utvecklas snabbare i varmare miljöer");

            Console.WriteLine(output);
            SaveToFile(output.ToString(), "Månadsrapport");
           
            Console.WriteLine("\nTryck på valfri tangent för att återgå till menyn...");
            Console.ReadKey();
        }
    }
}
