using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using PriceCalculator.Core.DiscountRules;
using PriceCalculator.DataServices;
using PriceCalculator.Infrastructure;

namespace PriceCalculator.Core;

public interface IShoppingPriceCalculator
{
    /// <summary>
    /// <para> get the shopping list values for the given products, applies and discounts that may be applicable, reports
    /// any unknown products </para>
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    ValueTask<string> ShowPriceShoppingList(string[] items);
}

public class ShoppingPriceCalculator : IShoppingPriceCalculator
{
    private readonly IShopContext _shopContext;
    private readonly IProductService _productService;
    private readonly IDiscountRulesSource _discountRulesSource;

    public ShoppingPriceCalculator(IShopContext shopContext, IProductService productService,
        IDiscountRulesSource discountRulesSource)
    {
        _shopContext = shopContext;
        _productService = productService;
        _discountRulesSource = discountRulesSource;
    }

    ValueTask<string> IShoppingPriceCalculator.ShowPriceShoppingList(string[] items) =>
        ShowPriceShoppingList(_shopContext, _productService, _discountRulesSource, items);


    public static string ShowDiscountedShoppingList(NamedShoppingListAndDiscount discountedShoppingList)
    {
        string FormatPrice(decimal value) =>
            value >= 1m ? $"£{value:0.00}" : $"{value * 100:0}p"
        ; // eg £3.10 -10p move out to static class for testing

        string FormatPricePounds(decimal value) => $"£{value:0.00}";

        var subTotal = discountedShoppingList.GetSubTotal;
        var total = discountedShoppingList.GetTotal;
        var savings = discountedShoppingList.DiscountsSummary
            .Select(summary => $"{summary.DiscountSummaryText.SummaryText}: {FormatPrice(-summary.Saving)}")
            .DefaultIfEmpty("(No offers available)");

        var receiptText = discountedShoppingList.DiscountsSummary.Any()
            ? $"Subtotal: {FormatPricePounds(subTotal)}\n{string.Join("\n", savings)}\nTotal: {FormatPricePounds(total)}\n"
            : $"Subtotal: {FormatPricePounds(subTotal)}\n{string.Join("\n", savings)}\nTotal price: {FormatPricePounds(total)}\n";

        return receiptText;
    }

    public static async ValueTask<(ImmutableList<ProductIdentifier> unknownIdentifiers, NamedShoppingListAndDiscount shoppingList)>
        PriceShoppingList(IShopContext shopContext, IProductService productService,
            IDiscountRulesSource discountRulesSource, string[] cartItemNames)
    {
        var (unknowns, shoppingBasket) =
            await productService.GetProductPrices(cartItemNames).Map(m=>m.PartitionEithers()); 
        var rules = discountRulesSource.GetRules(shopContext); // the date should be injected 
        return (unknowns,
            DiscountRuleAggregator.ApplyDiscountsSync(shopContext, rules, shoppingBasket));          
    }

    public static async ValueTask<string> ShowPriceShoppingList(IShopContext shopContext, IProductService productService,
        IDiscountRulesSource discountRulesSource, string[] cartItemsNames)
    {
        string PrintUnknownProducts(ImmutableList<ProductIdentifier> unknownProducts) =>
            $"Unknown products: {string.Join(",", unknownProducts.Select(x => x.ProductName))}\n";

        var (unknowns, discountedShoppingList) =
            await PriceShoppingList(shopContext, productService, discountRulesSource, cartItemsNames);
        var printout = ShowDiscountedShoppingList(discountedShoppingList);
        var printoutWithErrors =
            unknowns.Count > 0
                ? $"{printout}\n{PrintUnknownProducts(unknowns)}"
                : printout;
        return printoutWithErrors;
    }
}
