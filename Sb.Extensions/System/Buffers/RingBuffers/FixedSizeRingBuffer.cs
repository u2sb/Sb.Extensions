using System;
using System.IO;

namespace Sb.Extensions.System.Buffers.RingBuffers;

/// <summary>
///   固定大小的环形缓冲区
/// </summary>
/// <typeparam name="T">缓冲区元素类型</typeparam>
public class FixedSizeRingBuffer<T>(int capacity) : RingBuffer<T>(capacity) where T : unmanaged
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
    RemoveFirst(n);
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
    if (overflow > 0) MoveHead(overflow);

    base.AddLastRange(span);
  }

  /// <summary>
  ///   从流中读取字节数据到字节缓冲区
  /// </summary>
  /// <param name="buffer"></param>
  /// <param name="stream">要读取的流</param>
  /// <param name="maxBytesToRead">最大读取字节数，-1 表示尽可能多地读取</param>
  /// <returns>实际读取的字节数</returns>
  /// <exception cref="ArgumentNullException">stream 为 null</exception>
  /// <exception cref="ArgumentException">流不可读</exception>
  public static int ReadFromStream(FixedSizeRingBuffer<byte> buffer, Stream stream, int maxBytesToRead = -1)
  {
    if (stream == null)
      throw new ArgumentNullException(nameof(stream));
    if (!stream.CanRead)
      throw new ArgumentException("Stream is not readable.", nameof(stream));

    // 计算可读取的最大字节数
    var bytesToRead = maxBytesToRead < 0
      ? buffer.Buffer.Length - buffer.Count
      : Math.Min(maxBytesToRead, buffer.Buffer.Length - buffer.Count);

    if (bytesToRead <= 0)
      return 0;

    // 计算尾部位置
    var end = (buffer.Head + buffer.Count) & buffer.Mask;
    var firstReadLength = Math.Min(bytesToRead, buffer.Buffer.Length - end);

    var totalBytesRead = 0;

    // 读取第一段
    if (firstReadLength > 0)
    {
      var bytesRead = stream.Read(buffer.Buffer, end, firstReadLength);
      totalBytesRead += bytesRead;

      // 如果第一段没读满，直接返回
      if (bytesRead < firstReadLength)
      {
        if (totalBytesRead > 0)
          buffer.MoveEnd(totalBytesRead);
        return totalBytesRead;
      }
    }

    // 读取第二段
    if (totalBytesRead < bytesToRead)
    {
      var secondReadLength = bytesToRead - firstReadLength;
      if (secondReadLength > 0)
      {
        var bytesRead = stream.Read(buffer.Buffer, 0, secondReadLength);
        totalBytesRead += bytesRead;
      }
    }

    // 更新计数器
    if (totalBytesRead > 0) buffer.MoveEnd(totalBytesRead);

    return totalBytesRead;
  }
}