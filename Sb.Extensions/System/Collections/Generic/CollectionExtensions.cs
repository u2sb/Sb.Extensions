using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释

namespace Sb.Extensions.System.Collections.Generic;

public static class CollectionExtensions
{
  private const int ArrayMaxLength = 0X7FFFFFC7;

  extension<TKey, TValue>(KeyValuePair<TKey, TValue> kvp)
  {
    public void Deconstruct(out TKey key, out TValue value)
    {
      key = kvp.Key;
      value = kvp.Value;
    }
  }

  extension<TKey, TValue>(SortedDictionary<TKey, TValue> dict) where TKey : notnull
  {
    public bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
      if (dict.TryGetValue(key, out value)) return dict.Remove(key);
      return false;
    }
  }

  extension<TKey, TValue>(Dictionary<TKey, TValue> dict) where TKey : notnull
  {
    public bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
      if (dict.TryGetValue(key, out value)) return dict.Remove(key);
      return false;
    }
  }

#if !NET6_0_OR_GREATER
  extension<T>(IEnumerable<T> source)
  {
    public bool TryGetNonEnumeratedCount(out int count)
    {
      if (source is ICollection<T> collection)
      {
        count = collection.Count;
        return true;
      }

      if (source is IReadOnlyCollection<T> rCollection)
      {
        count = rCollection.Count;
        return true;
      }

      count = 0;
      return false;
    }
  }

#endif

#if !NET8_0_OR_GREATER
#pragma warning disable CS0436

  // CollectionExtensions.AddRange
  extension<T>(List<T> list)
  {
    public void AddRange(ReadOnlySpan<T> source)
    {
      if (!source.IsEmpty)
      {
        ref var view = ref Unsafe.As<List<T>, CollectionsMarshal.ListView<T>>(ref list!);

        if (view.Items.Length - view.Size < source.Length) Grow(ref view, checked(view.Size + source.Length));

        source.CopyTo(view.Items.AsSpan(view.Size));
        view.Size += source.Length;
        view.Version++;
      }
    }

    public void InsertRange(int index, ReadOnlySpan<T> source)
    {
      if (!source.IsEmpty)
      {
        ref var view = ref Unsafe.As<List<T>, CollectionsMarshal.ListView<T>>(ref list!);

        if (view.Items.Length - view.Size < source.Length) Grow(ref view, checked(view.Size + source.Length));

        if (index < view.Size) Array.Copy(view.Items, index, view.Items, index + source.Length, view.Size - index);

        source.CopyTo(view.Items.AsSpan(index));
        view.Size += source.Length;
        view.Version++;
      }
    }
  }

  // CollectionExtensions.InsertRange

  private static void Grow<T>(ref CollectionsMarshal.ListView<T> list, int capacity)
  {
    SetCapacity(ref list, GetNewCapacity(ref list, capacity));
  }

  private static void SetCapacity<T>(ref CollectionsMarshal.ListView<T> list, int value)
  {
    if (value != list.Items.Length)
    {
      if (value > 0)
      {
        var newItems = new T[value];
        if (list.Size > 0) Array.Copy(list.Items, newItems, list.Size);
        list.Items = newItems;
      }
      else
      {
        list.Items = [];
      }
    }
  }

  private static int GetNewCapacity<T>(ref CollectionsMarshal.ListView<T> list, int capacity)
  {
    var newCapacity = list.Items.Length == 0 ? 4 : 2 * list.Items.Length;

    if ((uint)newCapacity > ArrayMaxLength) newCapacity = ArrayMaxLength;

    if (newCapacity < capacity) newCapacity = capacity;

    return newCapacity;
  }

#pragma warning restore CS0436
#endif
}

#if !NET5_0_OR_GREATER
public interface IReadOnlySet<out T> : IEnumerable<T>, IReadOnlyCollection<T>
{
}
#endif