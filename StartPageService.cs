using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherData.Services
{
    internal class StartPageService
    {
        public static string ShowStartPage()
        {
            Console.WriteLine("Välkommen till väder appen!");
            Console.WriteLine("1. Sök efter datum");
            Console.WriteLine("2. Se statistik");
            Console.WriteLine("3. Avsluta");
            Console.Write("\nVälj alternativ (1-3): ");
            var input = Console.ReadLine();

            return input;
        }
    }
}
