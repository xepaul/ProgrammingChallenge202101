using System;
using System.Collections.Immutable;
using System.Linq;
using PriceCalculator.Infrastructure;

namespace PriceCalculator.Core.DiscountRules
{
    public class DependentProductDiscountRule: IDiscountRule
    {
        private readonly Func<ImmutableList<ShoppingCartItem>, Option<ShoppingListAndDiscount>> _tryApplyDiscountRuleFunc;

        private DependentProductDiscountRule(uint activatingProductCount, ProductIdentifier activatingProductIdentifier, ProductIdentifier dependentProductIdentifier, decimal dependentFractionalPercentDiscount) // private ctor
        {
            _tryApplyDiscountRuleFunc =  cartItems =>
            {
                DiscountSummary CreateSummary(decimal totalDiscount) =>  // need to fix message with pluralization and quantity type -> Buy 2 cans of Bean and get a loaf of bread for half price
                    new(new DiscountSummaryText(
                            $"Buy {activatingProductCount} items of {activatingProductIdentifier.ProductName}  get {dependentProductIdentifier.ProductName} for {dependentFractionalPercentDiscount * 100:0.} % off"),
                        totalDiscount);
                    
                var matchingProductsCount = cartItems.Count(shoppingCartItem => shoppingCartItem.ProductIdentifier == activatingProductIdentifier);

                if (matchingProductsCount >= activatingProductCount)
                {
                    var dependentProductsToChange = matchingProductsCount / activatingProductCount;
                    var updated = 
                        cartItems.Aggregate((Shopping: ImmutableList<Option<DiscountedPrice>>.Empty, AppliedCount: 0),
                                (acc, item) => 
                                    item.ProductIdentifier == dependentProductIdentifier && acc.AppliedCount < dependentProductsToChange
                                        ? (acc.Shopping.Add(Option.Some((DiscountedPrice)new DiscountedPrice.FractionalPercentDiscount(dependentFractionalPercentDiscount))), acc.AppliedCount + 1)
                                        : (acc.Shopping.Add(Option<DiscountedPrice>.None), acc.AppliedCount))
                            .Shopping;
                    return cartItems.TryFind(cartItem => cartItem.ProductIdentifier == dependentProductIdentifier)
                        .Map(dependentItem => dependentItem.Price.ToPounds() * dependentProductsToChange * dependentFractionalPercentDiscount)
                        .Map(totalDiscount => new ShoppingListAndDiscount(updated,ImmutableList.Create(CreateSummary(totalDiscount))));
                }
                else
                    return Option<ShoppingListAndDiscount>.None;
            };
        }

        Option<ShoppingListAndDiscount> IDiscountRule.TryApply(IShopContext timeProvider,
            ImmutableList<ShoppingCartItem> cartItems) => _tryApplyDiscountRuleFunc(cartItems);

        public static Option<IDiscountRule> TryCreate(uint activatingProductCount, ProductIdentifier activatingProductIdentifier, 
            ProductIdentifier dependentProductIdentifier, byte percentDiscount) =>
            //should log any failures somewhere
            activatingProductCount < 1 || percentDiscount > 100
                ? Option<IDiscountRule>.None // should be returning a Result<DependentProductDiscount,FailureReason> with reason creation failed type Result<'s,'e> = | Ok of 's | Error of 'e
                : Option.Some((IDiscountRule)new DependentProductDiscountRule(activatingProductCount, activatingProductIdentifier, dependentProductIdentifier, percentDiscount/100m));
    }
}