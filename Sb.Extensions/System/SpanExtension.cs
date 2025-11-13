using System;
using System.Runtime.CompilerServices;

#if NETSTANDARD2_0

#else
using System.Runtime.InteropServices;
#endif

namespace Sb.Extensions.System;

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
  public static Span<T> CreateSpan<T>(scoped ref T source, int length = 1) where T : unmanaged
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
  public static ReadOnlySpan<T> CreateReadOnlySpan<T>(scoped in T source, int length = 1) where T : unmanaged
  {
#if NETSTANDARD2_0
    unsafe
    {
      return new ReadOnlySpan<T>(Unsafe.AsPointer(ref Unsafe.AsRef(in source)), length);
    }
#else
    return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in source), length);
#endif
  }
}