using System.Collections.Immutable;
using System.Linq;
using PriceCalculator.Core;
using PriceCalculator.Core.DiscountRules;
using PriceCalculator.Infrastructure;

namespace PriceCalculator.DataServices;

public interface IProductService
{
    /// <summary>
    /// <para> validate product names and gets current product price, returns each string product name as either a found
    /// product with a price or the unfound product name </para>
    /// </summary>
    /// <param name="productNames"></param>
    /// <returns></returns>
    ImmutableList<Either<ProductIdentifier, ShoppingCartItem>> GetProductPrices(string[] productNames);
}

public class ProductService : IProductService
{
    /* 
        Should really parse the product definitions
            Beans – 65p per can
            Bread – 80p per loaf
            Milk – £1.30 per bottle
            Apples – £1.00 per bag // referred to as Apple on command line
    */
    private static ImmutableDictionary<ProductIdentifier, PennyPrice> ProductToPriceMap =
        ImmutableDictionary<ProductIdentifier, PennyPrice>.Empty
            .Add(new ProductIdentifier("beans"), new PennyPrice(65U))
            .Add(new ProductIdentifier("bread"), new PennyPrice(80U))
            .Add(new ProductIdentifier("milk"), new PennyPrice(130U))
            .Add(new ProductIdentifier("apple"), new PennyPrice(100U));

    public ProductService( /* real dependencies such as config etc*/)
    {
    }

    public ImmutableList<Either<ProductIdentifier, ShoppingCartItem>> GetProductPrices(string[] productNames) =>
        productNames
            .Select(productName => productName.ToLower())
            .Select(productName => new ProductIdentifier(productName.ToLower()))
            .Select(productName =>
                ProductToPriceMap.TryGetValue(productName)
                    .Map(price => new ShoppingCartItem(productName, price, ImmutableList<ProductDiscount>.Empty))
                    .Note(productName)
            )
            .ToImmutableList();
}
