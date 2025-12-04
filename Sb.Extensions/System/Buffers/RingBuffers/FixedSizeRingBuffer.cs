using System;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释

namespace Sb.Extensions.System.Buffers.RingBuffers;

public class FixedSizeRingBuffer<T> : IReadOnlyList<T>
{
  private readonly RingBuffer<T> buffer;

  public FixedSizeRingBuffer(int capacity)
  {
    if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));
    Capacity = capacity;
    buffer = new RingBuffer<T>(capacity);
  }

  public int Capacity { get; }

  public int Count => buffer.Count;

  public T this[int index] => buffer[index];

  public IEnumerator<T> GetEnumerator()
  {
    return buffer.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  public void Add(T item)
  {
    if (buffer.Count == Capacity)
      buffer.RemoveFirst();
    buffer.AddLast(item);
  }

  public void AddRange(ReadOnlySpan<T> items)
  {
    foreach (var item in items)
      Add(item);
  }

  public T RemoveFirst()
  {
    return buffer.RemoveFirst();
  }

  public void RemoveFirst(int count)
  {
    buffer.RemoveFirst(count);
  }

  public T RemoveLast()
  {
    return buffer.RemoveLast();
  }

  public void RemoveLast(int count)
  {
    buffer.RemoveLast(count);
  }

  public T[] ToArray()
  {
    return buffer.ToArray();
  }
  
  public RingBufferSpan<T> GetSpan()
  {
    return buffer.GetSpan();
  }
}