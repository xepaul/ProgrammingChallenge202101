using System.Threading.Tasks;
using Autofac;
using PriceCalculator.Core.DiscountRules;
using PriceCalculator.DataServices;

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

            return builder.Build().Resolve<T>();
        }

        return SetupContainerAndResolve<IShoppingPriceCalculator>()
            .ShowPriceShoppingList(cartItemNames);
    }
}
