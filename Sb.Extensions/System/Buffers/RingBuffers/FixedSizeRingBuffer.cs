using System;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释

namespace Sb.Extensions.System.Buffers.RingBuffers;

public class FixedSizeRingBuffer<T> : IReadOnlyList<T>
{
  private readonly RingBuffer<T> _buffer;

  public FixedSizeRingBuffer(int capacity)
  {
    if (capacity <= 0)
    {
      throw new ArgumentOutOfRangeException(nameof(capacity));
    }

    Capacity = capacity;
    _buffer = new RingBuffer<T>(capacity);
  }

  public int Capacity { get; }

  public RingBufferSpan<T> WrittenSpan => _buffer.WrittenSpan;

  public RingBufferSpan<T> WritableSpan => _buffer.WritableSpan;

  public int Count => _buffer.Count;

  public T this[int index] => _buffer[index];

  public IEnumerator<T> GetEnumerator()
  {
    return _buffer.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  public void MoveEnd(int n)
  {
    _buffer.MoveEnd(n);
  }

  public void MoveHead(int n)
  {
    _buffer.MoveHead(n);
  }

  public void Add(T item)
  {
    if (_buffer.Count == Capacity)
    {
      _buffer.RemoveFirst();
    }

    _buffer.AddLast(item);
  }

  public void AddRange(ReadOnlySpan<T> items)
  {
    foreach (var item in items)
    {
      Add(item);
    }
  }

  public T RemoveFirst()
  {
    return _buffer.RemoveFirst();
  }

  public void RemoveFirst(int count)
  {
    _buffer.RemoveFirst(count);
  }

  public T RemoveLast()
  {
    return _buffer.RemoveLast();
  }

  public void RemoveLast(int count)
  {
    _buffer.RemoveLast(count);
  }

  public T[] ToArray()
  {
    return _buffer.ToArray();
  }
}