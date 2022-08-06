using System.Collections.Immutable;
using System.Linq;
using PriceCalculator.Infrastructure;

namespace PriceCalculator.Core;

public static class DiscountRuleAggregator
{
    public static NamedShoppingListAndDiscount ApplyDiscountsSync(IShopContext currentTimeProvider, ImmutableList<DiscountRule> discountRules, ImmutableList<ShoppingCartItem> shoppingList)
    {
        static NamedShoppingListAndDiscount UpdateDiscount(NamedShoppingListAndDiscount accShoppingDiscount, ShoppingListAndDiscount shoppingListAndDiscount, DiscountRuleIdentity rule) =>
            accShoppingDiscount with
            {
                DiscountedShoppingList =
                MergeShoppingListWithDiscounts(accShoppingDiscount, shoppingListAndDiscount, rule),
                AppliedDiscountRules = accShoppingDiscount.AppliedDiscountRules.Add(rule),
                DiscountsSummary =
                accShoppingDiscount.DiscountsSummary.AddRange(shoppingListAndDiscount.DiscountsSummary)
            };

        static ImmutableList<ShoppingCartItem> MergeShoppingListWithDiscounts(NamedShoppingListAndDiscount namedShoppingListAndDiscount, ShoppingListAndDiscount shoppingListAndDiscount,
                                                                        DiscountRuleIdentity discountRule) =>
            namedShoppingListAndDiscount.DiscountedShoppingList
                .Zip(shoppingListAndDiscount.DiscountedShoppingList,
                    (cartItem, possibleDiscount) =>
                        possibleDiscount
                                .Fold(cartItem,
                                (shoppingCartItem, discountPrice) =>
                                        shoppingCartItem with
                                        { ProductDiscounts = cartItem.ProductDiscounts.Add(new ProductDiscount(discountPrice, discountRule)) }))
                .ToImmutableList();

        return discountRules.Aggregate(
            new NamedShoppingListAndDiscount(shoppingList, ImmutableList<DiscountRuleIdentity>.Empty, ImmutableList<DiscountSummary>.Empty),
            (accShoppingDiscount, rule) =>
                rule.Rule.TryApply(currentTimeProvider, accShoppingDiscount.DiscountedShoppingList)
                    .Map(shoppingListAndDiscount =>
                        UpdateDiscount(accShoppingDiscount, shoppingListAndDiscount, rule.DiscountRuleName))
                    .DefaultValue(accShoppingDiscount));
    }
}
