// https://github.com/Cysharp/ObservableCollections/blob/master/src/ObservableCollections/RingBuffer.cs

#if NET8_0_OR_GREATER
using CommunityToolkit.HighPerformance;
#else
using Sb.Extensions.System.Collections.Generic;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释

namespace Sb.Extensions.System.Buffers.RingBuffers;

public class RingBuffer<T> : IList<T>, IReadOnlyList<T>
{
  protected T[] Buffer;
  protected int Head;
  protected int Mask;

  public RingBuffer()
  {
    Buffer = new T[8];
    Head = 0;
    Count = 0;
    Mask = Buffer.Length - 1;
  }

  public RingBuffer(int capacity)
  {
    Buffer = new T[CalculateCapacity(capacity)];
    Head = 0;
    Count = 0;
    Mask = Buffer.Length - 1;
  }

  public RingBuffer(IEnumerable<T> collection)
  {
    var array = collection.TryGetNonEnumeratedCount(out var count)
      ? new T[CalculateCapacity(count)]
      : new T[8];
    var i = 0;
    foreach (var item in collection)
    {
      if (i == array.Length) Array.Resize(ref array, i * 2);
      array[i++] = item;
    }

    Buffer = array;
    Head = 0;
    Count = i;
    Mask = Buffer.Length - 1;
  }

  public RingBufferSpan<T> WrittenSpan
  {
    get
    {
      if (Count == 0) return new RingBufferSpan<T>(Span<T>.Empty, Span<T>.Empty, 0);

      var start = Head & Mask;
      var end = (Head + Count) & Mask;

      if (end > start)
      {
        var first = Buffer.AsSpan(start, Count);
        var second = Span<T>.Empty;
        return new RingBufferSpan<T>(first, second, Count);
      }
      else
      {
        var first = Buffer.AsSpan(start, Buffer.Length - start);
        var second = Buffer.AsSpan(0, end);
        return new RingBufferSpan<T>(first, second, Count);
      }
    }
  }

  public T this[int index]
  {
    get
    {
      if (index < 0 || index >= Count)
        throw new IndexOutOfRangeException($"Index {index} is out of range [0, {Count - 1}]");

      var i = (Head + index) & Mask;
      return Buffer[i];
    }
    set
    {
      if (index < 0 || index >= Count)
        throw new IndexOutOfRangeException($"Index {index} is out of range [0, {Count - 1}]");

      var i = (Head + index) & Mask;
      Buffer[i] = value;
    }
  }

  public int Count { get; protected set; }

  public bool IsReadOnly => false;

  void ICollection<T>.Add(T item)
  {
    AddLast(item);
  }

  public void Clear()
  {
    Head = 0;
    Count = 0;
  }

  public IEnumerator<T> GetEnumerator()
  {
    if (Count == 0) yield break;

    var start = Head;
    var end = (Head + Count) & Mask;

    if (end > start)
    {
      // start...end
      for (var i = start; i < end; i++) yield return Buffer[i];
    }
    else
    {
      // start...
      for (var i = start; i < Buffer.Length; i++) yield return Buffer[i];
      // 0...end
      for (var i = 0; i < end; i++) yield return Buffer[i];
    }
  }

  public bool Contains(T item)
  {
    return IndexOf(item) != -1;
  }

  public void CopyTo(T[] array, int arrayIndex)
  {
    var span = WrittenSpan;
    var dest = array.AsSpan(arrayIndex);
    span.CopyTo(dest);
  }

  public int IndexOf(T item)
  {
    var i = 0;
    var span = WrittenSpan;
    foreach (var v in span)
    {
      if (EqualityComparer<T>.Default.Equals(item, v)) return i;
      i++;
    }

    return -1;
  }

  void IList<T>.Insert(int index, T item)
  {
    throw new NotSupportedException();
  }

  bool ICollection<T>.Remove(T item)
  {
    throw new NotSupportedException();
  }

  void IList<T>.RemoveAt(int index)
  {
    throw new NotSupportedException();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  protected RingBufferSpan<T> GetWritableSpan()
  {
    var writable = Buffer.Length - Count;
    if (writable == 0) return new RingBufferSpan<T>(Span<T>.Empty, Span<T>.Empty, 0);

    var tail = (Head + Count) & Mask;
    if (tail + writable <= Buffer.Length)
    {
      var first = Buffer.AsSpan(tail, writable);
      var second = Span<T>.Empty;
      return new RingBufferSpan<T>(first, second, writable);
    }
    else
    {
      var firstLen = Buffer.Length - tail;
      var first = Buffer.AsSpan(tail, firstLen);
      var second = Buffer.AsSpan(0, writable - firstLen);
      return new RingBufferSpan<T>(first, second, writable);
    }
  }

  public void Add(T item)
  {
    AddLast(item);
  }

  public void AddRange(ReadOnlySpan<T> collection)
  {
    AddLastRange(collection);
  }

  public void AddRange(List<T> collection)
  {
    AddLastRange(collection.AsSpan());
  }

  public void AddRange(IEnumerable<T> collection)
  {
    AddLastRange(collection.ToArray());
  }

  private static int CalculateCapacity(int size)
  {
    size--;
    size |= size >> 1;
    size |= size >> 2;
    size |= size >> 4;
    size |= size >> 8;
    size |= size >> 16;
    size += 1;

    if (size < 8) size = 8;
    return size;
  }

  public virtual void AddLast(T item)
  {
    if (Count == Buffer.Length) EnsureCapacity();

    var index = (Head + Count) & Mask;
    Buffer[index] = item;
    Count++;
  }

  public virtual void AddLastRange(ReadOnlySpan<T> items)
  {
    if (items.Length == 0) return;
    if (Count + items.Length > Buffer.Length)
      while (Count + items.Length > Buffer.Length)
        EnsureCapacity();
    var tail = (Head + Count) & Mask;
    var firstLen = Math.Min(Buffer.Length - tail, items.Length);
    items[..firstLen].CopyTo(Buffer.AsSpan(tail, firstLen));
    if (items.Length > firstLen)
      items[firstLen..].CopyTo(Buffer.AsSpan(0, items.Length - firstLen));
    Count += items.Length;
  }

  public virtual void AddFirst(T item)
  {
    if (Count == Buffer.Length) EnsureCapacity();

    Head = (Head - 1) & Mask;
    Buffer[Head] = item;
    Count++;
  }

  public T RemoveLast()
  {
    if (Count == 0) ThrowForEmpty();

    var index = (Head + Count - 1) & Mask;
    var v = Buffer[index];
    Buffer[index] = default!;
    Count--;
    return v;
  }

  public void RemoveLast(int n)
  {
    if (n < 0) throw new ArgumentOutOfRangeException(nameof(n), "n cannot be negative.");
    if (n > Count) throw new ArgumentOutOfRangeException(nameof(n), $"n ({n}) cannot be greater than Count ({Count}).");
    if (n == 0) return;

    // Clear removed elements to prevent memory leaks
    var start = (Head + Count - n) & Mask;
    var end = (Head + Count) & Mask;

    if (start < end)
    {
      // Single contiguous segment
      Array.Clear(Buffer, start, n);
    }
    else
    {
      // Wrapped around
      var firstSegmentLength = Buffer.Length - start;
      Array.Clear(Buffer, start, firstSegmentLength);
      var secondSegmentLength = n - firstSegmentLength;
      if (secondSegmentLength > 0) Array.Clear(Buffer, 0, secondSegmentLength);
    }

    Count -= n;
  }

  public T RemoveFirst()
  {
    if (Count == 0) ThrowForEmpty();

    var index = Head & Mask;
    var v = Buffer[index];
    Buffer[index] = default!;
    Head = (Head + 1) & Mask;
    Count--;
    return v;
  }

  public void RemoveFirst(int n)
  {
    if (n < 0) throw new ArgumentOutOfRangeException(nameof(n), "n cannot be negative.");
    if (n > Count) throw new ArgumentOutOfRangeException(nameof(n), $"n ({n}) cannot be greater than Count ({Count}).");
    if (n == 0) return;

    // Clear removed elements to prevent memory leaks
    var start = Head & Mask;

    if (start + n <= Buffer.Length)
    {
      // Single contiguous segment
      Array.Clear(Buffer, start, n);
    }
    else
    {
      // Wrapped around
      var firstSegmentLength = Buffer.Length - start;
      Array.Clear(Buffer, start, firstSegmentLength);
      var secondSegmentLength = n - firstSegmentLength;
      if (secondSegmentLength > 0) Array.Clear(Buffer, 0, secondSegmentLength);
    }

    Head = (Head + n) & Mask;
    Count -= n;
  }

  private void EnsureCapacity()
  {
    var newBuffer = new T[Buffer.Length * 2];

    var i = Head & Mask;
    Buffer.AsSpan(i).CopyTo(newBuffer);

    if (i != 0) Buffer.AsSpan(0, i).CopyTo(newBuffer.AsSpan(Buffer.Length - i));

    Head = 0;
    Buffer = newBuffer;
    Mask = newBuffer.Length - 1;
  }

  public IEnumerable<T> Reverse()
  {
    if (Count == 0) yield break;

    var start = Head;
    var end = (Head + Count) & Mask;

    if (end > start)
    {
      // end...start
      for (var i = end - 1; i >= start; i--) yield return Buffer[i];
    }
    else
    {
      // end...0
      for (var i = end - 1; i >= 0; i--) yield return Buffer[i];

      // ...start
      for (var i = Buffer.Length - 1; i >= start; i--) yield return Buffer[i];
    }
  }

  public T[] ToArray()
  {
    var result = new T[Count];
    CopyTo(result, 0);
    return result;
  }

  public int BinarySearch(T item)
  {
    return BinarySearch(item, Comparer<T>.Default);
  }

  public int BinarySearch(T item, IComparer<T> comparer)
  {
    var lo = 0;
    var hi = Count - 1;

    while (lo <= hi)
    {
      var mid = (int)(((uint)hi + (uint)lo) >> 1);
      var found = comparer.Compare(this[mid], item);

      if (found == 0) return mid;
      if (found < 0)
        lo = mid + 1;
      else
        hi = mid - 1;
    }

    return ~lo;
  }

  private static void ThrowForEmpty()
  {
    throw new InvalidOperationException("RingBuffer is empty.");
  }
}

public readonly ref struct RingBufferSpan<T>
{
  public readonly Span<T> First;
  public readonly Span<T> Second;
  public readonly int Length;

  public int Count => Length;

  public bool IsEmpty => Length == 0;

  internal RingBufferSpan(Span<T> first, Span<T> second, int length)
  {
    First = first;
    Second = second;
    Length = length;
  }

  public ref T this[int index]
  {
    get
    {
      if (index < 0 || index >= Length) throw new ArgumentOutOfRangeException(nameof(index));

      if (index < First.Length) return ref First[index];

      return ref Second[index - First.Length];
    }
  }

  public void CopyTo(Span<T> destination)
  {
    if (destination.Length < Length)
      throw new ArgumentException("Destination span is too short.", nameof(destination));

    if (!First.IsEmpty) First.CopyTo(destination);

    if (!Second.IsEmpty) Second.CopyTo(destination[First.Length..]);
  }

  public void CopyFrom(ReadOnlySpan<T> source)
  {
    if (source.Length > Length)
      throw new ArgumentException("Source span is too long.", nameof(source));

    if (First.Length >= source.Length)
    {
      source.CopyTo(First);
    }
    else
    {
      source[..First.Length].CopyTo(First);
      source.Slice(First.Length, source.Length - First.Length).CopyTo(Second);
    }
  }

  public T[] ToArray()
  {
    if (Length == 0) return [];

    var result = new T[Length];
    CopyTo(result);
    return result;
  }

  public RingBufferSpan<T> Slice(int start)
  {
    var length = Length - start;
    return Slice(start, length);
  }

  public RingBufferSpan<T> Slice(int start, int length)
  {
    if (start < 0 || length < 0 || start + length > Length)
      throw new ArgumentOutOfRangeException();

    if (start < First.Length)
    {
      var firstSliceLen = Math.Min(length, First.Length - start);
      var first = First.Slice(start, firstSliceLen);
      var second = length > firstSliceLen ? Second[..(length - firstSliceLen)] : Span<T>.Empty;
      return new RingBufferSpan<T>(first, second, length);
    }
    else
    {
      var secondStart = start - First.Length;
      var first = Second.Slice(secondStart, length);
      return new RingBufferSpan<T>(first, Span<T>.Empty, length);
    }
  }

  public Enumerator GetEnumerator()
  {
    return new Enumerator(this);
  }

  public ref struct Enumerator
  {
    private Span<T>.Enumerator firstEnumerator;
    private Span<T>.Enumerator secondEnumerator;
    private bool useFirst;

    public Enumerator(RingBufferSpan<T> span)
    {
      firstEnumerator = span.First.GetEnumerator();
      secondEnumerator = span.Second.GetEnumerator();
      useFirst = true;
    }

    public bool MoveNext()
    {
      if (useFirst)
      {
        if (firstEnumerator.MoveNext()) return true;

        useFirst = false;
      }

      if (secondEnumerator.MoveNext()) return true;
      return false;
    }

    public T Current
    {
      get
      {
        if (useFirst) return firstEnumerator.Current;

        return secondEnumerator.Current;
      }
    }
  }
}