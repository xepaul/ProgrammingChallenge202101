using System;

namespace PriceCalculator.Core;

public interface IShopContext
{
    DateTimeOffset ExecutionTime { get; }
    string Region { get; }
    string UserId { get; }
}

public class ShopContext : IShopContext
{
    public DateTimeOffset ExecutionTime => DateTimeOffset.Now;
    public string Region => throw new NotImplementedException();
    public string UserId => throw new NotImplementedException();
}

public class MockShopContext : IShopContext
{
    private readonly Func<DateTime> _getDateTime;

    public MockShopContext(Func<DateTime> getDateTime) => _getDateTime = getDateTime;
    public DateTimeOffset ExecutionTime => _getDateTime();
    public string Region => throw new NotImplementedException();
    public string UserId => throw new NotImplementedException();
}
