using System;
using System.Threading.Tasks;
using PriceCalculator.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Autofac.Extensions.DependencyInjection;
namespace PriceCalculator;

class Program
{
    static async Task Main(string[] args) // should really be async
    {
        var result =  await ProgramBootStrapper.ProcessShoppingList(args);
        Console.Write(result);
     
    }
}
