
namespace CSimTests {
    using System;
	using System.Numerics;
	using NUnit.Framework;

	using CSim.Core.Native;

	[TestFixture]
	public class UInt128Tests {
		[Test]
		public void TestByte()
		{
			for(byte x = byte.MinValue; x < byte.MaxValue; ++x)
			{
				UInt128 nx = x;
				Assert.AreEqual( (BigInteger) x, nx.Value, "UInt128 {0} != {1}", x, nx );
			}
		}
        
        [Test]
        public void TestUInt16()
        {
            for(UInt16 x = UInt16.MinValue; x < UInt16.MaxValue; ++x)
            {
                UInt128 nx = (UInt128) x;
                Assert.AreEqual( (BigInteger) x, nx.Value, "UInt128 {0} != {1}", x, nx );
            }
        }
	}
}
