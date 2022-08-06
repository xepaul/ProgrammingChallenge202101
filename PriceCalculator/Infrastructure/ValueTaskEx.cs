using System;
using System.Threading.Tasks;

namespace PriceCalculator.Infrastructure;

public static class ValueTaskEx
{
  public static async ValueTask<U> Map<T, U>(this ValueTask<T> task, Func<T, U> mapper) =>
        mapper(await task.ConfigureAwait(false));
  public static async ValueTask<U> Select<T, U>(this ValueTask<T> task, Func<T, U> mapper) =>
      mapper(await task.ConfigureAwait(false));
  public static async ValueTask<U> SelectMany<T, U>(this ValueTask<T> task, Func<T, ValueTask<U>> mapper) =>
      await mapper(await task.ConfigureAwait(false));

}