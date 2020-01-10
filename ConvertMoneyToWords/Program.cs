using System;
using static LanguageExt.Prelude;

namespace ConvertMoneyToWords
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a currency value between 1p and £999,999,999.99");
            var value = Console.ReadLine()?.ToLower().Replace("£", "").Replace("p", "");

            Console.WriteLine(Functions.GetWords(value));
        }
    }
}
