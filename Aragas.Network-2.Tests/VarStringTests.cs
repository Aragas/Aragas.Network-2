using Aragas.Network.Data;

using System;
using System.IO;

using Xunit;

namespace Aragas.Network_2.Tests
{
    public class VarStringTests :IVariant
    {
        int IVariant.Size => throw new NotImplementedException();
        Span<byte> IVariant.Encode() => throw new NotImplementedException();

        [Fact]
        public void VariantDecode_Stream_Test()
        {
            var ms = new MemoryStream();

            var value = new VarString("TestShitPleaseIgnore");

            VarString.Encode(value, ms);
            ms.Seek(0, SeekOrigin.Begin);

            var newValue = VarString.Decode(ms);

            Assert.Equal(value, newValue);
        }

        [Fact]
        public void VariantDecode_Span_New_Test()
        {
            Span<byte> array = new byte[128];

            var value = new VarString("TestShitPleaseIgnore");

            VarString.Encode(value, array);

            var newValue = VarString.Decode(array);

            Assert.Equal(value, newValue);
        }

        [Fact]
        public void VariantDecode_Span_Stackalloc_Test()
        {
            Span<byte> array = stackalloc byte[128];

            var value = new VarString("TestShitPleaseIgnore");

            VarString.Encode(value, array);

            var newValue = VarString.Decode(array);

            Assert.Equal(value, newValue);
        }
    }
}