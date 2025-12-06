using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Sb.Extensions.System;

/// <summary>
///   提供对字节缓冲区的位级别操作的结构体，支持切片、按位访问、批量操作等功能。
/// </summary>
public readonly ref struct BitSpan
{
  private readonly Span<byte> _span;
  private readonly int _startBitOffset;

  /// <summary>
  ///   获取位段长度
  /// </summary>
  public int Length { get; }

  /// <summary>
  ///   使用字节缓冲区、位数和起始位偏移构造位段。
  /// </summary>
  /// <param name="buffer">字节缓冲区</param>
  /// <param name="bitCount">位数</param>
  /// <param name="startBitOffset">起始位偏移（0-7）</param>
  /// <exception cref="ArgumentOutOfRangeException">bitCount 或 startBitOffset 非法</exception>
  /// <exception cref="ArgumentException">缓冲区不足</exception>
  public BitSpan(Span<byte> buffer, int bitCount, int startBitOffset = 0)
  {
    if (bitCount < 0)
      BitSpanException.ThrowInvalidSliceArguments(0, bitCount, buffer.Length * 8);
    if (startBitOffset is < 0 or >= 8)
      BitSpanException.ThrowInvalidSliceArguments(startBitOffset, bitCount, buffer.Length * 8);

    var maxAvailableBits = buffer.Length * 8 - startBitOffset;
    if (bitCount > maxAvailableBits)
      throw new ArgumentException("Insufficient buffer for bit count and offset.");

    _span = buffer;
    Length = bitCount;
    _startBitOffset = startBitOffset;
  }

  /// <summary>
  ///   使用字节缓冲区构造位段，长度为缓冲区总位数。
  /// </summary>
  /// <param name="buffer">字节缓冲区</param>
  public BitSpan(Span<byte> buffer)
  {
    _span = buffer;
    Length = _span.Length * 8;
    _startBitOffset = 0;
  }

  /// <summary>
  ///   使用 ushort 缓冲区构造位段。
  /// </summary>
  /// <param name="buffer">ushort 缓冲区</param>
  public BitSpan(Span<ushort> buffer)
  {
    _span = MemoryMarshal.AsBytes(buffer);
    Length = _span.Length * 8;
    _startBitOffset = 0;
  }

  /// <summary>
  ///   使用 short 缓冲区构造位段。
  /// </summary>
  /// <param name="buffer">short 缓冲区</param>
  public BitSpan(Span<short> buffer)
  {
    _span = MemoryMarshal.AsBytes(buffer);
    Length = _span.Length * 8;
    _startBitOffset = 0;
  }

  /// <summary>
  /// </summary>
  public Span<byte> Span => _span;


  /// <summary>
  ///   获取或设置指定索引的位值。
  /// </summary>
  /// <param name="index">位索引</param>
  /// <returns>指定索引的位值</returns>
  public bool this[int index]
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get => Get(index);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    set => Set(index, value);
  }

  /// <summary>
  ///   获取指定起始位和长度的位段切片。
  /// </summary>
  /// <param name="start">起始位</param>
  /// <param name="length">长度</param>
  /// <returns>位段切片</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public BitSpan Slice(int start, int length)
  {
    if (start < 0 || length < 0 || start + length > Length)
      BitSpanException.ThrowInvalidSliceArguments(start, length, Length);

    var newStartBitOffset = _startBitOffset + start;
    var startByte = newStartBitOffset >> 3;
    var newBitOffset = newStartBitOffset & 7;

    var requiredBytes = (length + newBitOffset + 7) >> 3;
    var newSpan = _span.Slice(startByte, requiredBytes);

    return new BitSpan(newSpan, length, newBitOffset);
  }

  /// <summary>
  ///   获取指定索引的位值。
  /// </summary>
  /// <param name="index">位索引</param>
  /// <returns>指定索引的位值</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool Get(int index)
  {
    if ((uint)index >= (uint)Length)
      BitSpanException.ThrowIndexOutOfRange(index, Length);

    var totalBit = _startBitOffset + index;
    var byteIndex = totalBit >> 3;
    var bitOffset = totalBit & 7;
    return (_span[byteIndex] & (1 << bitOffset)) != 0;
  }

  /// <summary>
  ///   设置指定索引的位值。
  /// </summary>
  /// <param name="index">位索引</param>
  /// <param name="value">要设置的位值</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Set(int index, bool value)
  {
    if ((uint)index >= (uint)Length)
      BitSpanException.ThrowIndexOutOfRange(index, Length);

    var totalBit = _startBitOffset + index;
    var byteIndex = totalBit >> 3;
    var bitOffset = totalBit & 7;
    ref var target = ref _span[byteIndex];
    if (value)
      target |= (byte)(1 << bitOffset);
    else
      target &= (byte)~(1 << bitOffset);
  }

  /// <summary>
  ///   将所有位设置为指定值。
  /// </summary>
  /// <param name="value">要设置的位值</param>
  public void SetAll(bool value)
  {
    if (Length == 0) return;

    int fullBytes;
    int lastBits;

    // 优化起始对齐情况
    if (_startBitOffset == 0)
    {
      // 直接批量处理完整字节
      fullBytes = Length >> 3;
      if (fullBytes > 0)
      {
        var pattern = value ? (byte)0xFF : (byte)0x00;
        _span.Slice(0, fullBytes).Fill(pattern);
      }

      // 处理末尾剩余位
      lastBits = Length & 0x07;
      if (lastBits <= 0) return;

      var mask = (byte)(0xFF >> (8 - lastBits));
      _span[fullBytes] = value ? (byte)(_span[fullBytes] | mask) : (byte)(_span[fullBytes] & ~mask);

      return;
    }

    // 原逻辑处理非对齐情况
    var firstByteBits = Math.Min(8 - _startBitOffset, Length);
    SetBits(0, firstByteBits, value);

    var remainingBits = Length - firstByteBits;
    if (remainingBits <= 0) return;

    fullBytes = remainingBits >> 3;
    var patternFull = value ? (byte)0xFF : (byte)0x00;
    var startByte = (_startBitOffset + firstByteBits) >> 3;
    _span.Slice(startByte, fullBytes).Fill(patternFull);

    lastBits = remainingBits & 0x07;
    if (lastBits > 0)
    {
      var lastStart = firstByteBits + (fullBytes << 3);
      var lastMask = (byte)(0xFF >> (8 - lastBits));
      var lastBytePos = (_startBitOffset + lastStart) >> 3;
      _span[lastBytePos] = value ? (byte)(_span[lastBytePos] | lastMask) : (byte)(_span[lastBytePos] & ~lastMask);
    }
  }

  /// <summary>
  ///   对指定范围的位批量设置为指定值。
  /// </summary>
  /// <param name="startBit">起始位</param>
  /// <param name="bitCount">位数</param>
  /// <param name="value">要设置的位值</param>
  private void SetBits(int startBit, int bitCount, bool value)
  {
    if (bitCount <= 0) return;

    var firstBit = _startBitOffset + startBit;
    var firstByte = firstBit >> 3;
    var firstBitInByte = firstBit & 7;

    // 处理首字节部分位
    var bitsInFirstByte = Math.Min(8 - firstBitInByte, bitCount);
    if (bitsInFirstByte < 8)
    {
      var mask = (byte)(((1 << bitsInFirstByte) - 1) << firstBitInByte);
      if (value)
        _span[firstByte] |= mask;
      else
        _span[firstByte] &= (byte)~mask;
      bitCount -= bitsInFirstByte;
      firstByte++;
    }

    // 处理中间完整字节
    var fullBytes = bitCount >> 3;
    if (fullBytes > 0)
    {
      var pattern = value ? (byte)0xFF : (byte)0x00;
      _span.Slice(firstByte, fullBytes).Fill(pattern);
      firstByte += fullBytes;
    }

    // 处理尾部零头
    var lastBits = bitCount & 7;
    if (lastBits > 0)
    {
      var mask = (byte)((1 << lastBits) - 1);
      if (value)
        _span[firstByte] |= mask;
      else
        _span[firstByte] &= (byte)~mask;
    }
  }

  /// <summary>
  ///   对所有位取反。
  /// </summary>
  public void Not()
  {
    int remainingBits;
    int fullBytes;
    if (_startBitOffset == 0)
    {
      fullBytes = Length >> 3;
      if (fullBytes > 0)
        for (var i = 0; i < fullBytes; i++)
          _span[i] = (byte)~_span[i];

      remainingBits = Length & 0x07;
      if (remainingBits > 0) NotBits(fullBytes << 3, remainingBits);
      return;
    }

    var firstByteBits = Math.Min(8 - _startBitOffset, Length);
    NotBits(0, firstByteBits);

    remainingBits = Length - firstByteBits;
    if (remainingBits <= 0) return;

    fullBytes = remainingBits >> 3;
    var startByte = (_startBitOffset + firstByteBits) >> 3;
    for (var i = 0; i < fullBytes; i++) _span[startByte + i] = (byte)~_span[startByte + i];

    var lastBits = remainingBits & 0x07;
    if (lastBits > 0) NotBits(firstByteBits + (fullBytes << 3), lastBits);
  }

  /// <summary>
  ///   对指定范围的位批量取反。
  /// </summary>
  /// <param name="startBit">起始位</param>
  /// <param name="bitCount">位数</param>
  private void NotBits(int startBit, int bitCount)
  {
    if (bitCount <= 0) return;

    var firstBit = _startBitOffset + startBit;
    var firstByte = firstBit >> 3;
    var firstBitInByte = firstBit & 7;

    var bitsInFirstByte = Math.Min(8 - firstBitInByte, bitCount);
    if (bitsInFirstByte < 8)
    {
      var mask = (byte)(((1 << bitsInFirstByte) - 1) << firstBitInByte);
      _span[firstByte] ^= mask;
      bitCount -= bitsInFirstByte;
      firstByte++;
    }

    var fullBytes = bitCount >> 3;
    for (var i = 0; i < fullBytes; i++)
      _span[firstByte + i] = (byte)~_span[firstByte + i];
    firstByte += fullBytes;

    var lastBits = bitCount & 7;
    if (lastBits > 0)
    {
      var mask = (byte)((1 << lastBits) - 1);
      _span[firstByte] ^= mask;
    }
  }

  /// <summary>
  ///   对应位与另一个 BitSpan 做按位与操作。
  /// </summary>
  /// <param name="other">另一个 BitSpan</param>
  public void And(BitSpan other)
  {
    if (other.Length != Length)
      BitSpanException.ThrowLengthMismatch(Length, other.Length);
    ApplyBitwise(other, (a, b) => a & b);
  }

  /// <summary>
  ///   对应位与另一个 BitSpan 做按位或操作。
  /// </summary>
  /// <param name="other">另一个 BitSpan</param>
  public void Or(BitSpan other)
  {
    if (other.Length != Length)
      BitSpanException.ThrowLengthMismatch(Length, other.Length);
    ApplyBitwise(other, (a, b) => a | b);
  }

  /// <summary>
  ///   对应位与另一个 BitSpan 做按位异或操作。
  /// </summary>
  /// <param name="other">另一个 BitSpan</param>
  public void Xor(BitSpan other)
  {
    if (other.Length != Length)
      BitSpanException.ThrowLengthMismatch(Length, other.Length);
    ApplyBitwise(other, (a, b) => a ^ b);
  }

  /// <summary>
  ///   对应位与另一个 BitSpan 做自定义按位操作。
  /// </summary>
  /// <param name="other">另一个 BitSpan</param>
  /// <param name="op">按位操作委托</param>
  private void ApplyBitwise(BitSpan other, Func<bool, bool, bool> op)
  {
    for (var i = 0; i < Length; i++) this[i] = op(this[i], other[i]);
  }

  /// <summary>
  ///   转换为 BitArray。
  /// </summary>
  /// <returns>包含所有位的 BitArray</returns>
  public BitArray ToBitArray()
  {
    if (_startBitOffset == 0 && (Length & 0x07) == 0) return new BitArray(_span.ToArray());
    return new BitArray(ToBoolArray());
  }

  /// <summary>
  ///   拷贝位段到目标 bool 数组。
  /// </summary>
  /// <param name="destination">目标 bool 数组</param>
  /// <exception cref="ArgumentException">目标数组长度不足</exception>
  public void CopyTo(Span<bool> destination)
  {
    if (destination.Length < Length)
      BitSpanException.ThrowDestinationTooShort(destination.Length, Length);

    for (var i = 0; i < Length; i++)
      destination[i] = Get(i);
  }

  /// <summary>
  ///   转换为 bool 数组。
  /// </summary>
  /// <returns>包含所有位的 bool 数组</returns>
  public bool[] ToBoolArray()
  {
    var result = new bool[Length];
    CopyTo(result);
    return result;
  }

  /// <summary>
  ///   获取位段的枚举器。
  /// </summary>
  /// <returns>位段枚举器</returns>
  public Enumerator GetEnumerator()
  {
    return new Enumerator(this);
  }

  /// <summary>
  ///   位段枚举器。
  /// </summary>
  /// <param name="bitSpan">要枚举的 BitSpan</param>
  public ref struct Enumerator(BitSpan bitSpan)
  {
    private readonly BitSpan _bitSpan = bitSpan;
    private int _index = -1;

    /// <summary>
    ///   获取当前位的值。
    /// </summary>
    public bool Current
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _bitSpan.Get(_index);
    }

    /// <summary>
    ///   移动到下一个位。
    /// </summary>
    /// <returns>是否还有下一个位</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
      return ++_index < _bitSpan.Length;
    }

    /// <summary>
    ///   让枚举器也可以使用 foreach
    /// </summary>
    /// <returns></returns>
    public Enumerator GetEnumerator()
    {
      return this;
    }
  }

  #region 获取数据

  /// <summary>
  ///   获取指定偏移和长度的字节值。
  /// </summary>
  /// <param name="offset">起始位偏移</param>
  /// <param name="length">位长度（默认8）</param>
  /// <returns>对应的字节值</returns>
  public byte GetByte(int offset, int length = 8)
  {
    return GetT<byte>(offset, length);
  }

  /// <summary>
  ///   获取指定偏移和长度的 ushort 值。
  /// </summary>
  /// <param name="offset">起始位偏移</param>
  /// <param name="length">位长度（默认16）</param>
  /// <param name="useBigEndianMode">是否大端模式</param>
  /// <returns>对应的 ushort 值</returns>
  public ushort GetUInt16(int offset, int length = 16, bool useBigEndianMode = true)
  {
    return GetT<ushort>(offset, length, useBigEndianMode);
  }

  /// <summary>
  ///   获取指定偏移和长度的 uint 值。
  /// </summary>
  /// <param name="offset">起始位偏移</param>
  /// <param name="length">位长度（默认32）</param>
  /// <param name="useBigEndianMode">是否大端模式</param>
  /// <returns>对应的 uint 值</returns>
  public uint GetUInt32(int offset, int length = 32, bool useBigEndianMode = true)
  {
    return GetT<uint>(offset, length, useBigEndianMode);
  }

  /// <summary>
  ///   获取指定偏移和长度的 T 类型值。
  /// </summary>
  /// <typeparam name="T">目标类型</typeparam>
  /// <param name="offset">起始位偏移</param>
  /// <param name="length">位长度</param>
  /// <param name="useBigEndianMode">是否大端模式</param>
  /// <returns>对应的 T 类型值</returns>
  public T GetT<T>(int offset, int length, bool useBigEndianMode = true) where T : unmanaged
  {
    Span<byte> span = stackalloc byte[Unsafe.SizeOf<T>()];
    CopyTo(span, offset, length);
    return span.ToT<T>(useBigEndianMode);
  }

  /// <summary>
  ///   拷贝指定范围的位到目标字节数组。
  /// </summary>
  /// <param name="destination">目标字节数组</param>
  /// <param name="bitOffset">起始位偏移</param>
  /// <param name="bitCount">位数</param>
  /// <exception cref="ArgumentOutOfRangeException">参数越界</exception>
  /// <exception cref="ArgumentException">目标数组空间不足</exception>
  public void CopyTo(Span<byte> destination, int bitOffset, int bitCount)
  {
    if (bitOffset < 0 || bitCount < 0)
      BitSpanException.ThrowInvalidSliceArguments(bitOffset, bitCount, Length);
    if (bitOffset + bitCount > Length)
      BitSpanException.ThrowInvalidSliceArguments(bitOffset, bitCount, Length);
    if (destination.Length * 8 < bitCount)
      BitSpanException.ThrowDestinationTooShort(destination.Length * 8, bitCount);

    destination.Clear();

    var srcBit = _startBitOffset + bitOffset;
    var dstBit = 0;
    var remaining = bitCount;

    while (remaining > 0)
    {
      var srcByte = srcBit >> 3;
      var srcBitInByte = srcBit & 0x07;
      var copyBits = Math.Min(8 - srcBitInByte, remaining);

      var srcValue = (byte)((_span[srcByte] >> srcBitInByte) & (0xFF >> (8 - copyBits)));

      var dstByte = dstBit >> 3;
      var dstBitInByte = dstBit & 0x07;

      if (dstBitInByte == 0 && copyBits == 8)
      {
        destination[dstByte] = srcValue;
      }
      else
      {
        var availableBits = 8 - dstBitInByte;
        if (availableBits >= copyBits)
        {
          var mask = (byte)((0xFF >> (8 - copyBits)) << dstBitInByte);
          destination[dstByte] = (byte)((destination[dstByte] & ~mask) | (srcValue << dstBitInByte));
        }
        else
        {
          // var lowMask = (byte)(0xFF >> (8 - availableBits));
          destination[dstByte] |= (byte)(srcValue << dstBitInByte);

          var highBits = copyBits - availableBits;
          var highValue = (byte)(srcValue >> availableBits);
          var highMask = (byte)(0xFF >> (8 - highBits));
          destination[dstByte + 1] |= (byte)(highValue & highMask);
        }
      }

      srcBit += copyBits;
      dstBit += copyBits;
      remaining -= copyBits;
    }

    var validBits = bitCount & 0x07;
    if (validBits != 0)
    {
      var lastByte = bitCount >> 3;
      destination[lastByte] &= (byte)(0xFF >> (8 - validBits));
    }
  }

  #endregion
}

/// <summary>
///   只读位段，支持 ReadOnlySpan 等类型的只读位访问。
/// </summary>
public readonly ref struct ReadOnlyBitSpan
{
  private readonly ReadOnlySpan<byte> _span;
  private readonly int _startBitOffset;

  /// <summary>
  ///   获取位段长度（bit）。
  /// </summary>
  public int Length { get; }

  /// <summary>
  ///   使用只读字节缓冲区、位数和起始位偏移构造只读位段。
  /// </summary>
  /// <param name="buffer">只读字节缓冲区</param>
  /// <param name="bitCount">位数</param>
  /// <param name="startBitOffset">起始位偏移（0-7）</param>
  /// <exception cref="ArgumentOutOfRangeException">bitCount 或 startBitOffset 非法</exception>
  /// <exception cref="ArgumentException">缓冲区不足</exception>
  public ReadOnlyBitSpan(ReadOnlySpan<byte> buffer, int bitCount, int startBitOffset = 0)
  {
    if (bitCount < 0)
      BitSpanException.ThrowInvalidSliceArguments(0, bitCount, buffer.Length * 8);
    if (startBitOffset is < 0 or >= 8)
      BitSpanException.ThrowInvalidSliceArguments(startBitOffset, bitCount, buffer.Length * 8);

    var maxAvailableBits = buffer.Length * 8 - startBitOffset;
    if (bitCount > maxAvailableBits)
      throw new ArgumentException("Insufficient buffer for bit count and offset.");

    _span = buffer;
    Length = bitCount;
    _startBitOffset = startBitOffset;
  }

  /// <summary>
  ///   使用只读字节缓冲区构造只读位段，长度为缓冲区总位数。
  /// </summary>
  /// <param name="buffer">只读字节缓冲区</param>
  public ReadOnlyBitSpan(ReadOnlySpan<byte> buffer)
  {
    _span = buffer;
    Length = _span.Length * 8;
    _startBitOffset = 0;
  }

  /// <summary>
  ///   使用只读 ushort 缓冲区构造只读位段。
  /// </summary>
  /// <param name="buffer">只读 ushort 缓冲区</param>
  public ReadOnlyBitSpan(ReadOnlySpan<ushort> buffer)
  {
    _span = MemoryMarshal.AsBytes(buffer);
    Length = _span.Length * 8;
    _startBitOffset = 0;
  }

  /// <summary>
  ///   使用只读 short 缓冲区构造只读位段。
  /// </summary>
  /// <param name="buffer">只读 short 缓冲区</param>
  public ReadOnlyBitSpan(ReadOnlySpan<short> buffer)
  {
    _span = MemoryMarshal.AsBytes(buffer);
    Length = _span.Length * 8;
    _startBitOffset = 0;
  }

  /// <summary>
  ///   按位只读索引器
  /// </summary>
  public bool this[int index]
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get => Get(index);
  }

  /// <summary>
  ///   切片
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public ReadOnlyBitSpan Slice(int start, int length)
  {
    if (start < 0 || length < 0 || start + length > Length)
      BitSpanException.ThrowInvalidSliceArguments(start, length, Length);

    var newStartBitOffset = _startBitOffset + start;
    var startByte = newStartBitOffset >> 3;
    var newBitOffset = newStartBitOffset & 7;

    var requiredBytes = (length + newBitOffset + 7) >> 3;
    var newSpan = _span.Slice(startByte, requiredBytes);

    return new ReadOnlyBitSpan(newSpan, length, newBitOffset);
  }

  /// <summary>
  ///   获取指定索引的位值。
  /// </summary>
  /// <param name="index">位索引</param>
  /// <returns>指定索引的位值</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool Get(int index)
  {
    if ((uint)index >= (uint)Length)
      BitSpanException.ThrowIndexOutOfRange(index, Length);

    var totalBit = _startBitOffset + index;
    var byteIndex = totalBit >> 3;
    var bitOffset = totalBit & 7;
    return (_span[byteIndex] & (1 << bitOffset)) != 0;
  }

  /// <summary>
  ///   拷贝位段到目标 bool 数组。
  /// </summary>
  /// <param name="destination">目标 bool 数组</param>
  /// <exception cref="ArgumentException">目标数组长度不足</exception>
  public void CopyTo(Span<bool> destination)
  {
    if (destination.Length < Length)
      BitSpanException.ThrowDestinationTooShort(destination.Length, Length);

    for (var i = 0; i < Length; i++)
      destination[i] = Get(i);
  }

  /// <summary>
  ///   转换为 bool 数组。
  /// </summary>
  /// <returns>包含所有位的 bool 数组</returns>
  public bool[] ToBoolArray()
  {
    var result = new bool[Length];
    CopyTo(result);
    return result;
  }

  /// <summary>
  ///   转换为 BitArray。
  /// </summary>
  /// <returns>包含所有位的 BitArray</returns>
  public BitArray ToBitArray()
  {
    if (_startBitOffset == 0 && (Length & 0x07) == 0) return new BitArray(_span.ToArray());
    return new BitArray(ToBoolArray());
  }

  /// <summary>
  ///   获取位段的枚举器。
  /// </summary>
  /// <returns>位段枚举器</returns>
  public Enumerator GetEnumerator()
  {
    return new Enumerator(this);
  }

  /// <summary>
  ///   只读位段枚举器。
  /// </summary>
  public ref struct Enumerator
  {
    private readonly ReadOnlyBitSpan _bitSpan;
    private int _index;

    /// <summary>
    ///   获取枚举器
    /// </summary>
    /// <param name="bitSpan"></param>
    public Enumerator(ReadOnlyBitSpan bitSpan)
    {
      _bitSpan = bitSpan;
      _index = -1;
    }

    /// <summary>
    ///   获取当前位的值。
    /// </summary>
    public bool Current
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _bitSpan.Get(_index);
    }

    /// <summary>
    ///   移动到下一个位。
    /// </summary>
    /// <returns>是否还有下一个位</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
      return ++_index < _bitSpan.Length;
    }

    /// <summary>
    ///   为枚举器实现 foreach 支持
    /// </summary>
    /// <returns></returns>
    public Enumerator GetEnumerator()
    {
      return this;
    }
  }

  #region 获取数据

  /// <summary>
  ///   获取指定偏移和长度的字节值。
  /// </summary>
  /// <param name="offset">起始位偏移</param>
  /// <param name="length">位长度（默认8）</param>
  /// <returns>对应的字节值</returns>
  public byte GetByte(int offset, int length = 8)
  {
    return GetT<byte>(offset, length);
  }

  /// <summary>
  ///   获取指定偏移和长度的 ushort 值。
  /// </summary>
  /// <param name="offset">起始位偏移</param>
  /// <param name="length">位长度（默认16）</param>
  /// <param name="useBigEndianMode">是否大端模式</param>
  /// <returns>对应的 ushort 值</returns>
  public ushort GetUInt16(int offset, int length = 16, bool useBigEndianMode = true)
  {
    return GetT<ushort>(offset, length, useBigEndianMode);
  }

  /// <summary>
  ///   获取指定偏移和长度的 uint 值。
  /// </summary>
  /// <param name="offset">起始位偏移</param>
  /// <param name="length">位长度（默认32）</param>
  /// <param name="useBigEndianMode">是否大端模式</param>
  /// <returns>对应的 uint 值</returns>
  public uint GetUInt32(int offset, int length = 32, bool useBigEndianMode = true)
  {
    return GetT<uint>(offset, length, useBigEndianMode);
  }

  /// <summary>
  ///   获取指定偏移和长度的 T 类型值。
  /// </summary>
  /// <typeparam name="T">目标类型</typeparam>
  /// <param name="offset">起始位偏移</param>
  /// <param name="length">位长度</param>
  /// <param name="useBigEndianMode">是否大端模式</param>
  /// <returns>对应的 T 类型值</returns>
  public T GetT<T>(int offset, int length, bool useBigEndianMode = true) where T : unmanaged
  {
    Span<byte> span = stackalloc byte[Unsafe.SizeOf<T>()];
    CopyTo(span, offset, length);
    return span.ToT<T>(useBigEndianMode);
  }

  /// <summary>
  ///   拷贝指定范围的位到目标字节数组。
  /// </summary>
  /// <param name="destination">目标字节数组</param>
  /// <param name="bitOffset">起始位偏移</param>
  /// <param name="bitCount">位数</param>
  /// <exception cref="ArgumentOutOfRangeException">参数越界</exception>
  /// <exception cref="ArgumentException">目标数组空间不足</exception>
  public void CopyTo(Span<byte> destination, int bitOffset, int bitCount)
  {
    if (bitOffset < 0 || bitCount < 0)
      BitSpanException.ThrowInvalidSliceArguments(bitOffset, bitCount, Length);
    if (bitOffset + bitCount > Length)
      BitSpanException.ThrowInvalidSliceArguments(bitOffset, bitCount, Length);
    if (destination.Length * 8 < bitCount)
      BitSpanException.ThrowDestinationTooShort(destination.Length * 8, bitCount);

    destination.Clear();

    var srcBit = _startBitOffset + bitOffset;
    var dstBit = 0;
    var remaining = bitCount;

    while (remaining > 0)
    {
      var srcByte = srcBit >> 3;
      var srcBitInByte = srcBit & 0x07;
      var copyBits = Math.Min(8 - srcBitInByte, remaining);

      var srcValue = (byte)((_span[srcByte] >> srcBitInByte) & (0xFF >> (8 - copyBits)));

      var dstByte = dstBit >> 3;
      var dstBitInByte = dstBit & 0x07;

      if (dstBitInByte == 0 && copyBits == 8)
      {
        destination[dstByte] = srcValue;
      }
      else
      {
        var availableBits = 8 - dstBitInByte;
        if (availableBits >= copyBits)
        {
          var mask = (byte)((0xFF >> (8 - copyBits)) << dstBitInByte);
          destination[dstByte] = (byte)((destination[dstByte] & ~mask) | (srcValue << dstBitInByte));
        }
        else
        {
          // var lowMask = (byte)(0xFF >> (8 - availableBits));
          destination[dstByte] |= (byte)(srcValue << dstBitInByte);

          var highBits = copyBits - availableBits;
          var highValue = (byte)(srcValue >> availableBits);
          var highMask = (byte)(0xFF >> (8 - highBits));
          destination[dstByte + 1] |= (byte)(highValue & highMask);
        }
      }

      srcBit += copyBits;
      dstBit += copyBits;
      remaining -= copyBits;
    }

    var validBits = bitCount & 0x07;
    if (validBits != 0)
    {
      var lastByte = bitCount >> 3;
      destination[lastByte] &= (byte)(0xFF >> (8 - validBits));
    }
  }

  #endregion
}

#region 异常处理

/// <summary>
///   BitSpan/ReadOnlyBitSpan internal exception helper class with detailed parameter info.
/// </summary>
internal class BitSpanException
{
  [MethodImpl(MethodImplOptions.NoInlining)]
  public static void ThrowInvalidSliceArguments(int startBit, int length, int totalLength)
  {
    throw new ArgumentOutOfRangeException(
      $"Invalid slice arguments: startBit={startBit}, length={length}, Length={totalLength}");
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  public static void ThrowInvalidRange(Range range, int totalLength)
  {
    throw new ArgumentOutOfRangeException(
      $"Invalid range: {range}, Length={totalLength}");
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  public static void ThrowIndexOutOfRange(int index, int totalLength)
  {
    throw new ArgumentOutOfRangeException(nameof(index), index, $"Index out of range. Length={totalLength}");
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  public static void ThrowLengthMismatch(int length1, int length2)
  {
    throw new ArgumentException($"BitSpan length mismatch: {length1} != {length2}");
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  public static void ThrowDestinationTooShort(int destLength, int requiredLength)
  {
    throw new ArgumentException($"Destination span is too short: {destLength} < {requiredLength}");
  }
}

#endregion