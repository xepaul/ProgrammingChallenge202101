using System;
using System.Collections.Immutable;
using System.Linq;
using PriceCalculator.Core;
using PriceCalculator.Core.DiscountRules;
using Xunit;
using PriceCalculator.Infrastructure;
using PriceCalculatorTests.TestingSupport;
using static PriceCalculator.Infrastructure.Maybe;

namespace PriceCalculatorTests.Core.DiscountRules;

public class DependentProductDiscountRuleTests
{
    [Fact]
    public void TestDependentProductDiscountRule()
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
        var mockShopContext = new MockShopContext(() => throw new NotImplementedException());

        var expectedDiscount = ((DiscountedPrice)new DiscountedPrice.FractionalPercentDiscount(0.5m));
        var expectedDiscounts =
            Enumerable.Repeat(Just(expectedDiscount), 3)
            .Concat(Enumerable.Repeat(Maybe<DiscountedPrice>.Nothing, 8))
            .ToImmutableList();

        var result =
            DependentProductDiscountRule.TryCreate(2u, new ProductIdentifier(productNameBeans),
                new ProductIdentifier(productNameBread), 50)
                .Bind(r => r.TryApply(mockShopContext, shoppingList));

        AssertMaybe.SequenceEqual(Just(expectedDiscounts.AsEnumerable()),
            result.Map(x => x.DiscountedShoppingList.AsEnumerable()));
    }
}
