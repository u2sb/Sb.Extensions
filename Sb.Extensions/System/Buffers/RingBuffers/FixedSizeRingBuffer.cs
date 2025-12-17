using System;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sb.Extensions.System.Buffers.RingBuffers;

/// <summary>
///   环形缓冲区，固定大小，支持覆盖写入
/// </summary>
/// <typeparam name="T"></typeparam>
public class FixedSizeRingBuffer<T> : IEnumerable<T> where T : unmanaged
{
  private readonly int _mask;
  private T[] _buffer;
  private int _head;

  /// <summary>
  ///   环形缓冲区
  /// </summary>
  /// <param name="capacity"></param>
  public FixedSizeRingBuffer(int capacity = 1024)
  {
    _buffer = new T[CalculateCapacity(capacity)];
    _head = 0;
    Count = 0;
    _mask = _buffer.Length - 1;
  }

  /// <summary>
  ///   长度
  /// </summary>
  public int Count { get; private set; }

  /// <summary>
  ///   可以存储的最大元素数量
  /// </summary>
  public int Capacity => _buffer.Length;

  /// <summary>
  ///   是否为空
  /// </summary>
  public bool IsEmpty => Count == 0;

  public IEnumerator<T> GetEnumerator()
  {
    for (var i = 0; i < Count; i++)
    {
      yield return this[i];
    }
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  #region 核心方法

  /// <summary>
  ///   在末尾添加单个元素
  /// </summary>
  public void AddLast(T item)
  {
    var tail = (_head + Count) & _mask;
    _buffer[tail] = item;

    if (Count < Capacity)
    {
      // 未写满
      Count++;
    }
    else
    {
      // 已写满，覆盖最旧的数据
      _head = (_head + 1) & _mask;
    }
  }

  /// <summary>
  ///   在末尾添加多个元素
  /// </summary>
  public void AddLastRange(ReadOnlySpan<T> items)
  {
    if (items.IsEmpty)
    {
      return;
    }

    // 如果输入数据长度超过缓冲区容量，只取最后 Capacity 个元素
    var dataToWrite = items.Length > Capacity
      ? items[^Capacity..]
      : items;

    var availableSpace = Capacity - Count;

    if (dataToWrite.Length <= availableSpace)
    {
      // 缓冲区有足够空间，直接写入
      WriteToTail(dataToWrite);
      Count += dataToWrite.Length;
    }
    else
    {
      // 缓冲区空间不足，需要覆盖旧数据
      var overflowCount = dataToWrite.Length - availableSpace;
      WriteToTail(dataToWrite);
      _head = (_head + overflowCount) & _mask;
      Count = Capacity;
    }
  }

  /// <summary>
  ///   将数据写入缓冲区末尾（不更新 Count 和 Head）
  /// </summary>
  private void WriteToTail(ReadOnlySpan<T> items)
  {
    var tail = (_head + Count) & _mask;
    var spaceBeforeWrap = _buffer.Length - tail;
    var itemCount = items.Length;

    if (itemCount <= spaceBeforeWrap)
    {
      // 数据可以连续写入
      items.CopyTo(new Span<T>(_buffer, tail, itemCount));
    }
    else
    {
      // 数据需要分两部分写入
      var firstPart = items[..spaceBeforeWrap];
      var secondPart = items[spaceBeforeWrap..];

      firstPart.CopyTo(_buffer.AsSpan(tail, spaceBeforeWrap));
      secondPart.CopyTo(_buffer.AsSpan(0, secondPart.Length));
    }
  }

  /// <summary>
  ///   获取已写入数据的Span视图
  /// </summary>
  public RingBufferSpan<T> WrittenSpan
  {
    get
    {
      if (Count == 0)
      {
        return new RingBufferSpan<T>(Span<T>.Empty, Span<T>.Empty, 0);
      }

      var tail = (_head + Count) & _mask;

      // 判断是否绕圈：当 tail <= Head 且 Count == Capacity 时是特殊情况
      // 正常情况：tail > Head 表示未绕圈
      // 绕圈情况：tail <= Head
      if (tail > _head)
      {
        // 数据连续，未绕圈
        var first = new Span<T>(_buffer, _head, Count);
        return new RingBufferSpan<T>(first, Span<T>.Empty, Count);
      }

      if (tail == _head && Count == Capacity)
      {
        // 特殊情况：缓冲区已满，数据绕了一圈回到同一位置
        var first = new Span<T>(_buffer, _head, _buffer.Length - _head);
        var second = new Span<T>(_buffer, 0, _head);
        return new RingBufferSpan<T>(first, second, Count);
      }
      else
      {
        // 数据被分割成两部分（绕圈了）
        var firstLength = _buffer.Length - _head;
        var first = new Span<T>(_buffer, _head, firstLength);
        var second = new Span<T>(_buffer, 0, tail);
        return new RingBufferSpan<T>(first, second, Count);
      }
    }
  }

  /// <summary>
  ///   获取指定索引的元素引用
  /// </summary>
  public ref T this[int index]
  {
    get
    {
      if (index < 0 || index >= Count)
      {
        throw new ArgumentOutOfRangeException(nameof(index));
      }

      var actualIndex = (_head + index) & _mask;
      return ref _buffer[actualIndex];
    }
  }

  /// <summary>
  ///   清空缓冲区
  /// </summary>
  public void Clear()
  {
    _head = 0;
    Count = 0;
  }

  /// <summary>
  ///   查找元素首次出现的索引
  /// </summary>
  public int IndexOf(T item)
  {
    for (var i = 0; i < Count; i++)
    {
      if (this[i].Equals(item))
      {
        return i;
      }
    }

    return -1;
  }

  /// <summary>
  ///   检查缓冲区是否包含指定元素
  /// </summary>
  public bool Contains(T item)
  {
    return IndexOf(item) >= 0;
  }

  /// <summary>
  ///   在开头添加单个元素
  /// </summary>
  public void AddFirst(T item)
  {
    if (Count == 0)
    {
      _buffer[0] = item;
      _head = 0;
      Count = 1;
    }
    else
    {
      _head = (_head - 1) & _mask;
      _buffer[_head] = item;

      if (Count < Capacity)
      {
        Count++;
      }
    }
  }

  /// <summary>
  ///   移除并返回末尾的元素
  /// </summary>
  public T RemoveLast()
  {
    if (Count == 0)
    {
      ThrowForEmpty();
    }

    var tail = (_head + Count - 1) & _mask;
    var item = _buffer[tail];
    Count--;

    return item;
  }

  /// <summary>
  ///   移除末尾的n个元素
  /// </summary>
  public void RemoveLast(int n)
  {
    if (n < 0 || n > Count)
    {
      throw new ArgumentOutOfRangeException(nameof(n));
    }

    Count -= n;
  }

  /// <summary>
  ///   移除并返回开头的元素
  /// </summary>
  public T RemoveFirst()
  {
    if (Count == 0)
    {
      ThrowForEmpty();
    }

    var item = _buffer[_head];
    _head = (_head + 1) & _mask;
    Count--;

    return item;
  }

  /// <summary>
  ///   移除开头的n个元素
  /// </summary>
  public void RemoveFirst(int n)
  {
    if (n < 0 || n > Count)
    {
      throw new ArgumentOutOfRangeException(nameof(n));
    }

    _head = (_head + n) & _mask;
    Count -= n;
  }

  /// <summary>
  ///   将缓冲区内容转换为数组
  /// </summary>
  public T[] ToArray()
  {
    if (Count == 0)
    {
      return [];
    }

    var result = new T[Count];
    var span = WrittenSpan;
    span.CopyTo(result);

    return result;
  }

  #endregion

  #region 辅助方法

  private static int CalculateCapacity(int size)
  {
    size--;
    size |= size >> 1;
    size |= size >> 2;
    size |= size >> 4;
    size |= size >> 8;
    size |= size >> 16;
    size += 1;

    if (size < 8)
    {
      size = 8;
    }

    return size;
  }

  private static void ThrowForEmpty()
  {
    throw new InvalidOperationException("RingBuffer is empty.");
  }

  #endregion
}

/// <summary>
///   缓冲区的Span视图，可能由两个不连续的Span组成
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly ref struct RingBufferSpan<T> where T : unmanaged
{
  public Span<T> First { get; }
  public Span<T> Second { get; }

  public int Length { get; }

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
      if (index < 0 || index >= Length)
      {
        throw new ArgumentOutOfRangeException(nameof(index));
      }

      if (index < First.Length)
      {
        return ref First[index];
      }

      return ref Second[index - First.Length];
    }
  }

  public void CopyTo(Span<T> destination)
  {
    if (destination.Length < Length)
    {
      throw new ArgumentException("Destination span is too short.", nameof(destination));
    }

    if (!First.IsEmpty)
    {
      First.CopyTo(destination);
    }

    if (!Second.IsEmpty)
    {
      Second.CopyTo(destination.Slice(First.Length, destination.Length - First.Length));
    }
  }

  public void CopyFrom(ReadOnlySpan<T> source)
  {
    if (source.Length > Length)
    {
      throw new ArgumentException("Source span is too long.", nameof(source));
    }

    if (First.Length >= source.Length)
    {
      source.CopyTo(First);
    }
    else
    {
      source.Slice(0, First.Length).CopyTo(First);
      source.Slice(First.Length, source.Length - First.Length).CopyTo(Second);
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
    return Slice(start, length);
  }

  public RingBufferSpan<T> Slice(int start, int length)
  {
    if (start < 0 || length < 0 || start + length > Length)
    {
      throw new ArgumentOutOfRangeException();
    }

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

  public ref struct Enumerator(RingBufferSpan<T> span)
  {
    private Span<T>.Enumerator _firstEnumerator = span.First.GetEnumerator();
    private Span<T>.Enumerator _secondEnumerator = span.Second.GetEnumerator();
    private bool _useFirst = true;

    public bool MoveNext()
    {
      if (_useFirst)
      {
        if (_firstEnumerator.MoveNext())
        {
          return true;
        }

        _useFirst = false;
      }

      return _secondEnumerator.MoveNext();
    }

    public T Current => _useFirst ? _firstEnumerator.Current : _secondEnumerator.Current;
  }
}