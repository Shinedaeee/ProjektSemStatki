using System;
using System.Collections.Generic;
using System.Text;

namespace GeneralTools
{
    public class Tools
    {
        public static int WprowadzInt(string tekst, int min, int max)
        {
            while (true)
            {
                Console.Write(tekst);

                int liczba = 0;

                if (int.TryParse(Console.ReadLine(), out liczba) == true && liczba > min && liczba < max)
                {
                    return liczba;
                }
                else
                {
                    Console.WriteLine($"Błędna wartość. ");
                }
            }
        }
    }
}
