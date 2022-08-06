using System;
using System.Collections.Immutable;
using System.Linq;
using PriceCalculator.Infrastructure;
using static PriceCalculator.Infrastructure.Maybe;

namespace PriceCalculator.Core.DiscountRules.DiscountRuleComposition;

public interface IComposableDiscountRule
{
    Maybe<ComposableShoppingListAndDiscount> TryComposeApply(ComposableShoppingListAndDiscount shoppingState);
}

public class ComposableDiscountRule : IComposableDiscountRule
{
    private readonly IDiscountRule _discountRule;

    public  ComposableDiscountRule(IDiscountRule discountRule) => _discountRule = discountRule;

    public Maybe<ComposableShoppingListAndDiscount> TryComposeApply(ComposableShoppingListAndDiscount shoppingState) =>
        _discountRule.TryApply(shoppingState.ShopContext,shoppingState.OriginalShoppingList)
            .Map(x=>shoppingState with { DiscountedShoppingList = x.DiscountedShoppingList, DiscountsSummary = x.DiscountsSummary});

    public Maybe<ComposableShoppingListAndDiscount> TryApplyStart(IShopContext timeProvider, ImmutableList<ShoppingCartItem> shoppingCartItems) =>
        ((IComposableDiscountRule) this).TryComposeApply(new ComposableShoppingListAndDiscount(timeProvider,
            shoppingCartItems,
            shoppingCartItems.Select(_ => Just((DiscountedPrice) new DiscountedPrice.NoDiscount()))
                .ToImmutableList(), ImmutableList<DiscountSummary>.Empty));
}

public class CompositeDiscountRule : IDiscountRule
{
    private readonly Func<ComposableShoppingListAndDiscount, Maybe<ComposableShoppingListAndDiscount>> _f;
    public  CompositeDiscountRule(Func<ComposableShoppingListAndDiscount, Maybe<ComposableShoppingListAndDiscount>> f) => _f = f;
    
    public Maybe<ShoppingListAndDiscount> TryApply(IShopContext timeProvider, ImmutableList<ShoppingCartItem> shoppingCartItems) =>
        _f(new ComposableShoppingListAndDiscount(timeProvider,
                shoppingCartItems,
                shoppingCartItems
                    .Select(_ => Just((DiscountedPrice) new DiscountedPrice.NoDiscount()))
                    .ToImmutableList(),
                ImmutableList<DiscountSummary>.Empty))
            .Map(x=> new ShoppingListAndDiscount(x.DiscountedShoppingList,x.DiscountsSummary));
}

public class BasicComposeDependentRule : IDiscountRule
{
    private readonly IDiscountRule _r;
    private readonly IDiscountRule _r2;

    public BasicComposeDependentRule(IDiscountRule r,IDiscountRule r2)
    {
        _r = r;
        _r2 = r2;
    }

    public Maybe<ShoppingListAndDiscount> TryApply(IShopContext timeProvider, ImmutableList<ShoppingCartItem> shoppingCartItems) => 
        _r.TryApply(timeProvider, shoppingCartItems).Bind(_ => _r2.TryApply(timeProvider, shoppingCartItems));

    public static IDiscountRule Compose(IDiscountRule rule, IDiscountRule dependentRule) =>
        new BasicComposeDependentRule(rule, dependentRule);
}
