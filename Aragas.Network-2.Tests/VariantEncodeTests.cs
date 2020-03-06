using Aragas.Network.Data;

using System;

using Xunit;

namespace Aragas.Network_2.Tests
{
    public class VariantEncodeTests : IVariant
    {
        int IVariant.Size => throw new NotImplementedException();
        Span<byte> IVariant.Encode() => throw new NotImplementedException();


        [Fact]
        public void VariantEncode_0_Test()
        {
            Assert.Equal(new byte[] { 0 }, IVariant.Encode(0b_00000000).ToArray());
        }
        [Fact]
        public void VariantEncode_8_Test()
        {
            Assert.Equal(new byte[] { 255, 1 }, IVariant.Encode(0b11111111).ToArray());
        }
        [Fact]
        public void VariantEncode_16_Test()
        {
            Assert.Equal(new byte[] { 255, 255, 1 }, IVariant.Encode(0b01111111_11111111).ToArray());
        }
        [Fact]
        public void VariantEncode_24_Test()
        {
            Assert.Equal(new byte[] { 255, 255, 255, 1 }, IVariant.Encode(0b00111111_11111111_11111111).ToArray());
        }
        [Fact]
        public void VariantEncode_32_Test()
        {
            Assert.Equal(new byte[] { 255, 255, 255, 255, 1 }, IVariant.Encode(0b00011111_11111111_11111111_11111111).ToArray());
        }
        [Fact]
        public void VariantEncode_40_Test()
        {
            Assert.Equal(new byte[] { 255, 255, 255, 255, 255, 1 }, IVariant.Encode(0b00001111_11111111_11111111_11111111_11111111).ToArray());
        }
        [Fact]
        public void VariantEncode_48_Test()
        {
            Assert.Equal(new byte[] { 255, 255, 255, 255, 255, 255, 1 }, IVariant.Encode(0b00000111_11111111_11111111_11111111_11111111_11111111).ToArray());
        }
        [Fact]
        public void VariantEncode_56_Test()
        {
            Assert.Equal(new byte[] { 255, 255, 255, 255, 255, 255, 255, 1 }, IVariant.Encode(0b00000011_11111111_11111111_11111111_11111111_11111111_11111111).ToArray());
        }
        [Fact]
        public void VariantEncode_64_Test()
        {
            Assert.Equal(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 1 }, IVariant.Encode(0b00000001_11111111_11111111_11111111_11111111_11111111_11111111_11111111).ToArray());
        }
    }
}