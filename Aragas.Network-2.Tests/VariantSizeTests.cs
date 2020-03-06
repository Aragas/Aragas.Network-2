using Aragas.Network.Data;

using System;

using Xunit;

namespace Aragas.Network_2.Tests
{
    public class VariantSizeTests : IVariant
    {
        int IVariant.Size => throw new NotImplementedException();
        Span<byte> IVariant.Encode() => throw new NotImplementedException();


        [Fact]
        public void VariantSize_0_Test()
        {
            Assert.Equal(1, IVariant.VariantSize(0b_00000000));
        }
        [Fact]
        public void VariantSize_8_Test()
        {
            Assert.Equal(2, IVariant.VariantSize(0b11111111));
        }
        [Fact]
        public void VariantSize_16_Test()
        {
            Assert.Equal(3, IVariant.VariantSize(0b11111111_11111111));
        }
        [Fact]
        public void VariantSize_24_Test()
        {
            Assert.Equal(4, IVariant.VariantSize(0b11111111_11111111_11111111));
        }
        [Fact]
        public void VariantSize_32_Test()
        {
            Assert.Equal(5, IVariant.VariantSize(0b11111111_11111111_11111111_11111111));
        }
        [Fact]
        public void VariantSize_40_Test()
        {
            Assert.Equal(6, IVariant.VariantSize(0b11111111_11111111_11111111_11111111_11111111));
        }
        [Fact]
        public void VariantSize_48_Test()
        {
            Assert.Equal(7, IVariant.VariantSize(0b11111111_11111111_11111111_11111111_11111111_11111111));
        }
        [Fact]
        public void VariantSize_56_Test()
        {
            Assert.Equal(8, IVariant.VariantSize(0b11111111_11111111_11111111_11111111_11111111_11111111_11111111));
        }
        [Fact]
        public void VariantSize_64_Test()
        {
            Assert.Equal(9, IVariant.VariantSize(0b11111111_11111111_11111111_11111111_11111111_11111111_11111111_1111111));
        }

        [Fact]
        public void VariantSize_Zero_Test()
        {
            Assert.Equal(1, IVariant.VariantSize(0));
        }
        [Fact]
        public void VariantSize_Positive_Test()
        {
            Assert.Equal(1, IVariant.VariantSize(1));
        }
        [Fact]
        public void VariantSize_Negative_Test()
        {
            Assert.Equal(5, IVariant.VariantSize(-1));
        }
        [Fact]
        public void VariantSize_Negative_Long_Test()
        {
            Assert.Equal(10, IVariant.VariantSize(-1 + (Int64)Int32.MinValue));
        }

        [Fact]
        public void VariantSize_Int8_Min_Test()
        {
            Assert.Equal(5, IVariant.VariantSize(SByte.MinValue));
        }
        [Fact]
        public void VariantSize_Int16_Min_Test()
        {
            Assert.Equal(5, IVariant.VariantSize(Int16.MinValue));
        }
        [Fact]
        public void VariantSize_Int32_Min_Test()
        {
            Assert.Equal(5, IVariant.VariantSize(Int32.MinValue));
        }
        [Fact]
        public void VariantSize_Int64_Min_Test()
        {
            Assert.Equal(10, IVariant.VariantSize(Int64.MinValue));
        }

        [Fact]
        public void VariantSize_Int8_Max_Test()
        {
            Assert.Equal(1, IVariant.VariantSize(SByte.MaxValue));
        }
        [Fact]
        public void VariantSize_Int16_Max_Test()
        {
            Assert.Equal(3, IVariant.VariantSize(Int16.MaxValue));
        }
        [Fact]
        public void VariantSize_Int32_Max_Test()
        {
            Assert.Equal(5, IVariant.VariantSize(Int32.MaxValue));
        }
        [Fact]
        public void VariantSize_Int64_Max_Test()
        {
            Assert.Equal(9, IVariant.VariantSize(Int64.MaxValue));
        }
    }
}