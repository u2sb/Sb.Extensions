#if NETSTANDARD2_0
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sb.Extensions.System.Collections.Generic;

/// <summary>
///   Stack 拓展
/// </summary>
public static class StackExtensions
{
  /// <summary>
  /// </summary>
  /// <param name="stack"></param>
  /// <typeparam name="T"></typeparam>
  extension<T>(Stack<T> stack)
  {
    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryPeek(out T? result)
    {
      if (stack.Count > 0)
      {
        result = stack.Peek();
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
    public bool TryPop(out T? result)
    {
      if (stack.Count > 0)
      {
        result = stack.Pop();
        return true;
      }

      result = default;
      return false;
    }
  }
}
#endif