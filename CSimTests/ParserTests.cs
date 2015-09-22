using System;
using NUnit.Framework;

using CSim.Core;
using CSim.Core.Types;
using CSim.Core.Types.Primitives;

namespace CSimTests {
	[TestFixture]
	public class ParserTests {

		[TestFixtureSetUp]
		public void Init()
		{
			this.machine = new Machine();

			this.int_t = this.machine.TypeSystem.GetIntType();
			this.char_t = this.machine.TypeSystem.GetCharType();
			this.double_t = this.machine.TypeSystem.GetDoubleType();

			ArchManager.Execute( this.machine, @"int x;" );
			ArchManager.Execute( this.machine, @"char ch;" );
			ArchManager.Execute( this.machine, @"double d;" );

			ArchManager.Execute( this.machine, @"int * ptrInt;" );
			ArchManager.Execute( this.machine, @"char * ptrChar;" );
			ArchManager.Execute( this.machine, @"double * ptrDouble;" );
		}

		[Test]
		public void TestIntVbleAssign()
		{
			Variable vble = null;

			Assert.DoesNotThrow( () => {
				vble = this.machine.TDS.LookUp( @"x" ); } );

			Assert.AreEqual( int_t, vble.Type );

			ArchManager.Execute( this.machine, @"x = 5;" );

			Assert.AreEqual( 5, vble.LiteralValue.Value );
			Assert.AreEqual( 5, this.machine.Memory.CreateLiteral( vble.Address, vble.Type ).Value );
		}

		[Test]
		public void TestCharVbleAssign()
		{
			Variable vble = null;

			Assert.DoesNotThrow( () => {
				vble =this. machine.TDS.LookUp( @"ch" ); } );

			Assert.AreEqual( char_t, vble.Type );

			ArchManager.Execute( this.machine, @"ch = 'A';" );

			Assert.AreEqual( 65, vble.LiteralValue.Value );
			Assert.AreEqual( 65, this.machine.Memory.CreateLiteral( vble.Address, vble.Type ).Value );
		}

		[Test]
		public void TestDoubleVbleAssign()
		{
			Variable vble = null;

			Assert.DoesNotThrow( () => {
				vble = this.machine.TDS.LookUp( @"d" ); } );

			Assert.AreEqual( double_t, vble.Type );

			ArchManager.Execute( this.machine, @"d = 6.5;" );

			Assert.AreEqual( 6.5, vble.LiteralValue.Value );
			Assert.AreEqual( 6.5, this.machine.Memory.CreateLiteral( vble.Address, vble.Type ).Value );
		}

		[Test]
		public void TestIntPtrAssign()
		{
			Variable vble = null;

			Assert.DoesNotThrow( () => {
				vble = this.machine.TDS.LookUp( @"ptrInt" ); } );
			Assert.AreEqual( this.machine.TypeSystem.GetPtrType( int_t ), vble.Type );

			ArchManager.Execute( this.machine, @"ptrInt = &x;" );
			Assert.AreEqual( vble.Address, vble.LiteralValue.Value );
			Assert.AreEqual( vble.Address, this.machine.Memory.CreateLiteral( vble.Address, vble.Type ).Value );
		}

		[Test]
		public void TestCharPtrAssign()
		{
			Variable vble = null;

			Assert.DoesNotThrow( () => {
				vble = this.machine.TDS.LookUp( @"ptrChar" ); } );

			Assert.AreEqual( this.machine.TypeSystem.GetPtrType( char_t ), vble.Type );

			ArchManager.Execute( this.machine, @"ptrChar = &ch;" );
			Assert.AreEqual( vble.Address, vble.LiteralValue.Value );
			Assert.AreEqual( vble.Address, this.machine.Memory.CreateLiteral( vble.Address, vble.Type ).Value );
		}

		[Test]
		public void TestDoublePtrAssign()
		{
			Variable vble = null;

			Assert.DoesNotThrow( () => {
				vble = this.machine.TDS.LookUp( @"ptrDouble" ); } );
			Assert.AreEqual( this.machine.TypeSystem.GetPtrType( double_t ), vble.Type );

			ArchManager.Execute( this.machine, @"ptrDouble = &d;" );
			Assert.AreEqual( vble.Address, vble.LiteralValue.Value );
			Assert.AreEqual( vble.Address, this.machine.Memory.CreateLiteral( vble.Address, vble.Type ).Value );
		}

		private Machine machine;
		private CSim.Core.Type int_t;
		private CSim.Core.Type char_t;
		private CSim.Core.Type double_t;
	}
}

