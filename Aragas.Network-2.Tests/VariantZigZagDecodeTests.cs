using Aragas.Network.Data;

using System;

using Xunit;

namespace Aragas.Network_2.Tests
{
    public class VariantZigZagDecodeTests : IVariant
    {
        int IVariant.Size => throw new NotImplementedException();
        Span<byte> IVariant.Encode() => throw new NotImplementedException();

        [Fact]
        public void VariantZigZagEncode_0_Test()
        {
            Assert.Equal(new byte[] { 0 }, IVariant.Encode(IVariant.ZigZagEncode(0b0000_0000)).ToArray());
        }
        [Fact]
        public void VariantZigZagEncode_8_Test()
        {
            Assert.Equal(-0b1000_0000, IVariant.ZigZagDecode(IVariant.Decode(new byte[] { 255, 1 })));
        }
        [Fact]
        public void VariantZigZagEncode_16_Test()
        {
            Assert.Equal(-0b100_0000_0000_0000, IVariant.ZigZagDecode(IVariant.Decode(new byte[] { 255, 255, 1 })));
        }
        [Fact]
        public void VariantZigZagEncode_24_Test()
        {
            Assert.Equal(-0b10_0000_0000_0000_0000_0000, IVariant.ZigZagDecode(IVariant.Decode(new byte[] { 255, 255, 255, 1 })));
        }
        [Fact]
        public void VariantZigZagEncode_32_Test()
        {
            Assert.Equal(-0b1_0000_0000_0000_0000_0000_0000_0000, IVariant.ZigZagDecode(IVariant.Decode(new byte[] { 255, 255, 255, 255, 1 })));
        }
        [Fact]
        public void VariantZigZagEncode_40_Test()
        {
            Assert.Equal(-0b1000_0000_0000_0000_0000_0000_0000_0000_0000, IVariant.ZigZagDecode(IVariant.Decode(new byte[] { 255, 255, 255, 255, 255, 1 })));
        }
        [Fact]
        public void VariantZigZagEncode_48_Test()
        {
            Assert.Equal(-0b100_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000, IVariant.ZigZagDecode(IVariant.Decode(new byte[] { 255, 255, 255, 255, 255, 255, 1 })));
        }
        [Fact]
        public void VariantZigZagEncode_56_Test()
        {
            Assert.Equal(-0b10_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000, IVariant.ZigZagDecode(IVariant.Decode(new byte[] { 255, 255, 255, 255, 255, 255, 255, 1 })));
        }
        [Fact]
        public void VariantZigZagEncode_64_Test()
        {
            Assert.Equal(-0b1_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000, IVariant.ZigZagDecode(IVariant.Decode(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 1 })));
        }
    }
}