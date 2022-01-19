using System;
using System.Collections.Immutable;
using System.Linq;
using PriceCalculator.Infrastructure;
using static PriceCalculator.Infrastructure.Option;

namespace PriceCalculator.Core.DiscountRules;
public  class ProductDiscountRule : IDiscountRule
{
    private  Func<ImmutableList<ShoppingCartItem>, Option<ShoppingListAndDiscount>> _tryApplyDiscountRuleFunc;
    private ProductDiscountRule(ProductIdentifier discountedProductIdentifier, decimal discount)
    {
        _tryApplyDiscountRuleFunc =  cartItems =>
        {
            DiscountSummary CreateSummary(decimal totalDiscount) =>
                    new(new DiscountSummaryText($"Apples {discount * 100:0.} % off"), totalDiscount);
            
            return cartItems.TryFind(cartItem => cartItem.ProductIdentifier == discountedProductIdentifier)
                        .Map(_ =>
                        {
                            var updated = cartItems
                                .Select(shoppingItem => shoppingItem.ProductIdentifier == discountedProductIdentifier
                                                        ? Some((DiscountedPrice)new DiscountedPrice.FractionalPercentDiscount(discount))
                                                        : Option<DiscountedPrice>.None)
                                .ToImmutableList(); 
                            var totalDiscount = cartItems.Count(i => i.ProductIdentifier == discountedProductIdentifier) * discount;
                            return new ShoppingListAndDiscount(updated, ImmutableList.Create(CreateSummary(totalDiscount)));
                        });
        };
    }

    Option<ShoppingListAndDiscount> IDiscountRule.TryApply(IShopContext timeProvider, ImmutableList<ShoppingCartItem> cartItems) => _tryApplyDiscountRuleFunc(cartItems);

    /// <summary>
    /// Validate parameters of rule
    /// </summary>
    /// <param name="discountedProductIdentifier"></param>
    /// <param name="percentDiscount"></param>
    /// <returns></returns>
    public static Option<IDiscountRule> TryCreate(ProductIdentifier discountedProductIdentifier, byte percentDiscount) =>
        percentDiscount > 100
            ? Option<IDiscountRule>.None // should be returning a Result<DependentProductDiscount,FailureReason> with reason creation failed type Result<'s,'e> = | Ok of 's | Error of 'e
            : Some((IDiscountRule)new ProductDiscountRule(discountedProductIdentifier, percentDiscount/100m));
}
