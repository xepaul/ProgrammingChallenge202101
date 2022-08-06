using System;
using System.Collections.Immutable;
using System.Linq;
using PriceCalculator.Core;
using PriceCalculator.Core.DiscountRules;
using PriceCalculator.Infrastructure;
using static PriceCalculator.Infrastructure.Maybe;
using Xunit;

namespace PriceCalculatorTests.Core;

public class DiscountRuleAggregatorTests
{
    [Fact]
    public void TestPrintAppleOfferBeansOffer()
    {
        var productNameBread = "bread";
        var productNameBeans = "beans";
        var breadItem = new ShoppingCartItem(new ProductIdentifier(productNameBread), new PennyPrice(080U),
            ImmutableList<ProductDiscount>.Empty);
        var beansItem = new ShoppingCartItem(new ProductIdentifier(productNameBeans), new PennyPrice(065U),
            ImmutableList<ProductDiscount>.Empty);
        var shoppingList =
            Enumerable.Repeat(breadItem, 4)
                .Concat(Enumerable.Repeat(beansItem, 7))
                .ToImmutableList();
        var mockShopContext = new MockShopContext(() => DateTime.Now);

        var result = DependentProductDiscountRule.TryCreate(2u, new ProductIdentifier(productNameBeans),
                new ProductIdentifier(productNameBread), 50)
            .Map(rule => new DiscountRule(new DiscountRuleIdentity(productNameBeans), rule))
            .Map(namedRule => DiscountRuleAggregator.ApplyDiscountsSync(mockShopContext,
                                                                    ImmutableList.Create(namedRule), shoppingList));

        var expectedTotal = 0.8m * 3m * 0.5m + 0.8m + 0.65m * 7;
        var expectedSubTotal = 0.8m * 4m + 0.65m * 7;
        var expectedMultiBuyDiscount = 0.8m * 3m * 0.5m;
        Assert.Equal(Just(expectedSubTotal), result.Map(r => r.GetSubTotal));
        Assert.Equal(Just(expectedTotal), result.Map(r => r.GetTotal));
        Assert.Equal(Just(expectedMultiBuyDiscount), result.Map(r => r.DiscountsSummary[0].Saving));
        Assert.Equal(Just(1), result.Map(r => r.DiscountsSummary.Count));
    }
}
