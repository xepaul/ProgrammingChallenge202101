using System;
using System.Collections.Immutable;
using System.Linq;
using PriceCalculator.Core.DiscountRules;
using PriceCalculator.Infrastructure;

namespace PriceCalculator.Core
{

  public abstract record ActiveDateRange
  {
    private ActiveDateRange(){}
    public sealed record AlwaysActive : ActiveDateRange { }
    public sealed record ActiveTimeWeek(byte WeekNumber) : ActiveDateRange; // monday to sunday // ignore possible bad data for now - week number >52
  }

  public sealed record DiscountRuleActivation(ActiveDateRange ActiveDateRange, DiscountRule DiscountRule);

  public sealed record PennyPrice(uint Value) //unsigned penny value
  {
    public decimal ToPounds() => Value / 100m;
  };
  public abstract record DiscountedPrice
  {
    private DiscountedPrice() {  }
    public sealed record FractionalPercentDiscount(decimal DiscountPriceOff) : DiscountedPrice;
    public sealed record NoDiscount : DiscountedPrice;
  }

  public sealed record ProductDiscount(DiscountedPrice DiscountPriceOff, DiscountRuleIdentity DiscountRuleName);

  public sealed record ShoppingCartItem(ProductIdentifier ProductIdentifier, PennyPrice Price,
      ImmutableList<ProductDiscount> ProductDiscounts)
  {
    public decimal FractionalPercentOff =>
        ProductDiscounts
            .Select(productDiscount =>
                productDiscount.DiscountPriceOff
                    switch // deal with different types of discount // merge into a single discount
                    {
                      DiscountedPrice.FractionalPercentDiscount
                      { DiscountPriceOff: var fractionalPercentOff } => fractionalPercentOff,
                      _ => throw new Exception("case not dealt with")
                    })
            .Sum();
    public decimal DiscountedPrice => Price.ToPounds() * (1 - FractionalPercentOff);
  };

  public sealed record ProductIdentifier(string ProductName);

  public sealed record DiscountRuleIdentity(string DiscountRuleName); // application of one discount may disable other discounts 

  public sealed record DiscountRule(DiscountRuleIdentity DiscountRuleName, IDiscountRule Rule);

  public sealed record DiscountSummaryText(string SummaryText);
  public sealed record DiscountSummary(DiscountSummaryText DiscountSummaryText, decimal Saving);

  public sealed record NamedShoppingListAndDiscount(ImmutableList<ShoppingCartItem> DiscountedShoppingList, ImmutableList<DiscountRuleIdentity> AppliedDiscountRules, ImmutableList<DiscountSummary> DiscountsSummary)
  {
    public decimal GetSubTotal =>
      DiscountedShoppingList.Sum(shoppingItem => shoppingItem.Price.ToPounds());

    public decimal GetTotal => DiscountedShoppingList.Sum(shoppingItem => shoppingItem.DiscountedPrice);
  }
  public sealed record ShoppingListAndDiscount(ImmutableList<Maybe<DiscountedPrice>> DiscountedShoppingList, ImmutableList<DiscountSummary> DiscountsSummary);
  public sealed record ComposableShoppingListAndDiscount(IShopContext ShopContext, ImmutableList<ShoppingCartItem> OriginalShoppingList, ImmutableList<Maybe<DiscountedPrice>> DiscountedShoppingList, ImmutableList<DiscountSummary> DiscountsSummary);
}
