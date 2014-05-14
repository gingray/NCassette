using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCassetteLib.Common;
using NUnit.Framework;

namespace NCassetteTests
{
    [TestFixture]
    public class Test
    {
        [Test]
        public void TestNullRef()
        {
            var t = NCassetteLib.NCassette.Record(() => (string)null)
                .SerializeWayBinary()
                .WorkInReleaseMode()
                .StorageInTempFiles()
                .Execute();
            Assert.AreEqual(t, null);
        }

        [Test]
        public void TestNullRef2()
        {
            var t = NCassetteLib.NCassette.Record(() => (CustomUnserializable)null)
                .SerializeWayBinary()
                .StorageInTempFiles()
                .WorkInReleaseMode()
                .Execute();
            Assert.AreEqual(t, null);
        }
    }
}
