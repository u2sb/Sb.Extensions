using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Sb.Extensions.System;

#region 大小端枚举

// ReSharper disable InconsistentNaming
/// <summary>
///   大小端编码方式
/// </summary>
public enum BigAndSmallEndianEncodingMode : byte
{
  /// <summary>
  ///   小端模式
  /// </summary>
  DCBA = 0,
  
  /// <summary>
  ///   大端模式
  /// </summary>
  ABCD = 1,

  /// <summary>
  ///   前后顺序不变 二字节内部翻转
  /// </summary>
  BADC = 2,

  /// <summary>
  ///   二字节内部不变 前后顺序翻转
  /// </summary>
  CDAB = 3
}

// ReSharper restore InconsistentNaming

#endregion

/// <summary>
///   转换类
/// </summary>
public static class SbBitConverter
{
  /// <summary>
  ///   byte 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(byte source)
  {
    /// <summary>
    ///   应用大小端
    /// </summary>
    /// <param name="useBigEndianMode"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte WithEndianness(bool useBigEndianMode = false)
    {
      return source;
    }
  }

  /// <summary>
  ///   byte 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(scoped ref byte source)
  {
    /// <summary>
    ///   应用大小端
    /// </summary>
    /// <param name="useBigEndianMode"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ApplyEndianness(bool useBigEndianMode = false)
    {
    }
  }

  /// <summary>
  ///   byte 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(scoped in byte source)
  {
    /// <summary>
    ///   解释为 ReadOnlyByteSpan
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> AsReadOnlyByteSpan()
    {
      return SpanExtension.CreateReadOnlySpan(in source);
    }
  }

  /// <summary>
  ///   sbyte 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(sbyte source)
  {
    /// <summary>
    ///   应用大小端
    /// </summary>
    /// <param name="useBigEndianMode"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sbyte WithEndianness(bool useBigEndianMode = false)
    {
      return source;
    }
  }

  /// <summary>
  ///   sbyte 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(scoped ref sbyte source)
  {
    /// <summary>
    ///   应用大小端
    /// </summary>
    /// <param name="useBigEndianMode"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ApplyEndianness(bool useBigEndianMode = false)
    {
    }
  }

  /// <summary>
  ///   sbyte 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(scoped in sbyte source)
  {
    /// <summary>
    ///   解释为 ReadOnlyByteSpan
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> AsReadOnlyByteSpan()
    {
      return MemoryMarshal.AsBytes(SpanExtension.CreateReadOnlySpan(in source));
    }
  }

  /// <summary>
  ///   short 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(short source)
  {
    /// <summary>
    ///   应用大小端
    /// </summary>
    /// <param name="useBigEndianMode"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short WithEndianness(bool useBigEndianMode = false)
    {
      return BitConverter.IsLittleEndian != useBigEndianMode
        ? source
        : BinaryPrimitives.ReverseEndianness(source);
    }
  }

  /// <summary>
  ///   short 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(scoped ref short source)
  {
    /// <summary>
    ///   应用大小端
    /// </summary>
    /// <param name="useBigEndianMode"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ApplyEndianness(bool useBigEndianMode = false)
    {
      source = source.WithEndianness(useBigEndianMode);
    }
  }

  /// <summary>
  ///   short 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(scoped in short source)
  {
    /// <summary>
    ///   解释为 ReadOnlyByteSpan
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> AsReadOnlyByteSpan()
    {
      return MemoryMarshal.AsBytes(SpanExtension.CreateReadOnlySpan(in source));
    }
  }

  /// <summary>
  ///   ushort 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(ushort source)
  {
    /// <summary>
    ///   应用大小端
    /// </summary>
    /// <param name="useBigEndianMode"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort WithEndianness(bool useBigEndianMode = false)
    {
      return BitConverter.IsLittleEndian != useBigEndianMode
        ? source
        : BinaryPrimitives.ReverseEndianness(source);
    }
  }

  /// <summary>
  ///   ushort 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(scoped ref ushort source)
  {
    /// <summary>
    ///   应用大小端
    /// </summary>
    /// <param name="useBigEndianMode"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ApplyEndianness(bool useBigEndianMode = false)
    {
      source = source.WithEndianness(useBigEndianMode);
    }
  }

  /// <summary>
  ///   ushort 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(scoped in ushort source)
  {
    /// <summary>
    ///   解释为 ReadOnlyByteSpan
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> AsReadOnlyByteSpan()
    {
      return MemoryMarshal.AsBytes(SpanExtension.CreateReadOnlySpan(in source));
    }
  }

  /// <summary>
  ///   int 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(int source)
  {
    /// <summary>
    ///   应用大小端
    /// </summary>
    /// <param name="useBigEndianMode"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int WithEndianness(bool useBigEndianMode = false)
    {
      return BitConverter.IsLittleEndian != useBigEndianMode
        ? source
        : BinaryPrimitives.ReverseEndianness(source);
    }
  }

  /// <summary>
  ///   int 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(scoped ref int source)
  {
    /// <summary>
    ///   应用大小端
    /// </summary>
    /// <param name="useBigEndianMode"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ApplyEndianness(bool useBigEndianMode = false)
    {
      source = source.WithEndianness(useBigEndianMode);
    }
  }

  /// <summary>
  ///   int 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(scoped in int source)
  {
    /// <summary>
    ///   解释为 ReadOnlyByteSpan
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> AsReadOnlyByteSpan()
    {
      return MemoryMarshal.AsBytes(SpanExtension.CreateReadOnlySpan(in source));
    }
  }


  /// <summary>
  ///   uint 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(uint source)
  {
    /// <summary>
    ///   应用大小端
    /// </summary>
    /// <param name="useBigEndianMode"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint WithEndianness(bool useBigEndianMode = false)
    {
      return BitConverter.IsLittleEndian != useBigEndianMode
        ? source
        : BinaryPrimitives.ReverseEndianness(source);
    }
  }

  /// <summary>
  ///   uint 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(scoped ref uint source)
  {
    /// <summary>
    ///   应用大小端
    /// </summary>
    /// <param name="useBigEndianMode"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ApplyEndianness(bool useBigEndianMode = false)
    {
      source = source.WithEndianness(useBigEndianMode);
    }
  }

  /// <summary>
  ///   uint 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(scoped in uint source)
  {
    /// <summary>
    ///   解释为 ReadOnlyByteSpan
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> AsReadOnlyByteSpan()
    {
      return MemoryMarshal.AsBytes(SpanExtension.CreateReadOnlySpan(in source));
    }
  }

  /// <summary>
  ///   long 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(long source)
  {
    /// <summary>
    ///   应用大小端
    /// </summary>
    /// <param name="useBigEndianMode"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long WithEndianness(bool useBigEndianMode = false)
    {
      return BitConverter.IsLittleEndian != useBigEndianMode
        ? source
        : BinaryPrimitives.ReverseEndianness(source);
    }
  }

  /// <summary>
  ///   long 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(scoped ref long source)
  {
    /// <summary>
    ///   应用大小端
    /// </summary>
    /// <param name="useBigEndianMode"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ApplyEndianness(bool useBigEndianMode = false)
    {
      source = source.WithEndianness(useBigEndianMode);
    }
  }

  /// <summary>
  ///   long 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(scoped in long source)
  {
    /// <summary>
    ///   解释为 ReadOnlyByteSpan
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> AsReadOnlyByteSpan()
    {
      return MemoryMarshal.AsBytes(SpanExtension.CreateReadOnlySpan(in source));
    }
  }

  /// <summary>
  ///   ulong 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(ulong source)
  {
    /// <summary>
    ///   应用大小端
    /// </summary>
    /// <param name="useBigEndianMode"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong WithEndianness(bool useBigEndianMode = false)
    {
      return BitConverter.IsLittleEndian != useBigEndianMode
        ? source
        : BinaryPrimitives.ReverseEndianness(source);
    }
  }

  /// <summary>
  ///   ulong 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(scoped ref ulong source)
  {
    /// <summary>
    ///   应用大小端
    /// </summary>
    /// <param name="useBigEndianMode"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ApplyEndianness(bool useBigEndianMode = false)
    {
      source = source.WithEndianness(useBigEndianMode);
    }
  }

  /// <summary>
  ///   ulong 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(scoped in ulong source)
  {
    /// <summary>
    ///   解释为 ReadOnlyByteSpan
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> AsReadOnlyByteSpan()
    {
      return MemoryMarshal.AsBytes(SpanExtension.CreateReadOnlySpan(in source));
    }
  }

  /// <summary>
  ///   float 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(float source)
  {
    /// <summary>
    ///   应用大小端
    /// </summary>
    /// <param name="useBigEndianMode"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float WithEndianness(bool useBigEndianMode = false)
    {
      var span = source.AsReadOnlyByteSpan();
      return span.ToT<float>(useBigEndianMode);
    }
  }

  /// <summary>
  ///   float 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(scoped ref float source)
  {
    /// <summary>
    ///   应用大小端
    /// </summary>
    /// <param name="useBigEndianMode"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ApplyEndianness(bool useBigEndianMode = false)
    {
      if (BitConverter.IsLittleEndian == useBigEndianMode)
      {
        var span = source.AsByteSpan();
        span.ApplyEndianness(useBigEndianMode
          ? BigAndSmallEndianEncodingMode.ABCD
          : BigAndSmallEndianEncodingMode.DCBA);
      }
    }
  }

  /// <summary>
  ///   float 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(scoped in float source)
  {
    /// <summary>
    ///   解释为 ReadOnlyByteSpan
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> AsReadOnlyByteSpan()
    {
      return MemoryMarshal.AsBytes(SpanExtension.CreateReadOnlySpan(in source));
    }
  }

  /// <summary>
  ///   double 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(double source)
  {
    /// <summary>
    ///   应用大小端
    /// </summary>
    /// <param name="useBigEndianMode"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double WithEndianness(bool useBigEndianMode = false)
    {
      var span = source.AsReadOnlyByteSpan();
      return span.ToT<double>(useBigEndianMode);
    }
  }

  /// <summary>
  ///   double 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(scoped ref double source)
  {
    /// <summary>
    ///   应用大小端
    /// </summary>
    /// <param name="useBigEndianMode"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ApplyEndianness(bool useBigEndianMode = false)
    {
      if (BitConverter.IsLittleEndian == useBigEndianMode)
      {
        var span = source.AsByteSpan();
        span.ApplyEndianness(useBigEndianMode
          ? BigAndSmallEndianEncodingMode.ABCD
          : BigAndSmallEndianEncodingMode.DCBA);
      }
    }
  }

  /// <summary>
  ///   double 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(scoped in double source)
  {
    /// <summary>
    ///   解释为 ReadOnlyByteSpan
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> AsReadOnlyByteSpan()
    {
      return MemoryMarshal.AsBytes(SpanExtension.CreateReadOnlySpan(in source));
    }
  }


  /// <summary>
  ///   Memory 转换
  /// </summary>
  /// <param name="source"></param>
  /// <typeparam name="TFrom"></typeparam>
  extension<TFrom>(in Memory<TFrom> source) where TFrom : unmanaged
  {
    /// <summary>
    ///   Memory 转换
    /// </summary>
    /// <typeparam name="TTo"></typeparam>
    /// <returns></returns>
    public Memory<TTo> Cast<TTo>()
      where TTo : unmanaged
    {
      if (typeof(TFrom) == typeof(TTo))
        return (Memory<TTo>)(object)source;

      return new CastMemoryManager<TFrom, TTo>(source).Memory;
    }
  }

  /// <summary>
  ///   通用 转换
  /// </summary>
  /// <param name="source"></param>
  /// <typeparam name="T"></typeparam>
  extension<T>(scoped ref T source) where T : unmanaged
  {
    /// <summary>
    ///   解释为为 ByteSpan
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> AsByteSpan()
    {
      return SpanExtension.CreateSpan(ref Unsafe.As<T, byte>(ref source), Unsafe.SizeOf<T>());
    }

    /// <summary>
    ///   解释为 ReadOnlyByteSpan
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> AsReadOnlyByteSpan()
    {
      return MemoryMarshal.AsBytes(SpanExtension.CreateReadOnlySpan(in source));
    }
  }

  /// <summary>
  ///   通用 转换
  /// </summary>
  /// <param name="source"></param>
  /// <typeparam name="T"></typeparam>
  extension<T>(T source) where T : unmanaged
  {
    /// <summary>
    ///   转换到 byte[]
    /// </summary>
    /// <param name="useBigEndianMode">是否使用大端模式</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] ToByteArray(bool useBigEndianMode = false)
    {
      return ToByteArray(source,
        useBigEndianMode ? BigAndSmallEndianEncodingMode.ABCD : BigAndSmallEndianEncodingMode.DCBA);
    }

    /// <summary>
    ///   转换到 byte[]
    /// </summary>
    /// <param name="mode"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] ToByteArray(BigAndSmallEndianEncodingMode mode)
    {
      var size = Unsafe.SizeOf<T>();
      var result = new byte[size];
      WriteTo(source, result.AsSpan(), mode);
      return result;
    }

    /// <summary>
    ///   转换到 byte[]
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="mode"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteTo(scoped in Span<byte> destination, BigAndSmallEndianEncodingMode mode)
    {
      var size = Unsafe.SizeOf<T>();
#if NET8_0_OR_GREATER
      MemoryMarshal.Write(destination, in source);
#else
      MemoryMarshal.Write(destination, ref source);
#endif
      destination[..size].ApplyEndianness(mode);
    }
  }

  /// <summary>
  ///   ReadOnlySpan 拓展
  ///   <param name="source"></param>
  /// </summary>
  extension(scoped in ReadOnlySpan<byte> source)
  {
    /// <summary>
    ///   转换为 short 类型
    /// </summary>
    /// <param name="useBigEndianMode">是否使用大端模式</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short ToInt16(bool useBigEndianMode = false)
    {
      return useBigEndianMode
        ? BinaryPrimitives.ReadInt16BigEndian(source)
        : BinaryPrimitives.ReadInt16LittleEndian(source);
    }

    /// <summary>
    ///   转换为 ushort 类型
    /// </summary>
    /// <param name="useBigEndianMode">是否使用大端模式</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort ToUInt16(bool useBigEndianMode = false)
    {
      return useBigEndianMode
        ? BinaryPrimitives.ReadUInt16BigEndian(source)
        : BinaryPrimitives.ReadUInt16LittleEndian(source);
    }

    /// <summary>
    ///   转换为 int 类型
    /// </summary>
    /// <param name="useBigEndianMode">是否使用大端模式</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ToInt32(bool useBigEndianMode = false)
    {
      return useBigEndianMode
        ? BinaryPrimitives.ReadInt32BigEndian(source)
        : BinaryPrimitives.ReadInt32LittleEndian(source);
    }

    /// <summary>
    ///   转换为 uint 类型
    /// </summary>
    /// <param name="useBigEndianMode">是否使用大端模式</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ToUInt32(bool useBigEndianMode = false)
    {
      return useBigEndianMode
        ? BinaryPrimitives.ReadUInt32BigEndian(source)
        : BinaryPrimitives.ReadUInt32LittleEndian(source);
    }

    /// <summary>
    ///   转换为long类型
    /// </summary>
    /// <param name="useBigEndianMode">是否使用大端模式</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ToInt64(bool useBigEndianMode = false)
    {
      return useBigEndianMode
        ? BinaryPrimitives.ReadInt64BigEndian(source)
        : BinaryPrimitives.ReadInt64LittleEndian(source);
    }

    /// <summary>
    ///   转换为 ulong 类型
    /// </summary>
    /// <param name="useBigEndianMode">是否使用大端模式</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ToUInt64(bool useBigEndianMode = false)
    {
      return useBigEndianMode
        ? BinaryPrimitives.ReadUInt64BigEndian(source)
        : BinaryPrimitives.ReadUInt64LittleEndian(source);
    }

    /// <summary>
    ///   转换为 float 类型
    /// </summary>
    /// <param name="useBigEndianMode">是否使用大端模式</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ToSingle(bool useBigEndianMode = false)
    {
#if NET6_0_OR_GREATER
      return useBigEndianMode
        ? BinaryPrimitives.ReadSingleBigEndian(source)
        : BinaryPrimitives.ReadSingleLittleEndian(source);
#else
      return ToT<float>(source, useBigEndianMode);
#endif
    }

    /// <summary>
    ///   转换为 double 类型
    /// </summary>
    /// <param name="useBigEndianMode">是否使用大端模式</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double ToDouble(bool useBigEndianMode = false)
    {
#if NET6_0_OR_GREATER
      return useBigEndianMode
        ? BinaryPrimitives.ReadDoubleBigEndian(source)
        : BinaryPrimitives.ReadDoubleLittleEndian(source);
#else
      return ToT<double>(source, useBigEndianMode);
#endif
    }

    /// <summary>
    ///   转换到T
    /// </summary>
    /// <param name="useBigEndianMode"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T ToT<T>(bool useBigEndianMode = false) where T : unmanaged
    {
      return ToT<T>(source, useBigEndianMode ? BigAndSmallEndianEncodingMode.ABCD : BigAndSmallEndianEncodingMode.DCBA);
    }

    /// <summary>
    ///   转换到T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="mode"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T ToT<T>(BigAndSmallEndianEncodingMode mode) where T : unmanaged
    {
      T value = default;
      source.WriteTo(ref value, mode);
      return value;
    }

    /// <summary>
    ///   写入到 T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="mode"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteTo<T>(ref T destination, BigAndSmallEndianEncodingMode mode) where T : unmanaged
    {
      destination = MemoryMarshal.Read<T>(source);
      var span = AsByteSpan(ref destination);
      span.ApplyEndianness(mode);
    }

#if NET8_0_OR_GREATER
    /// <summary>
    ///   转换为 Int128 类型
    /// </summary>
    /// <param name="useBigEndianMode">是否使用大端模式</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Int128 ToInt128(bool useBigEndianMode = false)
    {
      return useBigEndianMode
        ? BinaryPrimitives.ReadInt128BigEndian(source)
        : BinaryPrimitives.ReadInt128LittleEndian(source);
    }

    /// <summary>
    ///   转换为 UInt128 类型
    /// </summary>
    /// <param name="useBigEndianMode">是否使用大端模式</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public UInt128 ToUInt128(bool useBigEndianMode = false)
    {
      return useBigEndianMode
        ? BinaryPrimitives.ReadUInt128BigEndian(source)
        : BinaryPrimitives.ReadUInt128LittleEndian(source);
    }

#endif
  }

  /// <summary>
  ///   ReadOnlySpan 拓展
  /// </summary>
  /// <param name="source"></param>
  /// <typeparam name="T"></typeparam>
  extension<T>(scoped in ReadOnlySpan<T> source) where T : unmanaged
  {
    /// <summary>
    ///   解释为Span
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan()
    {
      return SpanExtension.CreateSpan(ref MemoryMarshal.GetReference(source), source.Length);
    }
  }

  /// <summary>
  ///   Span 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(scoped in Span<byte> source)
  {
    /// <summary>
    ///   大小端转换
    /// </summary>
    /// <param name="mode"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ApplyEndianness(BigAndSmallEndianEncodingMode mode)
    {
      // 如果是单字节，也就是 byte 类型，直接返回
      var size = source.Length;
      if (size == 1) return;

      if (source.Length % 2 != 0)
        throw new ArgumentException("Data length must be even.");

      // 小端模式 初始为 DCBA
      if (BitConverter.IsLittleEndian)
        switch (mode)
        {
          case BigAndSmallEndianEncodingMode.DCBA:
            break;
          case BigAndSmallEndianEncodingMode.ABCD:
            source.Reverse();
            break;

          // 二字节翻转，前后不翻转 DCBA -> BADC
          case BigAndSmallEndianEncodingMode.BADC:
            // 解释为ushort，然后整体翻转
            var us = MemoryMarshal.Cast<byte, ushort>(source);
            us.Reverse();
            break;

          // 二字节不翻转，前后翻转 DCBA -> CDAB
          case BigAndSmallEndianEncodingMode.CDAB:
            var ushortSpan = MemoryMarshal.Cast<byte, ushort>(source);
            foreach (ref var value in ushortSpan) value = BinaryPrimitives.ReverseEndianness(value);
            break;
          default:
            throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
      // 大端模式 初始为 ABCD
      else
        switch (mode)
        {
          case BigAndSmallEndianEncodingMode.DCBA:
            source.Reverse();
            break;
          case BigAndSmallEndianEncodingMode.ABCD:
            break;

          // 二字节翻转，前后不翻转 ABCD -> BADC
          case BigAndSmallEndianEncodingMode.BADC:
            var ushortSpan = MemoryMarshal.Cast<byte, ushort>(source);
            foreach (ref var value in ushortSpan) value = BinaryPrimitives.ReverseEndianness(value);
            break;

          // 二字节不翻转，前后翻转 ABCD -> CDAB
          case BigAndSmallEndianEncodingMode.CDAB:
            // 解释为ushort，然后整体翻转
            var us = MemoryMarshal.Cast<byte, ushort>(source);
            us.Reverse();
            break;
          default:
            throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }


    /// <summary>
    ///   转换为 short 类型
    /// </summary>
    /// <param name="useBigEndianMode">是否使用大端模式</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short ToInt16(bool useBigEndianMode = false)
    {
      return useBigEndianMode
        ? BinaryPrimitives.ReadInt16BigEndian(source)
        : BinaryPrimitives.ReadInt16LittleEndian(source);
    }

    /// <summary>
    ///   转换为 ushort 类型
    /// </summary>
    /// <param name="useBigEndianMode">是否使用大端模式</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort ToUInt16(bool useBigEndianMode = false)
    {
      return useBigEndianMode
        ? BinaryPrimitives.ReadUInt16BigEndian(source)
        : BinaryPrimitives.ReadUInt16LittleEndian(source);
    }

    /// <summary>
    ///   转换为 int 类型
    /// </summary>
    /// <param name="useBigEndianMode">是否使用大端模式</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ToInt32(bool useBigEndianMode = false)
    {
      return useBigEndianMode
        ? BinaryPrimitives.ReadInt32BigEndian(source)
        : BinaryPrimitives.ReadInt32LittleEndian(source);
    }

    /// <summary>
    ///   转换为 uint 类型
    /// </summary>
    /// <param name="useBigEndianMode">是否使用大端模式</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ToUInt32(bool useBigEndianMode = false)
    {
      return useBigEndianMode
        ? BinaryPrimitives.ReadUInt32BigEndian(source)
        : BinaryPrimitives.ReadUInt32LittleEndian(source);
    }

    /// <summary>
    ///   转换为long类型
    /// </summary>
    /// <param name="useBigEndianMode">是否使用大端模式</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ToInt64(bool useBigEndianMode = false)
    {
      return useBigEndianMode
        ? BinaryPrimitives.ReadInt64BigEndian(source)
        : BinaryPrimitives.ReadInt64LittleEndian(source);
    }

    /// <summary>
    ///   转换为 ulong 类型
    /// </summary>
    /// <param name="useBigEndianMode">是否使用大端模式</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ToUInt64(bool useBigEndianMode = false)
    {
      return useBigEndianMode
        ? BinaryPrimitives.ReadUInt64BigEndian(source)
        : BinaryPrimitives.ReadUInt64LittleEndian(source);
    }

    /// <summary>
    ///   转换为 float 类型
    /// </summary>
    /// <param name="useBigEndianMode">是否使用大端模式</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ToSingle(bool useBigEndianMode = false)
    {
#if NET6_0_OR_GREATER
      return useBigEndianMode
        ? BinaryPrimitives.ReadSingleBigEndian(source)
        : BinaryPrimitives.ReadSingleLittleEndian(source);
#else
      return ToT<float>(source, useBigEndianMode);
#endif
    }

    /// <summary>
    ///   转换为 double 类型
    /// </summary>
    /// <param name="useBigEndianMode">是否使用大端模式</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double ToDouble(bool useBigEndianMode = false)
    {
#if NET6_0_OR_GREATER
      return useBigEndianMode
        ? BinaryPrimitives.ReadDoubleBigEndian(source)
        : BinaryPrimitives.ReadDoubleLittleEndian(source);
#else
      return ToT<double>(source, useBigEndianMode);
#endif
    }

    /// <summary>
    ///   转换到T
    /// </summary>
    /// <param name="useBigEndianMode"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T ToT<T>(bool useBigEndianMode = false) where T : unmanaged
    {
      return ToT<T>(source, useBigEndianMode ? BigAndSmallEndianEncodingMode.ABCD : BigAndSmallEndianEncodingMode.DCBA);
    }

    /// <summary>
    ///   转换到T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="mode"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T ToT<T>(BigAndSmallEndianEncodingMode mode) where T : unmanaged
    {
      T value = default;
      source.WriteTo(ref value, mode);
      return value;
    }

    /// <summary>
    ///   写入到 T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="mode"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteTo<T>(scoped ref T destination, BigAndSmallEndianEncodingMode mode) where T : unmanaged
    {
      destination = MemoryMarshal.Read<T>(source);
      var span = AsByteSpan(ref destination);
      span.ApplyEndianness(mode);
    }

#if NET8_0_OR_GREATER
    /// <summary>
    ///   转换为 Int128 类型
    /// </summary>
    /// <param name="useBigEndianMode">是否使用大端模式</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Int128 ToInt128(bool useBigEndianMode = false)
    {
      return useBigEndianMode
        ? BinaryPrimitives.ReadInt128BigEndian(source)
        : BinaryPrimitives.ReadInt128LittleEndian(source);
    }

    /// <summary>
    ///   转换为 UInt128 类型
    /// </summary>
    /// <param name="useBigEndianMode">是否使用大端模式</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public UInt128 ToUInt128(bool useBigEndianMode = false)
    {
      return useBigEndianMode
        ? BinaryPrimitives.ReadUInt128BigEndian(source)
        : BinaryPrimitives.ReadUInt128LittleEndian(source);
    }

#endif
  }

  #region Memory转换

  /// <summary>
  /// </summary>
  /// <typeparam name="TFrom"></typeparam>
  /// <typeparam name="TTo"></typeparam>
  /// <param name="memory"></param>
  private sealed class CastMemoryManager<TFrom, TTo>(Memory<TFrom> memory) : MemoryManager<TTo>
    where TFrom : unmanaged
    where TTo : unmanaged
  {
    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
    }

    /// <inheritdoc />
    public override Span<TTo> GetSpan()
    {
      return MemoryMarshal.Cast<TFrom, TTo>(memory.Span);
    }

    /// <inheritdoc />
    public override MemoryHandle Pin(int elementIndex = 0)
    {
      var byteOffset = elementIndex * Unsafe.SizeOf<TTo>();
      var shiftedOffset = Math.DivRem(byteOffset, Unsafe.SizeOf<TFrom>(), out var remainder);

      if (remainder != 0)
        throw new ArgumentException("The input index doesn't result in an aligned item access",
          nameof(elementIndex));

      return memory[shiftedOffset..].Pin();
    }

    /// <inheritdoc />
    public override void Unpin()
    {
    }
  }

  #endregion

#if NET8_0_OR_GREATER
  /// <summary>
  ///   Int128 拓展
  /// </summary>
  extension(Int128 source)
  {
    /// <summary>
    ///   应用大小端
    /// </summary>
    /// <param name="useBigEndianMode"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Int128 WithEndianness(bool useBigEndianMode = false)
    {
      return BitConverter.IsLittleEndian != useBigEndianMode
        ? source
        : BinaryPrimitives.ReverseEndianness(source);
    }
  }

  /// <summary>
  ///   Int128 拓展
  /// </summary>
  extension(scoped ref Int128 source)
  {
    /// <summary>
    ///   应用大小端
    /// </summary>
    /// <param name="useBigEndianMode"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ApplyEndianness(bool useBigEndianMode = false)
    {
      source = source.WithEndianness(useBigEndianMode);
    }
  }

  /// <summary>
  ///   Int128 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(scoped in Int128 source)
  {
    /// <summary>
    ///   解释为 ReadOnlyByteSpan
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> AsReadOnlyByteSpan()
    {
      return MemoryMarshal.AsBytes(SpanExtension.CreateReadOnlySpan(in source, 1));
    }
  }

  /// <summary>
  ///   Int128 拓展
  /// </summary>
  extension(UInt128 source)
  {
    /// <summary>
    ///   应用大小端
    /// </summary>
    /// <param name="useBigEndianMode"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public UInt128 WithEndianness(bool useBigEndianMode = false)
    {
      return BitConverter.IsLittleEndian != useBigEndianMode
        ? source
        : BinaryPrimitives.ReverseEndianness(source);
    }
  }

  /// <summary>
  ///   Int128 拓展
  /// </summary>
  extension(scoped ref UInt128 source)
  {
    /// <summary>
    ///   应用大小端
    /// </summary>
    /// <param name="useBigEndianMode"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ApplyEndianness(bool useBigEndianMode = false)
    {
      source = source.WithEndianness(useBigEndianMode);
    }
  }

  /// <summary>
  ///   UInt128 拓展
  /// </summary>
  /// <param name="source"></param>
  extension(scoped in UInt128 source)
  {
    /// <summary>
    ///   解释为 ReadOnlyByteSpan
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> AsReadOnlyByteSpan()
    {
      return MemoryMarshal.AsBytes(SpanExtension.CreateReadOnlySpan(in source, 1));
    }
  }

#endif

  #region 检查长度

  /// <summary>
  ///   检查长度是否符合要求
  /// </summary>
  /// <param name="data">数据</param>
  /// <param name="expectedLength">预期长度</param>
  /// <exception cref="InvalidArrayLengthException"></exception>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void CheckLength(ReadOnlySpan<byte> data, int expectedLength)
  {
    if (data.Length < expectedLength) throw new InvalidArrayLengthException(expectedLength, data.Length);
  }

  /// <summary>
  ///   检查长度是否符合要求
  /// </summary>
  /// <param name="data">数据</param>
  /// <param name="expectedLength">预期长度</param>
  /// <exception cref="InvalidArrayLengthException"></exception>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void CheckLength(Span<byte> data, int expectedLength)
  {
    if (data.Length < expectedLength) throw new InvalidArrayLengthException(expectedLength, data.Length);
  }

  /// <summary>
  ///   检查长度是否符合要求
  /// </summary>
  /// <param name="data"></param>
  /// <param name="expectedLength"></param>
  /// <exception cref="InvalidArrayLengthException"></exception>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void CheckLength(byte[] data, int expectedLength)
  {
    if (data.Length < expectedLength) throw new InvalidArrayLengthException(expectedLength, data.Length);
  }

  #endregion
}

#region 长度错误异常

/// <summary>
///   数组长度和预期不一致错误
/// </summary>
/// <param name="expectedLength">预期长度</param>
/// <param name="actualLength">真实长度</param>
public class InvalidArrayLengthException(int expectedLength, int actualLength)
  : Exception($"Invalid array length. Expected: {expectedLength}, Actual: {actualLength}")
{
  /// <summary>
  ///   预期长度
  /// </summary>
  public int ExpectedLength { get; } = expectedLength;

  /// <summary>
  ///   真实长度
  /// </summary>
  public int ActualLength { get; } = actualLength;
}

#endregion