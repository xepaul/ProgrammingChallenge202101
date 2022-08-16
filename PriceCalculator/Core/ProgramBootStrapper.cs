using System.Threading.Tasks;
using Autofac;
using PriceCalculator.Core.DiscountRules;
using PriceCalculator.DataServices;
using Serilog;

namespace PriceCalculator.Core;

public static class ProgramBootStrapper
{
    public static ValueTask<string> ProcessShoppingList(string[] cartItemNames)
    {
        T SetupContainerAndResolve<T>()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ShoppingPriceCalculator>().As<IShoppingPriceCalculator>();
            builder.RegisterType<ProductService>().As<IProductService>();
            builder.RegisterType<DiscountRulesSource>().As<IDiscountRulesSource>();
            builder.RegisterType<ShopContext>().As<IShopContext>();
            builder.Register<ILogger>((c, p) =>
                       new LoggerConfiguration()
                            .MinimumLevel.Debug()
                            .WriteTo.Console()
                            .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
                            .CreateLogger())
                    .SingleInstance();
            return builder.Build().Resolve<T>();
        }

        return SetupContainerAndResolve<IShoppingPriceCalculator>()
            .ShowPriceShoppingList(cartItemNames);
    }
}
