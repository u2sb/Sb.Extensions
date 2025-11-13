using System;
using System.Buffers;
using System.Diagnostics;

namespace Sb.Extensions.System.Buffers;

/// <summary>
/// </summary>
public sealed class StringBufferWriter : IBufferWriter<char>
{
  // Copy of Array.MaxLength.
  // Used by projects targeting .NET Framework.
  private const int ArrayMaxLength = 0x7FFFFFC7;

  private const int DefaultInitialBufferSize = 256;

  private const char Lf = '\n';
  private const string CrLf = "\r\n";


  private char[] _buffer;
  private int _index;

  /// <summary>
  ///   创建 StringBufferWriter
  /// </summary>
  public StringBufferWriter()
  {
    _buffer = [];
    _index = 0;
  }

  /// <summary>
  /// </summary>
  /// <param name="initialCapacity"></param>
  /// <exception cref="ArgumentException"></exception>
  public StringBufferWriter(int initialCapacity)
  {
    if (initialCapacity <= 0)
      throw new ArgumentException(null, nameof(initialCapacity));

    _buffer = new char[initialCapacity];
    _index = 0;
  }

  /// <summary>
  ///   写入的内容
  /// </summary>
  public ReadOnlyMemory<char> WrittenMemory => _buffer.AsMemory(0, _index);

  /// <summary>
  ///   写入的内容
  /// </summary>
  public ReadOnlySpan<char> WrittenSpan => _buffer.AsSpan(0, _index);

  /// <summary>
  ///   写入的内容
  /// </summary>
  public string WrittenString => WrittenSpan.ToString();

  // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
  /// <summary>
  ///   写入的长度
  /// </summary>
  public int WrittenCount => _index;

  /// <summary>
  ///   分配的总长度
  /// </summary>
  public int Capacity => _buffer.Length;

  /// <summary>
  ///   空余的长度
  /// </summary>
  public int FreeCapacity => _buffer.Length - _index;

  /// <summary>
  ///   移动写入位置
  /// </summary>
  /// <param name="count"></param>
  /// <exception cref="ArgumentException"></exception>
  public void Advance(int count)
  {
    if (count < 0)
      throw new ArgumentException(null, nameof(count));

    if (_index > _buffer.Length - count)
      ThrowInvalidOperationException_AdvancedTooFar(_buffer.Length);

    _index += count;
  }

  /// <summary>
  ///   获取未写入部分的 Memory
  /// </summary>
  /// <param name="sizeHint"></param>
  /// <returns></returns>
  public Memory<char> GetMemory(int sizeHint = 0)
  {
    CheckAndResizeBuffer(sizeHint);
    Debug.Assert(_buffer.Length > _index);
    return _buffer.AsMemory(_index);
  }

  /// <summary>
  ///   获取未写入部分的 Span
  /// </summary>
  /// <param name="sizeHint"></param>
  /// <returns></returns>
  public Span<char> GetSpan(int sizeHint = 0)
  {
    CheckAndResizeBuffer(sizeHint);
    Debug.Assert(_buffer.Length > _index);
    return _buffer.AsSpan(_index);
  }

  /// <summary>
  ///   写入的内容
  /// </summary>
  /// <returns></returns>
  public override string ToString()
  {
    return WrittenString;
  }

  /// <summary>
  ///   清除并重置
  /// </summary>
  public void Clear()
  {
    Debug.Assert(_buffer.Length >= _index);
    _buffer.AsSpan(0, _index).Clear();
    _index = 0;
  }

  /// <summary>
  ///   重置已写入的长度
  /// </summary>
  public void ResetWrittenCount()
  {
    _index = 0;
  }

  private void CheckAndResizeBuffer(int sizeHint)
  {
    if (sizeHint < 0)
      throw new ArgumentException(nameof(sizeHint));

    if (sizeHint == 0) sizeHint = 1;

    if (sizeHint > FreeCapacity)
    {
      var currentLength = _buffer.Length;

      // Attempt to grow by the larger of the sizeHint and double the current size.
      var growBy = Math.Max(sizeHint, currentLength);

      if (currentLength == 0) growBy = Math.Max(growBy, DefaultInitialBufferSize);

      var newSize = currentLength + growBy;

      if ((uint)newSize > int.MaxValue)
      {
        // Attempt to grow to ArrayMaxLength.
        var needed = (uint)(currentLength - FreeCapacity + sizeHint);
        Debug.Assert(needed > currentLength);

        if (needed > ArrayMaxLength) ThrowOutOfMemoryException(needed);

        newSize = ArrayMaxLength;
      }

      Array.Resize(ref _buffer, newSize);
    }

    Debug.Assert(FreeCapacity > 0 && FreeCapacity >= sizeHint);
  }

  private static void ThrowInvalidOperationException_AdvancedTooFar(int capacity)
  {
    //throw new InvalidOperationException(SR.Format(SR.BufferWriterAdvancedTooFar, capacity));
    throw new InvalidOperationException($"AdvancedTooFar, capacity: {capacity}");
  }

  private static void ThrowOutOfMemoryException(uint capacity)
  {
    //throw new OutOfMemoryException(SR.Format(SR.BufferMaximumSizeExceeded, capacity));
    throw new OutOfMemoryException($"buffer maximum exceeded, capacity: {capacity}");
  }

  #region 写入

  /// <summary>
  ///   写入字符串
  /// </summary>
  /// <param name="s"></param>
  public void Append(string s)
  {
    this.Write(s.AsSpan());
  }

  /// <summary>
  ///   写入字符
  /// </summary>
  /// <param name="c"></param>
  public void Append(char c)
  {
    var destination = GetSpan();
    destination[0] = c;
    Advance(1);
  }

  /// <summary>
  ///   写入换行
  /// </summary>
  public void AppendLf()
  {
    Append(Lf);
  }

  /// <summary>
  ///   写入字符串并换行
  /// </summary>
  /// <param name="s"></param>
  public void AppendLf(string s)
  {
    Append(s);
    AppendLf();
  }

  /// <summary>
  ///   写入字符串并换行
  /// </summary>
  /// <param name="c"></param>
  public void AppendLf(char c)
  {
    Append(c);
    AppendLf();
  }

  /// <summary>
  ///   写入换行
  /// </summary>
  public void AppendCrLf()
  {
    Append(CrLf);
  }

  /// <summary>
  ///   写入字符串并换行
  /// </summary>
  /// <param name="s"></param>
  public void AppendCrLf(string s)
  {
    Append(s);
    AppendCrLf();
  }

  /// <summary>
  ///   写入字符并换行
  /// </summary>
  /// <param name="c"></param>
  public void AppendCrLf(char c)
  {
    Append(c);
    AppendCrLf();
  }

  /// <summary>
  ///   写入系统默认的换行符
  /// </summary>
  public void AppendLine()
  {
    Append(Environment.NewLine);
  }

  /// <summary>
  ///   写入字符串并追加系统默认的换行符
  /// </summary>
  /// <param name="s"></param>
  public void AppendLine(string s)
  {
    Append(s);
    AppendLine();
  }

  /// <summary>
  ///   写入字串并追加系统默认的换行符
  /// </summary>
  /// <param name="c"></param>
  public void AppendLine(char c)
  {
    Append(c);
    AppendLine();
  }

  #endregion
}