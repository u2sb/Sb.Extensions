// ReSharper disable RedundantUsingDirective

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

// 命名空间不要改
// ReSharper disable once CheckNamespace
namespace System;

/// <summary>
///   String 拓展
/// </summary>
public static class StringExtension
{
  /// <summary>
  ///   字符串拓展
  /// </summary>
  /// <param name="s">字符串</param>
  extension(string? s)
  {
    #region GetBytes

    /// <summary>
    ///   将字符串转换为字节数组
    /// </summary>
    /// <returns>字节数组</returns>
    public byte[] GetBytes(Encoding? encoding = null)
    {
      if (s.IsNullOrEmpty()) return [];

      if (encoding == null) encoding = Encoding.Default;

      return encoding.GetBytes(s ?? string.Empty);
    }

    #endregion

    #region TryParse

    /// <summary>
    ///   判断字符串是否为 byte
    /// </summary>
    /// <returns>判断结果</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsByte()
    {
      return byte.TryParse(s, out _);
    }

    /// <summary>
    ///   尝试将字符串转换为 byte
    /// </summary>
    /// <param name="result">转换结果</param>
    /// <returns>是否转换成功</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryParseToByte(out byte result)
    {
      return byte.TryParse(s, out result);
    }

    /// <summary>
    ///   判断字符串是否为 sbyte
    /// </summary>
    /// <returns>判断结果</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSByte()
    {
      return sbyte.TryParse(s, out _);
    }

    /// <summary>
    ///   尝试将字符串转换为 sbyte
    /// </summary>
    /// <param name="result">转换结果</param>
    /// <returns>是否转换成功</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryParseToSByte(out sbyte result)
    {
      return sbyte.TryParse(s, out result);
    }

    /// <summary>
    ///   判断字符串是否为 ushort
    /// </summary>
    /// <returns>判断结果</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsInt16()
    {
      return short.TryParse(s, out _);
    }

    /// <summary>
    ///   尝试将字符串转换为 short
    /// </summary>
    /// <param name="result">转换结果</param>
    /// <returns>是否转换成功</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryParseToInt16(out short result)
    {
      return short.TryParse(s, out result);
    }


    /// <summary>
    ///   判断字符串是否为 ushort
    /// </summary>
    /// <returns>判断结果</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsUInt16()
    {
      return ushort.TryParse(s, out _);
    }

    /// <summary>
    ///   尝试将字符串转换为 ushort
    /// </summary>
    /// <param name="result">转换结果</param>
    /// <returns>是否转换成功</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryParseToUInt16(out ushort result)
    {
      return ushort.TryParse(s, out result);
    }


    /// <summary>
    ///   判断字符串是否为 int
    /// </summary>
    /// <returns>判断结果</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsInt32()
    {
      return int.TryParse(s, out _);
    }

    /// <summary>
    ///   尝试将字符串转换为 int
    /// </summary>
    /// <param name="result">转换结果</param>
    /// <returns>是否转换成功</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryParseToInt32(out int result)
    {
      return int.TryParse(s, out result);
    }

    /// <summary>
    ///   判断字符串是否为 uint
    /// </summary>
    /// <returns>判断结果</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsUInt32()
    {
      return uint.TryParse(s, out _);
    }

    /// <summary>
    ///   尝试将字符串转换为 uint
    /// </summary>
    /// <param name="result">转换结果</param>
    /// <returns>是否转换成功</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryParseToUInt32(out uint result)
    {
      return uint.TryParse(s, out result);
    }

    /// <summary>
    ///   判断字符串是否为 long
    /// </summary>
    /// <returns>判断结果</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsInt64()
    {
      return long.TryParse(s, out _);
    }

    /// <summary>
    ///   尝试将字符串转换为 long
    /// </summary>
    /// <param name="result">转换结果</param>
    /// <returns>是否转换成功</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryParseToInt64(out long result)
    {
      return long.TryParse(s, out result);
    }

    /// <summary>
    ///   判断字符串是否为 ulong
    /// </summary>
    /// <returns>判断结果</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsUInt64()
    {
      return ulong.TryParse(s, out _);
    }

    /// <summary>
    ///   尝试将字符串转换为 ulong
    /// </summary>
    /// <param name="result">转换结果</param>
    /// <returns>是否转换成功</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryParseToUInt64(out ulong result)
    {
      return ulong.TryParse(s, out result);
    }

    /// <summary>
    ///   判断字符串是否为 float
    /// </summary>
    /// <returns>判断结果</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsFloat()
    {
      return float.TryParse(s, out _);
    }

    /// <summary>
    ///   尝试将字符串转换为 float
    /// </summary>
    /// <param name="result">转换结果</param>
    /// <returns>是否转换成功</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryParseToFloat(out float result)
    {
      return float.TryParse(s, out result);
    }

    /// <summary>
    ///   判断字符串是否为 double
    /// </summary>
    /// <returns>判断结果</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsDouble()
    {
      return double.TryParse(s, out _);
    }

    /// <summary>
    ///   尝试将字符串转换为 double
    /// </summary>
    /// <param name="result">转换结果</param>
    /// <returns>是否转换成功</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryParseToDouble(out double result)
    {
      return double.TryParse(s, out result);
    }

    #endregion

    #region NullOrEmpty

    /// <summary>
    ///   判断是否为空
    /// </summary>
    /// <returns></returns>
    public bool IsNullOrEmpty()
    {
      return string.IsNullOrEmpty(s);
    }

    /// <summary>
    ///   判断是否为空或空行
    /// </summary>
    /// <returns></returns>
    public bool IsNullOrWhiteSpace()
    {
      return string.IsNullOrWhiteSpace(s);
    }

    #endregion
  }
}