using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Sb.Extensions.System.Buffers.RingBuffers;

/// <summary>
///   固定大小的环形缓冲区（兼容 NetStandard 2.0）
/// </summary>
/// <typeparam name="T">缓冲区元素类型</typeparam>
public class FixedSizeRingBuffer<T>(int capacity) : RingBuffer<T>(capacity)
  where T : struct // 只支持值类型以确保字节复制的安全性
{
  /// <summary>
  ///   获取可用于写入的空间区域
  /// </summary>
  public RingBufferSpan<T> WritableSpan => GetWritableSpan();

  /// <summary>
  ///   移动缓冲区末尾指针，增加元素计数（但不写入数据）
  /// </summary>
  /// <param name="n">要增加的计数</param>
  public void MoveEnd(int n)
  {
    if (n < 0)
      throw new ArgumentOutOfRangeException(nameof(n), "n cannot be negative.");
    if (n > Buffer.Length - Count)
      throw new ArgumentOutOfRangeException(nameof(n),
        $"n ({n}) exceeds available space ({Buffer.Length - Count}).");
    Count += n;
  }

  /// <summary>
  ///   移动头部指针，移除前 n 个元素
  /// </summary>
  /// <param name="n">要移除的元素数量</param>
  public void MoveHead(int n)
  {
    if (n < 0)
      throw new ArgumentOutOfRangeException(nameof(n), "n cannot be negative.");
    if (n > Count)
      throw new ArgumentOutOfRangeException(nameof(n),
        $"n ({n}) cannot be greater than Count ({Count}).");
    if (n == 0) return;

    // 清除要移除的元素，防止内存泄漏
    var start = Head & Mask;

    if (start + n <= Buffer.Length)
    {
      // 单个连续段
      Array.Clear(Buffer, start, n);
    }
    else
    {
      // 环绕情况
      var firstSegmentLength = Buffer.Length - start;
      Array.Clear(Buffer, start, firstSegmentLength);
      var secondSegmentLength = n - firstSegmentLength;
      if (secondSegmentLength > 0)
        Array.Clear(Buffer, 0, secondSegmentLength);
    }

    Head = (Head + n) & Mask;
    Count -= n;
  }

  /// <summary>
  ///   在末尾添加元素。如果缓冲区已满，则移除第一个元素
  /// </summary>
  /// <param name="item">要添加的元素</param>
  public override void AddLast(T item)
  {
    if (Count == Buffer.Length)
      RemoveFirst();
    base.AddLast(item);
  }

  /// <summary>
  ///   在开头添加元素。如果缓冲区已满，则移除最后一个元素
  /// </summary>
  /// <param name="item">要添加的元素</param>
  public override void AddFirst(T item)
  {
    if (Count == Buffer.Length)
      RemoveLast();
    base.AddFirst(item);
  }

  /// <summary>
  ///   批量在末尾添加元素。如果添加的数量超过缓冲区容量，则只保留最后的 capacity 个元素
  /// </summary>
  /// <param name="items">要添加的元素集合</param>
  public override void AddLastRange(ReadOnlySpan<T> items)
  {
    if (items.Length == 0) return;

    var span = items;

    // 如果要添加的数量超过了缓冲区大小，只保留最后的 capacity 个元素
    if (span.Length > Buffer.Length)
      span = span[^Buffer.Length..];

    // 计算需要溢出的旧元素数量
    var overflow = Math.Max(0, Count + span.Length - Buffer.Length);
    if (overflow > 0)
    {
      // 移除最旧的前 overflow 个元素
      var start = Head & Mask;

      if (start + overflow <= Buffer.Length)
      {
        // 单个连续段
        Array.Clear(Buffer, start, overflow);
      }
      else
      {
        // 环绕情况
        var firstSegmentLength = Buffer.Length - start;
        Array.Clear(Buffer, start, firstSegmentLength);
        var secondSegmentLength = overflow - firstSegmentLength;
        if (secondSegmentLength > 0)
          Array.Clear(Buffer, 0, secondSegmentLength);
      }

      Head = (Head + overflow) & Mask;
      Count -= overflow;
    }

    base.AddLastRange(span);
  }

  /// <summary>
  ///   从流中读取数据到缓冲区（兼容 NetStandard 2.0 版本）
  /// </summary>
  /// <param name="stream">要读取的流</param>
  /// <exception cref="ArgumentException">流不可读</exception>
  public void ReadFromStream(Stream stream)
  {
    if (stream == null)
      throw new ArgumentNullException(nameof(stream));
    if (!stream.CanRead)
      throw new ArgumentException("Stream is not readable.", nameof(stream));

    // 计算可用空间
    var availableSpace = Buffer.Length - Count;
    if (availableSpace == 0)
    {
      // 如果没有空间，先移除一些元素来腾出空间
      var toRemove = Buffer.Length / 4;
      if (toRemove > 0)
        MoveHead(toRemove);
      availableSpace = Buffer.Length - Count;
    }

    // 确定从哪里开始写入
    var tail = (Head + Count) & Mask;

    // 计算元素大小（兼容 NetStandard 2.0 的实现）
    var elementSize = GetElementSize();

    // 计算要读取的字节数
    long bytesToRead;
    if (stream.CanSeek)
    {
      var maxBytesFromAvailableSpace = availableSpace * elementSize;
      var maxBytesFromStream = stream.Length - stream.Position;
      bytesToRead = Math.Min(maxBytesFromAvailableSpace, maxBytesFromStream);
    }
    else
    {
      // 对于不可查找的流，我们只能读取可用空间能容纳的最大字节数
      bytesToRead = availableSpace * elementSize;
    }

    if (bytesToRead <= 0) return;

    // 处理连续段情况
    if (tail + availableSpace <= Buffer.Length)
    {
      // 单个连续段
      var elementsToRead = (int)Math.Min(availableSpace, bytesToRead / elementSize);
      if (elementsToRead > 0)
        ReadToBufferSegment(stream, tail, elementsToRead, elementSize);
    }
    else
    {
      // 环绕情况
      var firstSegmentLength = Buffer.Length - tail;
      var firstElementsToRead = (int)Math.Min(
        firstSegmentLength,
        bytesToRead / elementSize
      );

      if (firstElementsToRead > 0)
        ReadToBufferSegment(stream, tail, firstElementsToRead, elementSize);

      // 如果有剩余空间，继续读取到缓冲区开头
      var remainingBytes = bytesToRead - firstElementsToRead * elementSize;
      var remainingElementsToRead = (int)Math.Min(
        availableSpace - firstElementsToRead,
        remainingBytes / elementSize
      );

      if (remainingElementsToRead > 0)
        ReadToBufferSegment(stream, 0, remainingElementsToRead, elementSize);
    }
  }

  /// <summary>
  ///   将数据从流读取到缓冲区的特定段
  /// </summary>
  private void ReadToBufferSegment(Stream stream, int bufferStartIndex, int elementsToRead, int elementSize)
  {
    if (elementsToRead <= 0) return;

    // 使用字节数组作为中间缓冲区，兼容 NetStandard 2.0
    var totalBytes = elementsToRead * elementSize;
    var byteBuffer = new byte[totalBytes];

    var totalBytesRead = 0;
    while (totalBytesRead < totalBytes)
    {
      var bytesRead = stream.Read(
        byteBuffer,
        totalBytesRead,
        totalBytes - totalBytesRead
      );

      if (bytesRead == 0) break; // 流结束
      totalBytesRead += bytesRead;
    }

    if (totalBytesRead == 0) return;

    // 实际读取到的元素数
    var actualElementsRead = totalBytesRead / elementSize;

    if (actualElementsRead > 0)
    {
      // 将字节缓冲区转换为元素并复制到目标缓冲区
      CopyBytesToBuffer(byteBuffer, bufferStartIndex, actualElementsRead, elementSize);
      Count += actualElementsRead;
    }
  }

  /// <summary>
  ///   将字节数组中的数据复制到缓冲区的特定位置
  /// </summary>
  private void CopyBytesToBuffer(byte[] source, int bufferStartIndex, int elementCount, int elementSize)
  {
    // 注意：由于已经有 where T : struct 约束，这里可以安全地进行字节复制
    // 计算字节偏移和字节数
    var sourceByteOffset = 0;
    var destinationByteOffset = bufferStartIndex * elementSize;
    var byteCount = elementCount * elementSize;

    // 关键修复：使用全局命名空间的 System.Buffer 类
    // SystemBuffer.BlockCopy 的第一个目标数组参数必须是数组，这里 Buffer 是 T[]
    // 这是允许的，因为 Buffer.BlockCopy 接受任何数组类型
    global::System.Buffer.BlockCopy(
      source,
      sourceByteOffset,
      Buffer,
      destinationByteOffset,
      byteCount
    );
  }

  /// <summary>
  ///   获取元素大小（兼容 NetStandard 2.0 的实现）
  /// </summary>
  private static int GetElementSize()
  {
    // 由于 T 是值类型，我们可以安全地使用 Marshal.SizeOf
    // 但对于某些平台或特殊情况，需要备选方案

    // 首先尝试使用条件编译的高效方法
#if NETSTANDARD2_0
    // 对于 NetStandard 2.0，使用备选方法
    return EstimateElementSize();
#else
    try
    {
      // 使用泛型版本，在支持的平台上更高效
      return Marshal.SizeOf<T>();
    }
    catch
    {
      // 如果不支持，退回到估算
      return EstimateElementSize();
    }
#endif
  }

  /// <summary>
  ///   估算元素大小（备用方案）
  /// </summary>
  private static int EstimateElementSize()
  {
    // 常见类型的预设大小（基于 C# 规范）
    if (typeof(T) == typeof(byte) || typeof(T) == typeof(sbyte) || typeof(T) == typeof(bool))
      return 1;
    if (typeof(T) == typeof(char) || typeof(T) == typeof(short) || typeof(T) == typeof(ushort))
      return 2;
    if (typeof(T) == typeof(int) || typeof(T) == typeof(uint) || typeof(T) == typeof(float))
      return 4;
    if (typeof(T) == typeof(long) || typeof(T) == typeof(ulong) || typeof(T) == typeof(double))
      return 8;
    if (typeof(T) == typeof(decimal))
      return 16;

    // 对于未知的值类型，尝试通过反射获取大小
    try
    {
      // 这种方法在某些平台上可能不可用，所以放在 try-catch 中
#if !NETSTANDARD2_0
      return Marshal.SizeOf<T>();
#else
      // 在 NetStandard 2.0 中，非泛型 Marshal.SizeOf 需要具体类型
      return Marshal.SizeOf(typeof(T));
#endif
    }
    catch
    {
      // 如果所有方法都失败，返回一个安全的大小（通常是最大的基本类型大小）
      // 这可能会导致内存浪费，但确保代码能运行
      return 16; // decimal 的大小，足够大以容纳大多数值类型
    }
  }
}