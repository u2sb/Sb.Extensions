using System;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释

namespace Sb.Extensions.System.Buffers.RingBuffers;

public class FixedSizeRingBuffer<T> : IList<T>, IReadOnlyList<T>
{
  private readonly RingBuffer<T> _buffer;

  public FixedSizeRingBuffer(int capacity)
  {
    if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));

    Capacity = capacity;
    _buffer = new RingBuffer<T>(capacity);
  }

  public int Capacity { get; }

  public RingBufferSpan<T> WrittenSpan => _buffer.WrittenSpan;

  public RingBufferSpan<T> WritableSpan => _buffer.WritableSpan;

  public int Count => _buffer.Count;
  public bool IsReadOnly => _buffer.IsReadOnly;

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

  public T this[int index]
  {
    get => _buffer[index];
    set => _buffer[index] = value;
  }

  public IEnumerator<T> GetEnumerator()
  {
    return _buffer.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  public void Add(T item)
  {
    if (_buffer.Count == Capacity) _buffer.RemoveFirst();

    _buffer.AddLast(item);
  }

  public void Clear()
  {
    _buffer.Clear();
  }

  public bool Contains(T item)
  {
    return _buffer.Contains(item);
  }

  public void CopyTo(T[] array, int arrayIndex)
  {
    _buffer.CopyTo(array, arrayIndex);
  }

  public int IndexOf(T item)
  {
    return _buffer.IndexOf(item);
  }

  public void MoveEnd(int n)
  {
    _buffer.MoveEnd(n);
  }

  public void MoveHead(int n)
  {
    _buffer.MoveHead(n);
  }

  public void AddLast(T item)
  {
    // 在容量已满时，按尾部插入的语义驱逐头部元素（和 Add 一致）
    if (_buffer.Count == Capacity) _buffer.RemoveFirst();

    _buffer.AddLast(item);
  }

  public void AddFirst(T item)
  {
    // 在容量已满时，插入头部应驱逐尾部元素以保持固定容量
    if (_buffer.Count == Capacity) _buffer.RemoveLast();

    _buffer.AddFirst(item);
  }

  public void AddRange(ReadOnlySpan<T> items)
  {
    foreach (var item in items) Add(item);
  }

  public void AddRange(List<T>? collection)
  {
    if (collection is not { Count: > 0 }) return;
    foreach (var item in collection) Add(item);
  }

  public void AddRange(IEnumerable<T>? collection)
  {
    if (collection == null) return;
    foreach (var item in collection) Add(item);
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

  public IEnumerable<T> Reverse()
  {
    return _buffer.Reverse();
  }

  public int BinarySearch(T item)
  {
    return _buffer.BinarySearch(item);
  }

  public int BinarySearch(T item, IComparer<T> comparer)
  {
    return _buffer.BinarySearch(item, comparer);
  }
}