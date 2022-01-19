using System;
using System.Collections.Immutable;
using PriceCalculator.Infrastructure;

namespace PriceCalculator.Core.DiscountRules;
public interface IDiscountRule
{
    /// <summary>
    /// Apply rule of report failure
    /// </summary>
    /// <param name="timeProvider"></param>
    /// <param name="shoppingCartItems"></param>
    /// <returns></returns>
    Option<ShoppingListAndDiscount> TryApply(IShopContext timeProvider, ImmutableList<ShoppingCartItem> shoppingCartItems);
}
 
