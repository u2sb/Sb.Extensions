#if NETSTANDARD2_0
using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace System.Collections.Generic;

/// <summary>
///   Queue 拓展
/// </summary>
public static class QueueExtensions
{
  /// <summary>
  ///   尝试查看队列头部元素而不移除它
  /// </summary>
  /// <param name="queue">队列</param>
  /// <param name="result">结果</param>
  /// <typeparam name="T">元素类型</typeparam>
  /// <returns>是否成功</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool TryPeek<T>(this Queue<T> queue, out T? result)
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
  ///   尝试出队一个元素
  /// </summary>
  /// <param name="queue">队列</param>
  /// <param name="result">结果</param>
  /// <typeparam name="T">元素类型</typeparam>
  /// <returns>是否成功</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool TryDequeue<T>(this Queue<T> queue, out T? result)
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
#endif