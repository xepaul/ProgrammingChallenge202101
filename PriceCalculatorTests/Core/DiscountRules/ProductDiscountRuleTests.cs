using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using PriceCalculator.Core;
using PriceCalculator.Core.DiscountRules;
using Xunit;
using PriceCalculator.Infrastructure;
using PriceCalculatorTests.TestingSupport;
using static PriceCalculator.Infrastructure.Option;


namespace PriceCalculatorTests.Core.DiscountRules
{
    public class ProductDiscountRuleTests
    {
        [Fact]
        public void TestDependentProductDiscountRule()
        {
            var productNameApple = "apple";
            var productNameBeans = "beans";
            var appleItem = new ShoppingCartItem(new ProductIdentifier(productNameApple), new PennyPrice(080U),
                ImmutableList<ProductDiscount>.Empty);
            var beansItem = new ShoppingCartItem(new ProductIdentifier(productNameBeans), new PennyPrice(065U),
                ImmutableList<ProductDiscount>.Empty);
            var shoppingList =
                Enumerable.Repeat(appleItem, 4)
                    .Concat(Enumerable.Repeat(beansItem, 7))
                    .ToImmutableList();
            
            var mockShopContext = new MockShopContext(() => throw new NotImplementedException());

            var expectedDiscounts = 
                Enumerable.Repeat(Some((DiscountedPrice) new DiscountedPrice.FractionalPercentDiscount(0.5m)), 4)
                    .Concat(Enumerable.Repeat(Option<DiscountedPrice>.None, 7))
                    .ToImmutableList();

            var result = 
                ProductDiscountRule.TryCreate(new ProductIdentifier(productNameApple),50)
                    .Bind(r => r.TryApply(mockShopContext, shoppingList));
        
            AssertOption.SequenceEqual(Some(expectedDiscounts.AsEnumerable()),
                result.Map(x => x.DiscountedShoppingList.AsEnumerable()));
        }
    }
}