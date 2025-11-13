#if NETSTANDARD2_0
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sb.Extensions.System.Collections.Generic;

/// <summary>
///   Queue 拓展
/// </summary>
public static class QueueExtensions
{
  /// <summary>
  /// </summary>
  /// <param name="queue"></param>
  /// <typeparam name="T"></typeparam>
  extension<T>(Queue<T> queue)
  {
    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryPeek(out T? result)
    {
      if (queue.Count > 0)
      {
        result = queue.Peek();
        return true;
      }

      result = default;
      return false;
    }

    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryDequeue(out T? result)
    {
      if (queue.Count > 0)
      {
        result = queue.Dequeue();
        return true;
      }

      result = default;
      return false;
    }
  }
}
#endif