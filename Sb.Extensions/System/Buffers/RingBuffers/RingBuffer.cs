// https://github.com/Cysharp/ObservableCollections/blob/master/src/ObservableCollections/RingBuffer.cs

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sb.Extensions.System.Collections.Generic;
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释

namespace Sb.Extensions.System.Buffers.RingBuffers;

public class RingBuffer<T> : IList<T>, IReadOnlyList<T>
{
  private T[] _buffer;
  private int _head;
  private int _mask;

  public RingBuffer()
  {
    _buffer = new T[8];
    _head = 0;
    Count = 0;
    _mask = _buffer.Length - 1;
  }

  public RingBuffer(int capacity)
  {
    _buffer = new T[CalculateCapacity(capacity)];
    _head = 0;
    Count = 0;
    _mask = _buffer.Length - 1;
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

    _buffer = array;
    _head = 0;
    Count = i;
    _mask = _buffer.Length - 1;
  }

  public T this[int index]
  {
    get
    {
      var i = (_head + index) & _mask;
      return _buffer[i];
    }
    set
    {
      var i = (_head + index) & _mask;
      _buffer[i] = value;
    }
  }

  public int Count { get; private set; }

  public bool IsReadOnly => false;

  void ICollection<T>.Add(T item)
  {
    AddLast(item);
  }

  public void Clear()
  {
    Array.Clear(_buffer, 0, _buffer.Length);
    _head = 0;
    Count = 0;
  }

  public IEnumerator<T> GetEnumerator()
  {
    if (Count == 0) yield break;

    var start = _head & _mask;
    var end = (_head + Count) & _mask;

    if (end > start)
    {
      // start...end
      for (var i = start; i < end; i++) yield return _buffer[i];
    }
    else
    {
      // start...
      for (var i = start; i < _buffer.Length; i++) yield return _buffer[i];
      // 0...end
      for (var i = 0; i < end; i++) yield return _buffer[i];
    }
  }

  public bool Contains(T item)
  {
    return IndexOf(item) != -1;
  }

  public void CopyTo(T[] array, int arrayIndex)
  {
    var span = GetWrittenSpan();
    var dest = array.AsSpan(arrayIndex);
    span.First.CopyTo(dest);
    span.Second.CopyTo(dest.Slice(span.First.Length));
  }

  public int IndexOf(T item)
  {
    var i = 0;
    foreach (var v in GetWrittenSpan())
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
    return ((IEnumerable<T>)this).GetEnumerator();
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

  public void MoveEnd(int n)
  {
    if (n < 0 || Count + n > _buffer.Length)
      throw new ArgumentOutOfRangeException(nameof(n));
    Count += n;
  }

  public void MoveHead(int n)
  {
    if (n < 0 || n > Count)
      throw new ArgumentOutOfRangeException(nameof(n));
    if (n == 0) return;
    var start = _head & _mask;
    var firstLen = Math.Min(_buffer.Length - start, n);
    _buffer.AsSpan(start, firstLen).Clear();
    if (n > firstLen)
      _buffer.AsSpan(0, n - firstLen).Clear();
    _head = (_head + n) & _mask;
    Count -= n;
  }

  public void AddLast(T item)
  {
    if (Count == _buffer.Length) EnsureCapacity();

    var index = (_head + Count) & _mask;
    _buffer[index] = item;
    Count++;
  }

  public void AddLastRange(ReadOnlySpan<T> items)
  {
    if (items.Length == 0) return;
    if (Count + items.Length > _buffer.Length)
      while (Count + items.Length > _buffer.Length)
        EnsureCapacity();
    var tail = (_head + Count) & _mask;
    var firstLen = Math.Min(_buffer.Length - tail, items.Length);
    items[..firstLen].CopyTo(_buffer.AsSpan(tail, firstLen));
    if (items.Length > firstLen)
      items[firstLen..].CopyTo(_buffer.AsSpan(0, items.Length - firstLen));
    Count += items.Length;
  }

  public void AddFirst(T item)
  {
    if (Count == _buffer.Length) EnsureCapacity();

    _head = (_head - 1) & _mask;
    _buffer[_head] = item;
    Count++;
  }

  public T RemoveLast()
  {
    if (Count == 0) ThrowForEmpty();

    var index = (_head + Count - 1) & _mask;
    var v = _buffer[index];
    Count--;
    return v;
  }

  public void RemoveLast(int n)
  {
    if (n < 0 || n > Count) throw new ArgumentOutOfRangeException();
    if (n == 0) return;
    Count -= n;
  }

  public T RemoveFirst()
  {
    if (Count == 0) ThrowForEmpty();

    var index = _head & _mask;
    var v = _buffer[index];
    _head += 1;
    Count--;
    return v;
  }

  public void RemoveFirst(int n)
  {
    if (n < 0 || n > Count) throw new ArgumentOutOfRangeException();
    if (n == 0) return;
    _head = (_head + n) & _mask;
    Count -= n;
  }

  private void EnsureCapacity()
  {
    var newBuffer = new T[_buffer.Length * 2];

    var i = _head & _mask;
    _buffer.AsSpan(i).CopyTo(newBuffer);

    if (i != 0) _buffer.AsSpan(0, i).CopyTo(newBuffer.AsSpan(_buffer.Length - i));

    _head = 0;
    _buffer = newBuffer;
    _mask = newBuffer.Length - 1;
  }

  public RingBufferSpan<T> GetWrittenSpan()
  {
    if (Count == 0) return new RingBufferSpan<T>(Span<T>.Empty, Span<T>.Empty, 0);

    var start = _head & _mask;
    var end = (_head + Count) & _mask;

    if (end > start)
    {
      var first = _buffer.AsSpan(start, Count);
      var second = Span<T>.Empty;
      return new RingBufferSpan<T>(first, second, Count);
    }
    else
    {
      var first = _buffer.AsSpan(start, _buffer.Length - start);
      var second = _buffer.AsSpan(0, end);
      return new RingBufferSpan<T>(first, second, Count);
    }
  }

  public RingBufferSpan<T> GetWritableSpan()
  {
    int writable = _buffer.Length - Count;
    if (writable == 0) return new RingBufferSpan<T>(Span<T>.Empty, Span<T>.Empty, 0);

    var tail = (_head + Count) & _mask;
    if (tail + writable <= _buffer.Length)
    {
      var first = _buffer.AsSpan(tail, writable);
      var second = Span<T>.Empty;
      return new RingBufferSpan<T>(first, second, writable);
    }
    else
    {
      var firstLen = _buffer.Length - tail;
      var first = _buffer.AsSpan(tail, firstLen);
      var second = _buffer.AsSpan(0, writable - firstLen);
      return new RingBufferSpan<T>(first, second, writable);
    }
  }

  public IEnumerable<T> Reverse()
  {
    if (Count == 0) yield break;

    var start = _head & _mask;
    var end = (_head + Count) & _mask;

    if (end > start)
    {
      // end...start
      for (var i = end - 1; i >= start; i--) yield return _buffer[i];
    }
    else
    {
      // end...0
      for (var i = end - 1; i >= 0; i--) yield return _buffer[i];

      // ...start
      for (var i = _buffer.Length - 1; i >= start; i--) yield return _buffer[i];
    }
  }

  public T[] ToArray()
  {
    var result = new T[Count];
    var span = GetWrittenSpan();
    span.First.CopyTo(result.AsSpan(0, span.First.Length));
    span.Second.CopyTo(result.AsSpan(span.First.Length, span.Second.Length));
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
  
  public bool IsEmpty => this.Length == 0;

  internal RingBufferSpan(Span<T> first, Span<T> second, int length)
  {
    First = first;
    Second = second;
    Length = length;
  }
  
  public void CopyTo(Span<T> destination)
  {
    if (destination.Length < Length)
      throw new ArgumentException("目标空间不足");
    if (!First.IsEmpty)
    {
      First.CopyTo(destination);
    }

    if (!Second.IsEmpty)
    {
      Second.CopyTo(destination[First.Length..]);
    }
  }

  public void CopyFrom(ReadOnlySpan<T> source)
  {
    if (source.Length < Length)
      throw new ArgumentException("源数据不足");
    if (First.Length >= Length)
    {
      source[..Length].CopyTo(First);
    }
    else
    {
      source[..First.Length].CopyTo(First);
      source.Slice(First.Length, Second.Length).CopyTo(Second);
    }
  }
  
  public T[] ToArray()
  {
    if (Length == 0)
    {
      return [];
    }

    var result = new T[Length];
    CopyTo(result);
    return result;
  }

  public RingBufferSpan<T> Slice(int start)
  {
    var length = Length - start;
    
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