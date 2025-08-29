using Xunit;
using System;
using System.Text;

namespace Sb.Extensions.Tests;

public class StringExtensionTests
{
    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("abc", false)]
    public void IsNullOrEmpty_Works(string? input, bool expected)
    {
        Assert.Equal(expected, input.IsNullOrEmpty());
    }
    
    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("   ", true)]
    [InlineData("abc", false)]
    public void IsNullOrWhiteSpace_Works(string? input, bool expected)
    {
        Assert.Equal(expected, input.IsNullOrWhiteSpace());
    }

    [Theory]
    [InlineData("123", true, 123)]
    [InlineData("abc", false, 0)]
    public void TryParseToInt32_Works(string input, bool expected, int value)
    {
        var result = input.TryParseToInt32(out int v);
        Assert.Equal(expected, result);
        if (expected) Assert.Equal(value, v);
    }

    [Theory]
    [InlineData("255", true, (byte)255)]
    [InlineData("-1", false, (byte)0)]
    public void TryParseToByte_Works(string input, bool expected, byte value)
    {
        var result = input.TryParseToByte(out byte v);
        Assert.Equal(expected, result);
        if (expected) Assert.Equal(value, v);
    }

    [Theory]
    [InlineData("123.45", true, 123.45f)]
    [InlineData("abc", false, 0f)]
    public void TryParseToFloat_Works(string input, bool expected, float value)
    {
      var result = input.TryParseToFloat(out float v);
      Assert.Equal(expected, result);
      if (expected) Assert.Equal(value, v);
    }

    [Theory]
    [InlineData("123.45", true, 123.45)]
    [InlineData("abc", false, 0d)]
    public void TryParseToDouble_Works(string input, bool expected, double value)
    {

      var result = input.TryParseToDouble(out double v);
      Assert.Equal(expected, result);
      if (expected) Assert.Equal(value, v);
    }

    [Fact]
    public void GetBytes_DefaultEncoding()
    {
      var s = "abc";
      var bytes = s.EncodingToBytes();
      Assert.Equal(Encoding.Default.GetBytes(s), bytes);
    }

    [Fact]
    public void GetBytes_SpecifiedEncoding()
    {
      var s = "abc";
      var bytes = s.EncodingToBytes(Encoding.UTF8);
      Assert.Equal(Encoding.UTF8.GetBytes(s), bytes);
    }
}
