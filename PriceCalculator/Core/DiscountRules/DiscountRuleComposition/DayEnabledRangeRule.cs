using System;
using System.Collections.Immutable;
using PriceCalculator.Infrastructure;

namespace PriceCalculator.Core.DiscountRules.DiscountRuleComposition
{
    public class DayEnabledRangeRule : IDiscountRule
    {
        private readonly DateTime _fromDate;
        private readonly uint _daysRunning;

        public DayEnabledRangeRule(DateTime fromDate, uint daysRunning)
        {
            _fromDate = fromDate;
            _daysRunning = daysRunning;
        }
        public Option<ShoppingListAndDiscount> TryApply(IShopContext timeProvider, ImmutableList<ShoppingCartItem> shoppingCartItems) =>
            timeProvider.ExecutionTime.Date < _fromDate.Date || timeProvider.ExecutionTime.Date > _fromDate.Date.AddDays(_daysRunning) // only apply rule during specified dates should really be aligned to a week, whats the start of the week monday?
                ? Option<ShoppingListAndDiscount>.None
                : Option<ShoppingListAndDiscount>.Some(new ShoppingListAndDiscount( ImmutableList<Option<DiscountedPrice>>.Empty,  ImmutableList<DiscountSummary>.Empty));
    }
}