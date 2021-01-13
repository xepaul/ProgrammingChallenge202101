using System;
using System.Collections.Immutable;
using System.Linq;
using PriceCalculator.Core;
using PriceCalculator.Core.DiscountRules;
using PriceCalculator.Infrastructure;
using static PriceCalculator.Infrastructure.Option;

namespace PriceCalculator.DataServices
{
    public interface IDiscountRulesSource
    {
        /// <summary>
        /// get the discount rules for the current shopping context
        /// </summary>
        /// <param name="shopContext"></param>
        /// <returns></returns>
        ImmutableList<DiscountRule> GetRules(IShopContext shopContext);
    }
    
    public class DiscountRulesSource : IDiscountRulesSource
    {
        /*
         Current special offers:
            - Apples have a 10% discount off their normal price this week
            - Buy 2 cans of Bean and get a loaf of bread for half price
         */
        readonly ImmutableList<DiscountRuleActivation> _discountRules;
        
        public DiscountRulesSource() {
            // these rules would likely be defined in a simple DSL/service built via provided primitives
            var dateOfThisWeek = DateTimeOffset.Now; // this should be injected
            
            var appleDiscount =  
                ProductDiscountRule.TryCreate(new ProductIdentifier("apple"), 10)
                    .Map(rule => new DiscountRule(new DiscountRuleIdentity("appplesProductDiscount"), rule))
                    .Map(discountRule => new DiscountRuleActivation(new ActiveDateRange.ActiveTimeWeek(dateOfThisWeek.WeekNumberOfYear()),discountRule ));
            
            var breadDiscount = 
                    DependentProductDiscountRule.TryCreate(2, new ProductIdentifier("beans"), new ProductIdentifier("bread"), 50)
                        .Map(rule => new DiscountRule(new DiscountRuleIdentity("beans"), rule))
                        .Map(discountRule => new DiscountRuleActivation(new ActiveDateRange.AlwaysActive(),discountRule ));
            _discountRules =
                new[] { appleDiscount, 
                        breadDiscount}
                    .MapOption(rule => rule) 
                    .ToImmutableList();
        }

        public ImmutableList<DiscountRule> GetRules(IShopContext shopContext)
        {
            bool IsActiveRule(DiscountRuleActivation activation) =>
                activation.ActiveDateRange switch
                {
                    ActiveDateRange.AlwaysActive => true,
                    ActiveDateRange.ActiveTimeWeek
                        {WeekNumber: var dayOfTheWeek} when dayOfTheWeek == shopContext.ExecutionTime.WeekNumberOfYear() => true,
                    _ => false
                };

            return _discountRules
                        .Where(IsActiveRule)
                        .Select(x => x.DiscountRule)
                        .ToImmutableList(); // allowing rule retrieval to filter out none date active rules, more complicated date rules would be inside rules
        }
    }
}

