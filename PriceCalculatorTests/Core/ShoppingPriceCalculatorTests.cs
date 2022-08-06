using System.Collections.Immutable;
using System.Threading.Tasks;
using PriceCalculator.Core;
using PriceCalculator.Core.DiscountRules;
using Xunit;

namespace PriceCalculatorTests.Core;

public class ShoppingPriceCalculatorTests
{
    [Fact]
    public void TestPrintNoOffers()
    {
        var expected = "Subtotal: £1.30\n" +
                       "(No offers available)\n" +
                       "Total price: £1.30\n";
        var shoppingList = ImmutableList.Create(new ShoppingCartItem(new ProductIdentifier("apple"), new PennyPrice(130U), ImmutableList<ProductDiscount>.Empty));
        var discounts = new NamedShoppingListAndDiscount(shoppingList, ImmutableList<DiscountRuleIdentity>.Empty,
            ImmutableList<DiscountSummary>.Empty);
        var result = ShoppingPriceCalculator.ShowDiscountedShoppingList(discounts);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task TestPrintAppleOffer()
    {
        var expected = "Subtotal: £3.10\n" +
                       "Apples 10 % off: -10p\n" +
                       "Total: £3.00\n";

        var result = await ProgramBootStrapper.ProcessShoppingList(new[] { "Apple", "Milk", "Bread" });

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task TestPrintAppleWithUknowns()
    {
        var expected = "Subtotal: £1.00\n" +
                       "Apples 10 % off: -10p\n" +
                       "Total: £0.90\n" +
                       "\nUnknown products: milkz,breadz\n";

        var result = await ProgramBootStrapper.ProcessShoppingList(new[] { "Apple", "Milkz", "Breadz" });

        Assert.Equal(expected, result);
    }
}
