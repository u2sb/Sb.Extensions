#if NETSTANDARD2_0
using System.Runtime.CompilerServices;

#else
using System.Runtime.InteropServices;
#endif


// 命名空间不要改
// ReSharper disable once CheckNamespace
namespace System;

/// <summary>
/// </summary>
public static class SpanExtension
{
  /// <summary>
  ///   创建 Span
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="source"></param>
  /// <param name="length"></param>
  /// <returns></returns>
  public static Span<T> CreateSpan<T>(scoped ref T source, int length) where T : unmanaged
  {
#if NETSTANDARD2_0
    unsafe
    {
      return new Span<T>(Unsafe.AsPointer(ref source), length);
    }
#else
    return MemoryMarshal.CreateSpan(ref source, length);
#endif
  }

  /// <summary>
  ///   创建 ReadOnlySpan
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="source"></param>
  /// <param name="length"></param>
  /// <returns></returns>
  public static ReadOnlySpan<T> CreateReadOnlySpan<T>(scoped ref T source, int length) where T : unmanaged
  {
#if NETSTANDARD2_0
    unsafe
    {
      return new ReadOnlySpan<T>(Unsafe.AsPointer(ref source), length);
    }
#else
    return MemoryMarshal.CreateReadOnlySpan(ref source, length);
#endif
  }
}