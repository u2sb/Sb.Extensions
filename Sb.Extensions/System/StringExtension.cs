// ReSharper disable RedundantUsingDirective

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Sb.Extensions.System;

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
    public byte[] EncodingToBytes(Encoding? encoding = null)
    {
      if (s.IsNullOrEmpty()) return [];

      encoding ??= Encoding.Default;

      return encoding.GetBytes(s ?? string.Empty);
    }

    #endregion

    #region TryParse

    /// <summary>
    ///   判断字符串是否为 byte
    /// </summary>
    /// <param name="styles"></param>
    /// <param name="provider"></param>
    /// <returns>判断结果</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsByte(NumberStyles styles = NumberStyles.Integer, IFormatProvider? provider = null)
    {
      return byte.TryParse(s, styles, provider, out _);
    }

    /// <summary>
    ///   尝试将字符串转换为 byte
    /// </summary>
    /// <param name="result">转换结果</param>
    /// <param name="styles"></param>
    /// <param name="provider"></param>
    /// <returns>是否转换成功</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryParseToByte(out byte result, NumberStyles styles = NumberStyles.Integer,
      IFormatProvider? provider = null)
    {
      return byte.TryParse(s, styles, provider, out result);
    }

    /// <summary>
    ///   判断字符串是否为 sbyte
    /// </summary>
    /// <param name="styles"></param>
    /// <param name="provider"></param>
    /// <returns>判断结果</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSByte(NumberStyles styles = NumberStyles.Integer, IFormatProvider? provider = null)
    {
      return sbyte.TryParse(s, styles, provider, out _);
    }

    /// <summary>
    ///   尝试将字符串转换为 sbyte
    /// </summary>
    /// <param name="result">转换结果</param>
    /// <param name="styles"></param>
    /// <param name="provider"></param>
    /// <returns>是否转换成功</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryParseToSByte(out sbyte result, NumberStyles styles = NumberStyles.Integer,
      IFormatProvider? provider = null)
    {
      return sbyte.TryParse(s, styles, provider, out result);
    }

    /// <summary>
    ///   判断字符串是否为 ushort
    /// </summary>
    /// <param name="styles"></param>
    /// <param name="provider"></param>
    /// <returns>判断结果</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsInt16(NumberStyles styles = NumberStyles.Integer, IFormatProvider? provider = null)
    {
      return short.TryParse(s, styles, provider, out _);
    }

    /// <summary>
    ///   尝试将字符串转换为 short
    /// </summary>
    /// <param name="result">转换结果</param>
    /// <param name="styles"></param>
    /// <param name="provider"></param>
    /// <returns>是否转换成功</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryParseToInt16(out short result, NumberStyles styles = NumberStyles.Integer,
      IFormatProvider? provider = null)
    {
      return short.TryParse(s, styles, provider, out result);
    }


    /// <summary>
    ///   判断字符串是否为 ushort
    /// </summary>
    /// <param name="styles"></param>
    /// <param name="provider"></param>
    /// <returns>判断结果</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsUInt16(NumberStyles styles = NumberStyles.Integer, IFormatProvider? provider = null)
    {
      return ushort.TryParse(s, styles, provider, out _);
    }

    /// <summary>
    ///   尝试将字符串转换为 ushort
    /// </summary>
    /// <param name="result">转换结果</param>
    /// <param name="styles"></param>
    /// <param name="provider"></param>
    /// <returns>是否转换成功</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryParseToUInt16(out ushort result, NumberStyles styles = NumberStyles.Integer,
      IFormatProvider? provider = null)
    {
      return ushort.TryParse(s, styles, provider, out result);
    }


    /// <summary>
    ///   判断字符串是否为 int
    /// </summary>
    /// <param name="styles"></param>
    /// <param name="provider"></param>
    /// <returns>判断结果</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsInt32(NumberStyles styles = NumberStyles.Integer, IFormatProvider? provider = null)
    {
      return int.TryParse(s, styles, provider, out _);
    }

    /// <summary>
    ///   尝试将字符串转换为 int
    /// </summary>
    /// <param name="result">转换结果</param>
    /// <param name="styles"></param>
    /// <param name="provider"></param>
    /// <returns>是否转换成功</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryParseToInt32(out int result, NumberStyles styles = NumberStyles.Integer,
      IFormatProvider? provider = null)
    {
      return int.TryParse(s, styles, provider, out result);
    }

    /// <summary>
    ///   判断字符串是否为 uint
    /// </summary>
    /// <param name="styles"></param>
    /// <param name="provider"></param>
    /// <returns>判断结果</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsUInt32(NumberStyles styles = NumberStyles.Integer, IFormatProvider? provider = null)
    {
      return uint.TryParse(s, styles, provider, out _);
    }

    /// <summary>
    ///   尝试将字符串转换为 uint
    /// </summary>
    /// <param name="result">转换结果</param>
    /// <param name="styles"></param>
    /// <param name="provider"></param>
    /// <returns>是否转换成功</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryParseToUInt32(out uint result, NumberStyles styles = NumberStyles.Integer,
      IFormatProvider? provider = null)
    {
      return uint.TryParse(s, styles, provider, out result);
    }

    /// <summary>
    ///   判断字符串是否为 long
    /// </summary>
    /// <param name="styles"></param>
    /// <param name="provider"></param>
    /// <returns>判断结果</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsInt64(NumberStyles styles = NumberStyles.Integer, IFormatProvider? provider = null)
    {
      return long.TryParse(s, styles, provider, out _);
    }

    /// <summary>
    ///   尝试将字符串转换为 long
    /// </summary>
    /// <param name="result">转换结果</param>
    /// <param name="styles"></param>
    /// <param name="provider"></param>
    /// <returns>是否转换成功</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryParseToInt64(out long result, NumberStyles styles = NumberStyles.Integer,
      IFormatProvider? provider = null)
    {
      return long.TryParse(s, styles, provider, out result);
    }

    /// <summary>
    ///   判断字符串是否为 ulong
    /// </summary>
    /// <param name="styles"></param>
    /// <param name="provider"></param>
    /// <returns>判断结果</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsUInt64(NumberStyles styles = NumberStyles.Integer, IFormatProvider? provider = null)
    {
      return ulong.TryParse(s, styles, provider, out _);
    }

    /// <summary>
    ///   尝试将字符串转换为 ulong
    /// </summary>
    /// <param name="result">转换结果</param>
    /// <param name="styles"></param>
    /// <param name="provider"></param>
    /// <returns>是否转换成功</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryParseToUInt64(out ulong result, NumberStyles styles = NumberStyles.Integer,
      IFormatProvider? provider = null)
    {
      return ulong.TryParse(s, styles, provider, out result);
    }

    /// <summary>
    ///   判断字符串是否为 float
    /// </summary>
    /// <param name="styles"></param>
    /// <param name="provider"></param>
    /// <returns>判断结果</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsFloat(NumberStyles styles = NumberStyles.Float | NumberStyles.AllowThousands,
      IFormatProvider? provider = null)
    {
      return float.TryParse(s, styles, provider, out _);
    }

    /// <summary>
    ///   尝试将字符串转换为 float
    /// </summary>
    /// <param name="result">转换结果</param>
    /// <param name="styles"></param>
    /// <param name="provider"></param>
    /// <returns>是否转换成功</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryParseToFloat(out float result,
      NumberStyles styles = NumberStyles.Float | NumberStyles.AllowThousands,
      IFormatProvider? provider = null)
    {
      return float.TryParse(s, styles, provider, out result);
    }

    /// <summary>
    ///   判断字符串是否为 double
    /// </summary>
    /// <param name="styles"></param>
    /// <param name="provider"></param>
    /// <returns>判断结果</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsDouble(NumberStyles styles = NumberStyles.Float | NumberStyles.AllowThousands,
      IFormatProvider? provider = null)
    {
      return double.TryParse(s, styles, provider, out _);
    }

    /// <summary>
    ///   尝试将字符串转换为 double
    /// </summary>
    /// <param name="result">转换结果</param>
    /// <param name="styles"></param>
    /// <param name="provider"></param>
    /// <returns>是否转换成功</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryParseToDouble(out double result,
      NumberStyles styles = NumberStyles.Float | NumberStyles.AllowThousands,
      IFormatProvider? provider = null)
    {
      return double.TryParse(s, styles, provider, out result);
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