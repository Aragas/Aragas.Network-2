using Aragas.Network.Data;

using System;

using Xunit;

namespace Aragas.Network_2.Tests
{
    public class VariantZigZagSizeTests : IVariant
    {
        int IVariant.Size => throw new NotImplementedException();
        Span<byte> IVariant.Encode() => throw new NotImplementedException();

        [Fact]
        public void VariantSize_ZigZag_Zero_Test()
        {
            Assert.Equal(1, IVariant.VariantSize(IVariant.ZigZagEncode(0)));
        }
        [Fact]
        public void VariantSize_ZigZag_Positive_Test()
        {
            Assert.Equal(1, IVariant.VariantSize(IVariant.ZigZagEncode(1)));
        }
        [Fact]
        public void VariantSize_ZigZag_Negative_Test()
        {
            Assert.Equal(1, IVariant.VariantSize(IVariant.ZigZagEncode(-1)));
        }
        [Fact]
        public void VariantSize_ZigZag_Negative_Long_Test()
        {
            Assert.Equal(5, IVariant.VariantSize(IVariant.ZigZagEncode(-1 + (Int64) Int32.MinValue)));
        }

        [Fact]
        public void VariantSize_ZigZag_Int8_Min_Test()
        {
            Assert.Equal(2, new VarZShort(SByte.MinValue).Size);
        }
        [Fact]
        public void VariantSize_ZigZag_Int16_Min_Test()
        {
            Assert.Equal(3, IVariant.VariantSize(IVariant.ZigZagEncode(Int16.MinValue)));
        }
        [Fact]
        public void VariantSize_ZigZag_Int32_Min_Test()
        {
            Assert.Equal(5, IVariant.VariantSize(IVariant.ZigZagEncode(Int32.MinValue)));
        }
        [Fact]
        public void VariantSize_ZigZag_Int64_Min_Test()
        {
            Assert.Equal(10, IVariant.VariantSize(IVariant.ZigZagEncode(Int64.MinValue)));
        }

        [Fact]
        public void VariantSize_ZigZag_Int8_Max_Test()
        {
            Assert.Equal(2, IVariant.VariantSize(IVariant.ZigZagEncode(SByte.MaxValue)));
        }
        [Fact]
        public void VariantSize_ZigZag_Int16_Max_Test()
        {
            Assert.Equal(3, IVariant.VariantSize(IVariant.ZigZagEncode(Int16.MaxValue)));
        }
        [Fact]
        public void VariantSize_ZigZag_Int32_Max_Test()
        {
            Assert.Equal(5, IVariant.VariantSize(IVariant.ZigZagEncode(Int32.MaxValue)));
        }
        [Fact]
        public void VariantSize_ZigZag_Int64_Max_Test()
        {
            Assert.Equal(10, IVariant.VariantSize(IVariant.ZigZagEncode(Int64.MaxValue)));
        }
    }
}