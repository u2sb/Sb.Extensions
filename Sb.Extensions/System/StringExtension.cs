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
  #region GetBytes

  /// <summary>
  ///   将字符串转换为字节数组
  /// </summary>
  /// <param name="s">字符串</param>
  /// <param name="encoding">编码</param>
  /// <returns>字节数组</returns>
  public static byte[] GetBytes(this string? s, Encoding? encoding = null)
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
  /// <param name="s">字符串</param>
  /// <returns>判断结果</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool IsByte(this string? s)
  {
    return byte.TryParse(s, out _);
  }

  /// <summary>
  ///   尝试将字符串转换为 byte
  /// </summary>
  /// <param name="s">字符串</param>
  /// <param name="result">转换结果</param>
  /// <returns>是否转换成功</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool TryParseToByte(this string? s, out byte result)
  {
    return byte.TryParse(s, out result);
  }

  /// <summary>
  ///   判断字符串是否为 sbyte
  /// </summary>
  /// <param name="s">字符串</param>
  /// <returns>判断结果</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool IsSByte(this string? s)
  {
    return sbyte.TryParse(s, out _);
  }

  /// <summary>
  ///   尝试将字符串转换为 sbyte
  /// </summary>
  /// <param name="s">字符串</param>
  /// <param name="result">转换结果</param>
  /// <returns>是否转换成功</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool TryParseToSByte(this string? s, out sbyte result)
  {
    return sbyte.TryParse(s, out result);
  }

  /// <summary>
  ///   判断字符串是否为 short
  /// </summary>
  /// <param name="s">字符串</param>
  /// <returns>判断结果</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool IsInt16(this string? s)
  {
    return short.TryParse(s, out _);
  }

  /// <summary>
  ///   尝试将字符串转换为 short
  /// </summary>
  /// <param name="s">字符串</param>
  /// <param name="result">转换结果</param>
  /// <returns>是否转换成功</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool TryParseToInt16(this string? s, out short result)
  {
    return short.TryParse(s, out result);
  }

  /// <summary>
  ///   判断字符串是否为 ushort
  /// </summary>
  /// <param name="s">字符串</param>
  /// <returns>判断结果</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool IsUInt16(this string? s)
  {
    return ushort.TryParse(s, out _);
  }

  /// <summary>
  ///   尝试将字符串转换为 ushort
  /// </summary>
  /// <param name="s">字符串</param>
  /// <param name="result">转换结果</param>
  /// <returns>是否转换成功</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool TryParseToUInt16(this string? s, out ushort result)
  {
    return ushort.TryParse(s, out result);
  }

  /// <summary>
  ///   判断字符串是否为 int
  /// </summary>
  /// <param name="s">字符串</param>
  /// <returns>判断结果</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool IsInt32(this string? s)
  {
    return int.TryParse(s, out _);
  }

  /// <summary>
  ///   尝试将字符串转换为 int
  /// </summary>
  /// <param name="s">字符串</param>
  /// <param name="result">转换结果</param>
  /// <returns>是否转换成功</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool TryParseToInt32(this string? s, out int result)
  {
    return int.TryParse(s, out result);
  }

  /// <summary>
  ///   判断字符串是否为 uint
  /// </summary>
  /// <param name="s">字符串</param>
  /// <returns>判断结果</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool IsUInt32(this string? s)
  {
    return uint.TryParse(s, out _);
  }

  /// <summary>
  ///   尝试将字符串转换为 uint
  /// </summary>
  /// <param name="s">字符串</param>
  /// <param name="result">转换结果</param>
  /// <returns>是否转换成功</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool TryParseToUInt32(this string? s, out uint result)
  {
    return uint.TryParse(s, out result);
  }

  /// <summary>
  ///   判断字符串是否为 long
  /// </summary>
  /// <param name="s">字符串</param>
  /// <returns>判断结果</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool IsInt64(this string? s)
  {
    return long.TryParse(s, out _);
  }

  /// <summary>
  ///   尝试将字符串转换为 long
  /// </summary>
  /// <param name="s">字符串</param>
  /// <param name="result">转换结果</param>
  /// <returns>是否转换成功</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool TryParseToInt64(this string? s, out long result)
  {
    return long.TryParse(s, out result);
  }

  /// <summary>
  ///   判断字符串是否为 ulong
  /// </summary>
  /// <param name="s">字符串</param>
  /// <returns>判断结果</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool IsUInt64(this string? s)
  {
    return ulong.TryParse(s, out _);
  }

  /// <summary>
  ///   尝试将字符串转换为 ulong
  /// </summary>
  /// <param name="s">字符串</param>
  /// <param name="result">转换结果</param>
  /// <returns>是否转换成功</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool TryParseToUInt64(this string? s, out ulong result)
  {
    return ulong.TryParse(s, out result);
  }

  /// <summary>
  ///   判断字符串是否为 float
  /// </summary>
  /// <param name="s">字符串</param>
  /// <returns>判断结果</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool IsFloat(this string? s)
  {
    return float.TryParse(s, out _);
  }

  /// <summary>
  ///   尝试将字符串转换为 float
  /// </summary>
  /// <param name="s">字符串</param>
  /// <param name="result">转换结果</param>
  /// <returns>是否转换成功</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool TryParseToFloat(this string? s, out float result)
  {
    return float.TryParse(s, out result);
  }

  /// <summary>
  ///   判断字符串是否为 double
  /// </summary>
  /// <param name="s">字符串</param>
  /// <returns>判断结果</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool IsDouble(this string? s)
  {
    return double.TryParse(s, out _);
  }

  /// <summary>
  ///   尝试将字符串转换为 double
  /// </summary>
  /// <param name="s">字符串</param>
  /// <param name="result">转换结果</param>
  /// <returns>是否转换成功</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool TryParseToDouble(this string? s, out double result)
  {
    return double.TryParse(s, out result);
  }

  #endregion

  #region NullOrEmpty

  /// <summary>
  ///   判断是否为空
  /// </summary>
  /// <param name="s">字符串</param>
  /// <returns></returns>
  public static bool IsNullOrEmpty(this string? s)
  {
    return string.IsNullOrEmpty(s);
  }

  /// <summary>
  ///   判断是否为空或空行
  /// </summary>
  /// <param name="s">字符串</param>
  /// <returns></returns>
  public static bool IsNullOrWhiteSpace(this string? s)
  {
    return string.IsNullOrWhiteSpace(s);
  }

  #endregion
}