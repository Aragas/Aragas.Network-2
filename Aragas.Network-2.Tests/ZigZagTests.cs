using Aragas.Network.Data;

using System;

using Xunit;

namespace Aragas.Network_2.Tests
{
    public class ZigZagTests : IVariant
    {
        int IVariant.Size => throw new NotImplementedException();
        Span<byte> IVariant.Encode() => throw new NotImplementedException();

        [Fact]
        public void ZigZagEncode_0_Test()
        {
            var zigZag = IVariant.ZigZagEncode(0);

            Assert.Equal(0UL, zigZag);
        }
        [Fact]
        public void ZigZagEncode_1_Test()
        {
            var zigZag = IVariant.ZigZagEncode(1);

            Assert.Equal(2UL, zigZag);
        }
        [Fact]
        public void ZigZagEncode_2_Test()
        {
            var zigZag = IVariant.ZigZagEncode(-1);

            Assert.Equal(1UL, zigZag);
        }

        [Fact]
        public void ZigZagDecode_0_Test()
        {
            var zigZag = IVariant.ZigZagDecode(0);

            Assert.Equal(0, zigZag);
        }
        [Fact]
        public void ZigZagDecode_1_Test()
        {
            var zigZag = IVariant.ZigZagDecode(1);

            Assert.Equal(-1, zigZag);
        }
        [Fact]
        public void ZigZagDecode_2_Test()
        {
            var zigZag = IVariant.ZigZagDecode(2);

            Assert.Equal(1, zigZag);
        }
    }
}