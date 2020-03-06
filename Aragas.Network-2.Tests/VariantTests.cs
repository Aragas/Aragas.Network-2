using Aragas.Network.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Aragas.Network_2.Tests
{
    public class VariantTests
    {
        [Fact]
        public void VariantDecode_0_Test()
        {
            //Assert.Throws<Exception>(() =>
            //{
                var ms = new MemoryStream();
                var varLong = new VarLong(ulong.MaxValue);
                VarLong.Encode(varLong, ms);
                ms.Seek(0, SeekOrigin.Begin);

                var varShort = VarShort.Decode(ms);
                Assert.Equal(new VarShort(ushort.MaxValue), varShort);
            //});
        }

        [Fact]
        public void VariantDecode_1_Test()
        {
            //Assert.Throws<Exception>(() =>
            //{
            var ms = new MemoryStream();
            var varShort = new VarShort(ushort.MaxValue);
            VarShort.Encode(varShort, ms);
            ms.Seek(0, SeekOrigin.Begin);

            var varLong = VarLong.Decode(ms);
            Assert.Equal(new VarLong(ushort.MaxValue), varLong);
            //});
        }
    }
}