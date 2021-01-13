using System;
using PriceCalculator.Core;
using PriceCalculator.Core.DiscountRules;
namespace PriceCalculator
{
    class Program
    {
        static void Main(string[] args) // should really be async
        {
            Console.Write(ProgramBootStrapper.ProcessShoppingList(args));
        }
    }
}