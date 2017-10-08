
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

            Assert.DoesNotThrow( () => {
				this.int_t = this.machine.TypeSystem.GetIntType();
				this.char_t = this.machine.TypeSystem.GetCharType();
				this.double_t = this.machine.TypeSystem.GetDoubleType();
	            this.type_t = this.machine.TypeSystem.GetTypeType();
	
				this.machine.Execute( @"int x;" );
				this.machine.Execute( @"char ch;" );
				this.machine.Execute( @"double d;" );
	            this.machine.Execute( @"type_t t;" );
	
				this.machine.Execute( @"int * ptrInt;" );
				this.machine.Execute( @"char * ptrChar;" );
				this.machine.Execute( @"double * ptrDouble;" );
            });
		}
        
        [Test]
        public void TestPtrAssign()
        {
            PtrVariable int_ptr = null;
            PtrVariable char_ptr = null;
            PtrVariable double_ptr = null;
        
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"ptrInt = 0;" );
                this.machine.Execute( @"ptrChar = NULL;" );
                this.machine.Execute( @"ptrDouble = nullptr;" );
                
                int_ptr = (PtrVariable) this.machine.TDS.LookUp( @"ptrInt" );
                char_ptr = (PtrVariable) this.machine.TDS.LookUp( @"ptrChar" );
                double_ptr = (PtrVariable) this.machine.TDS.LookUp( @"ptrDouble" );
            });

            Assert.AreEqual( BigInteger.Zero, int_ptr.Value.ToBigInteger() );
            Assert.AreEqual( BigInteger.Zero, char_ptr.Value.ToBigInteger() );
            Assert.AreEqual( BigInteger.Zero, double_ptr.Value.ToBigInteger() );
        }

		[Test]
		public void TestIntVbleAssign()
		{
            Variable vble_x = null;
			Variable vble = null;

			Assert.DoesNotThrow( () => {
				vble = this.machine.TDS.LookUp( @"x" );
                Assert.AreEqual( this.int_t, vble.Type );
                vble_x = this.machine.Execute( @"x = 5;" );
            });

            Assert.AreSame( vble_x, vble, "Variable int x is not the one created" );
            Assert.AreSame( vble_x.Type, this.int_t, "Variable x does not have type: int" );
			Assert.AreEqual( (BigInteger) 5, vble.LiteralValue.Value, "Variable x does not have value: 5" );
			Assert.AreEqual( (BigInteger) 5,
                             this.machine.Memory.CreateLiteral( vble.Address, vble.Type ).Value,
                             "Variable x does not have a coherent value in memory" );
		}
        
        [Test]
        public void TestTypeTypeVbleAssign()
        {
            Variable vble_t = null;
            Variable vble = null;

            Assert.DoesNotThrow( () => {
                vble = this.machine.TDS.LookUp( @"t" );
                Assert.AreEqual( this.type_t, vble.Type );
                vble_t = this.machine.Execute( @"t = int;" );
            });

            Assert.AreSame( vble, vble_t, "Variable type_t t is not the one created" );
            Assert.AreSame( this.type_t, vble_t.Type, "Variable t does not have type: type_t" );

            Assert.AreSame( this.int_t.ToString(),
                            vble_t.Value.ToString(),
                             "Variable t does not have value: int_t" );  
        }

		[Test]
		public void TestCharVbleAssign()
		{
			Variable vble = null;
            Variable vble_ch = null;

			Assert.DoesNotThrow( () => {
				vble = this. machine.TDS.LookUp( @"ch" );
			    Assert.AreEqual( this.char_t, vble.Type );
                vble_ch = this.machine.Execute( @"ch = 'A';" );
            });

            Assert.AreSame( vble, vble_ch, "Variable char ch is not the one created" );
            Assert.AreSame( this.char_t, vble_ch.Type, "Variable ch does not have type: char_t" );
			Assert.AreEqual( 65, vble.LiteralValue.Value );
			Assert.AreEqual( 65, this.machine.Memory.CreateLiteral( vble.Address, vble.Type ).Value );
		}

		[Test]
		public void TestDoubleVbleAssign()
		{
			Variable vble = null;
            Variable vble_d = null;

			Assert.DoesNotThrow( () => {
				vble = this.machine.TDS.LookUp( @"d" );
			    Assert.AreEqual( this.double_t, vble.Type );
			    vble_d = this.machine.Execute( @"d = 6.5;" );
            });

            Assert.AreSame( vble, vble_d, "Variable double_d is not the one created" );
            Assert.AreSame( this.double_t, vble_d.Type, "Variable d does not have type: double_t" );
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
        private AType type_t;
	}
}
