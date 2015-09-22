using System;
using NUnit.Framework;

using CSim.Core;
using CSim.Core.Literals;

namespace CSimTests {
	[TestFixture]
	public class VbleTests {

		[TestFixtureSetUp]
		public void Init()
		{
			this.vm = new Machine();
			this.char_t = this.Machine.TypeSystem.GetCharType();
			this.int_t = this.Machine.TypeSystem.GetIntType();
			this.double_t = this.Machine.TypeSystem.GetDoubleType();
		}

		[Test]
		public void IntVble()
		{
			var int_v = this.Machine.TDS.Add( "x", int_t );
			int_v.LiteralValue = new IntLiteral( this.Machine, 5 );

            // Chk memory
            Assert.AreEqual( 5, this.Machine.Memory.CreateLiteral( int_v.Address, int_v.Type ).Value );
			Assert.AreEqual( 5, int_v.LiteralValue.Value );
		}

		[Test]
		public void CharVble()
		{
			var char_v = this.Machine.TDS.Add( "ch", char_t );
			char_v.LiteralValue = new CharLiteral( this.Machine, 'a' );

			Assert.AreEqual( 'a', char_v.LiteralValue.Value );
			Assert.AreEqual( 'a', this.Machine.Memory.CreateLiteral( char_v.Address, char_v.Type ).Value );
		}

		[Test]
		public void DoubleVble()
		{
			var double_v = this.Machine.TDS.Add( "d", double_t );
			double_v.LiteralValue = new DoubleLiteral( this.Machine, 0.55 );

			Assert.AreEqual( 0.55, double_v.LiteralValue.Value );
			Assert.AreEqual( 0.55, this.Machine.Memory.CreateLiteral( double_v.Address, double_v.Type ).Value );
		}

		public Machine Machine {
			get { return this.vm; }
		}

		private Machine vm;
		private CSim.Core.Types.Primitives.Char char_t;
		private CSim.Core.Types.Primitives.Int int_t;
		private CSim.Core.Types.Primitives.Double double_t;
	}
}

