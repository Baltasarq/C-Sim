
namespace CSimTests {
    using System.Numerics;
	using NUnit.Framework;

	using CSim.Core;
	using CSim.Core.Variables;

	[TestFixture]
	public class ParserTests {

		[OneTimeSetUp]
		public void Init()
		{
			this.machine = new Machine();

			this.int_t = this.machine.TypeSystem.GetIntType();
			this.char_t = this.machine.TypeSystem.GetCharType();
			this.double_t = this.machine.TypeSystem.GetDoubleType();

			this.machine.Execute( @"int x;" );
			this.machine.Execute( @"char ch;" );
			this.machine.Execute( @"double d;" );

			this.machine.Execute( @"int * ptrInt;" );
			this.machine.Execute( @"char * ptrChar;" );
			this.machine.Execute( @"double * ptrDouble;" );
		}

		[Test]
		public void TestIntVbleAssign()
		{
			Variable vble = null;

			Assert.DoesNotThrow( () => {
				vble = this.machine.TDS.LookUp( @"x" ); } );

			Assert.AreEqual( int_t, vble.Type );

			Variable vble_x = this.machine.Execute( @"x = 5;" );

            Assert.AreSame( vble_x, vble, "Variable int x is not the one created" );
            Assert.AreSame( vble_x.Type, this.int_t, "Variable x does not have type: int" );
			Assert.AreEqual( (BigInteger) 5, vble.LiteralValue.Value, "Variable x does not have value: 5" );
			Assert.AreEqual( (BigInteger) 5,
                             this.machine.Memory.CreateLiteral( vble.Address, vble.Type ).Value,
                             "Variable x does not have a coherent value in memory" );
		}

		[Test]
		public void TestCharVbleAssign()
		{
			Variable vble = null;

			Assert.DoesNotThrow( () => {
				vble =this. machine.TDS.LookUp( @"ch" ); } );

			Assert.AreEqual( char_t, vble.Type );

			this.machine.Execute( @"ch = 'A';" );

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

			this.machine.Execute( @"d = 6.5;" );

			Assert.AreEqual( 6.5, vble.LiteralValue.Value );
			Assert.AreEqual( 6.5, this.machine.Memory.CreateLiteral( vble.Address, vble.Type ).Value );
		}

		[Test]
		public void TestIntPtrAssign()
		{
			Variable intVble = null;
			PtrVariable ptrVble = null;

			// Locate variables
			Assert.DoesNotThrow( () => {
				ptrVble = (PtrVariable) this.machine.TDS.LookUp( @"ptrInt" ); } );
			Assert.AreEqual( this.machine.TypeSystem.GetPtrType( this.int_t ), ptrVble.Type );

			Assert.DoesNotThrow( () => {
				intVble = this.machine.TDS.LookUp( "x" );
			} );
			Assert.AreEqual( this.machine.TypeSystem.GetIntType(), intVble.Type );

			// Assign and test
			this.machine.Execute( @"ptrInt = &x;" );
			Assert.AreEqual( ptrVble.IntValue.Value, intVble.Address );
		}

		[Test]
		public void TestCharPtrAssign()
		{
			Variable charVble = null;
			PtrVariable ptrVble = null;

			// Locate variables
			Assert.DoesNotThrow( () => {
				ptrVble = (PtrVariable) this.machine.TDS.LookUp( @"ptrChar" ); } );
			Assert.AreEqual( this.machine.TypeSystem.GetPtrType( this.char_t ), ptrVble.Type );

			Assert.DoesNotThrow( () => {
				charVble = this.machine.TDS.LookUp( "ch" );
			} );
			Assert.AreEqual( this.machine.TypeSystem.GetCharType(), charVble.Type );

			// Assign and test
			this.machine.Execute( @"ptrChar = &ch;" );
			Assert.AreEqual( ptrVble.IntValue.Value, charVble.Address );
		}

		[Test]
		public void TestDoublePtrAssign()
		{
			Variable doubleVble = null;
			PtrVariable ptrVble = null;

			// Locate variables
			Assert.DoesNotThrow( () => {
				ptrVble = (PtrVariable) this.machine.TDS.LookUp( @"ptrDouble" ); } );
			Assert.AreEqual( this.machine.TypeSystem.GetPtrType( this.double_t ), ptrVble.Type );

			Assert.DoesNotThrow( () => {
				doubleVble = this.machine.TDS.LookUp( "d" );
			} );
			Assert.AreEqual( this.machine.TypeSystem.GetDoubleType(), doubleVble.Type );

			// Assign and test
			this.machine.Execute( @"ptrDouble = &d;" );
			Assert.AreEqual( ptrVble.IntValue.Value, doubleVble.Address );
		}

		private Machine machine;
		private AType int_t;
		private AType char_t;
		private AType double_t;
	}
}

