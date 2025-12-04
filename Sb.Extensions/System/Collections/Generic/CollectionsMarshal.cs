using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#if !NET7_0_OR_GREATER

#pragma warning disable CS0649
#pragma warning disable CS8618
#pragma warning disable CS8619

namespace Sb.Extensions.System.Collections.Generic;

/// <summary>
/// </summary>
public static class CollectionsMarshal
{
  /// <summary>
  /// </summary>
  /// <param name="list"></param>
  /// <typeparam name="T"></typeparam>
  extension<T>(List<T>? list)
  {
    /// <summary>
    ///   similar as AsSpan but modify size to create fixed-size span.
    /// </summary>
    public Span<T> AsSpan()
    {
      if (list is null) return default;

      ref var view = ref Unsafe.As<List<T>, ListView<T>>(ref list!);
      return view.Items.AsSpan(0, view.Size);
    }
  }

  /// <summary>
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public sealed class ListView<T>
  {
    /// <summary>
    /// </summary>
    public T[] Items;

    /// <summary>
    /// </summary>
    public int Size;

    /// <summary>
    /// </summary>
    public int Version;
  }
}

#endif