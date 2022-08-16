using System;
using System.Threading.Tasks;
using PriceCalculator.Core;
namespace PriceCalculator;
using Serilog;
using Microsoft.Extensions.Hosting;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extensions;
class Program
{
  static async Task Main(string[] args) // should really be async
  {  
    var result = await ProgramBootStrapper.ProcessShoppingList(args);
    Console.Write(result);
  }
}
