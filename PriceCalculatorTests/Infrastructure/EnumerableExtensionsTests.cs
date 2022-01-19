using System.Linq;
using PriceCalculator.Infrastructure;
using PriceCalculatorTests.TestingSupport;
using Xunit;

namespace PriceCalculatorTests.Infrastructure;

public class EnumerableExtensionsTests
{
    [Fact]
    public void TestPartitionEithers()
    {
        var (odds, evens) =
            Enumerable.Range(1, 10)
                .Select(x => x % 2 == 0 ? Either<int, int>.Right(x) : Either<int, int>.Left(x))
                .PartitionEithers();

        AssertExtras.SequenceEqual(Enumerable.Range(1, 10).Where(x => x % 2 == 1), odds);
        AssertExtras.SequenceEqual(Enumerable.Range(1, 10).Where(x => x % 2 == 0), evens);
    }
}
