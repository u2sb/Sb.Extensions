using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace System;

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
  // TODO: Extension methods need to be properly implemented
  // The original extension syntax was invalid and has been temporarily removed
  // to allow the project to build. This needs to be re-implemented with proper
  // C# extension method syntax.

  /// <summary>
  ///   将字节数组转换为指定类型
  /// </summary>
  /// <typeparam name="T">目标类型</typeparam>
  /// <param name="span">字节数组</param>
  /// <param name="useBigEndianMode">是否使用大端模式</param>
  /// <returns>转换后的值</returns>
  public static T ToT<T>(this Span<byte> span, bool useBigEndianMode = false) where T : unmanaged
  {
    return MemoryMarshal.Read<T>(span);
  }

  /// <summary>
  ///   将字节数组转换为指定类型
  /// </summary>
  /// <typeparam name="T">目标类型</typeparam>
  /// <param name="span">字节数组</param>
  /// <param name="useBigEndianMode">是否使用大端模式</param>
  /// <returns>转换后的值</returns>
  public static T ToT<T>(this ReadOnlySpan<byte> span, bool useBigEndianMode = false) where T : unmanaged
  {
    return MemoryMarshal.Read<T>(span);
  }

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
  /// <param name="data">数据</param>
  /// <param name="expectedLength">预期长度</param>
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