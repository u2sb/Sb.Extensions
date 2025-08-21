#if NETSTANDARD2_0
using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace System.Collections.Generic;

/// <summary>
///   Stack 拓展
/// </summary>
public static class StackExtensions
{
  /// <summary>
  ///   尝试查看栈顶元素而不移除它
  /// </summary>
  /// <param name="stack">栈</param>
  /// <param name="result">结果</param>
  /// <typeparam name="T">元素类型</typeparam>
  /// <returns>是否成功</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool TryPeek<T>(this Stack<T> stack, out T? result)
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
  ///   尝试弹出栈顶元素
  /// </summary>
  /// <param name="stack">栈</param>
  /// <param name="result">结果</param>
  /// <typeparam name="T">元素类型</typeparam>
  /// <returns>是否成功</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool TryPop<T>(this Stack<T> stack, out T? result)
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
#endif