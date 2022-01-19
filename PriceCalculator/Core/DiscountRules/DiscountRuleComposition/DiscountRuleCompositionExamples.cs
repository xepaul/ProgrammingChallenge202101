using System;
using PriceCalculator.Infrastructure;

namespace PriceCalculator.Core.DiscountRules.DiscountRuleComposition;

public static class DiscountRuleCompositionExamples
{
    public static Option<DiscountRuleActivation> ExampleComposedAppleDiscountWithDateRangeThisWeek()
    {
        var dateOfThisWeek = DateTimeOffset.Now;
        return
            ProductDiscountRule.TryCreate(new ProductIdentifier("apple"), 10)
                .Map(rule =>
                    rule.ComposeFromResult(x =>
                        x.Filter(y => y.ShopContext.ExecutionTime.WeekNumberOfYear() == dateOfThisWeek.WeekNumberOfYear())))
                .Map(rule => new DiscountRule(new DiscountRuleIdentity("appplesProductDiscount"), rule))
                .Map(discountRule => new DiscountRuleActivation(new ActiveDateRange.ActiveTimeWeek(dateOfThisWeek.WeekNumberOfYear()), discountRule));
    }
}

public static class RuleExt
{
    public static IComposableDiscountRule ToComposableDiscountRule(this IDiscountRule r1) =>
        new ComposableDiscountRule(r1);

    public static IDiscountRule ComposeFromResult(this IDiscountRule rule,
        Func<Option<ComposableShoppingListAndDiscount>, Option<ComposableShoppingListAndDiscount>> f) =>
        new CompositeDiscountRule(x => f(rule.ToComposableDiscountRule().TryComposeApply(x)));

    public static IDiscountRule ComposeAsDrivingRule(this IDiscountRule r1, IDiscountRule r2) =>
        BasicComposeDependentRule.Compose(r1, r2);

    public static IDiscountRule ComposeFrom(this IDiscountRule rule,
        Func<ComposableShoppingListAndDiscount, Option<ComposableShoppingListAndDiscount>> f) =>
        new CompositeDiscountRule(x => rule.ToComposableDiscountRule().TryComposeApply(x).Bind(f));
}
