using Aragas.Network.Data;

using System;

using Xunit;

namespace Aragas.Network_2.Tests
{
    public class VariantDecodeTests : IVariant
    {
        int IVariant.Size => throw new NotImplementedException();
        Span<byte> IVariant.Encode() => throw new NotImplementedException();


        [Fact]
        public void VariantDecode_0_Test()
        {
            Assert.Equal((ulong) 0b_00000000, IVariant.Decode(new byte[] { 0 }));
        }
        [Fact]
        public void VariantDecode_8_Test()
        {
            Assert.Equal((ulong) 0b11111111, IVariant.Decode(new byte[] { 255, 1 }));
        }
        [Fact]
        public void VariantDecode_16_Test()
        {
            Assert.Equal((ulong) 0b01111111_11111111, IVariant.Decode(new byte[] { 255, 255, 1 }));
        }
        [Fact]
        public void VariantDecode_24_Test()
        {
            Assert.Equal((ulong) 0b00111111_11111111_11111111, IVariant.Decode(new byte[] { 255, 255, 255, 1 }));
        }
        [Fact]
        public void VariantDecode_32_Test()
        {
            Assert.Equal((ulong) 0b00011111_11111111_11111111_11111111, IVariant.Decode(new byte[] { 255, 255, 255, 255, 1 }));
        }
        [Fact]
        public void VariantDecode_40_Test()
        {
            Assert.Equal((ulong) 0b00001111_11111111_11111111_11111111_11111111, IVariant.Decode(new byte[] { 255, 255, 255, 255, 255, 1 }));
        }
        [Fact]
        public void VariantDecode_48_Test()
        {
            Assert.Equal((ulong) 0b00000111_11111111_11111111_11111111_11111111_11111111, IVariant.Decode(new byte[] { 255, 255, 255, 255, 255, 255, 1 }));
        }
        [Fact]
        public void VariantDecode_56_Test()
        {
            Assert.Equal((ulong) 0b00000011_11111111_11111111_11111111_11111111_11111111_11111111, IVariant.Decode(new byte[] { 255, 255, 255, 255, 255, 255, 255, 1 }));
        }
        [Fact]
        public void VariantDecode_64_Test()
        {
            Assert.Equal((ulong) 0b00000001_11111111_11111111_11111111_11111111_11111111_11111111_11111111, IVariant.Decode(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 1 }));
        }
    }
}