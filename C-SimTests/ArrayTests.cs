// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSimTests {
    using System.Numerics;
    using System.Linq;
    using System.Collections.Generic;
    using NUnit.Framework;
    
    using CSim.Core;
    using CSim.Core.Variables;

    [TestFixture]
    public class ArrayTests {
        [TestFixtureSetUp]
        public void Init()
        {
            this.vm = new Machine();
            
            Assert.DoesNotThrow( () => {
                this.int_t = this.vm.TypeSystem.GetIntType();
                this.char_t = this.vm.TypeSystem.GetCharType();
                this.vm.Execute( @"int * v" );
                this.vble_v = this.vm.TDS.LookUp( @"v" );
            });
        }
        
        [Test]
        public void AccessArrayElements()
        {
            const int NumElements = 5;
            Variable vble_x;
            Variable mblk;
            var vbles = new List<Variable>();

            
            Assert.DoesNotThrow( () => {
                vble_x = this.vm.Execute( @"int x" );
                this.vm.Execute( @"v = new int[" + NumElements + "]" );
                vbles.AddRange( this.vm.TDS.LookForAllVblesInAddress(
                            this.vble_v.LiteralValue.GetValueAsInteger() ) );
            });
            
            Assert.AreEqual( 1, vbles.Count );
            
            mblk = vbles[ 0 ];
            
            Assert.DoesNotThrow( () => {
                for(int i = 0; i < NumElements; ++i) {
                    this.vm.Execute( @"v[ " + i + " ] = " + ( NumElements - i ) );
                }
            });
                
            for(int i = 0; i < NumElements; ++i) {
                vble_x = this.vm.Execute( @"x = v[" + i + "]" );
                Assert.AreEqual( ( NumElements - i ).ToBigInteger(),
                                 vble_x.Value.ToBigInteger() );
            }
        }
        
        [Test]
        public void TypedArrayCreation()
        {
            const int NumElements = 5;
            Variable mblk = null;
            var vbles = new List<Variable>();
            
            Assert.DoesNotThrow( () => {
                this.vm.Execute( @"v = new int[" + NumElements + "]" );
                vbles.AddRange( this.vm.TDS.LookForAllVblesInAddress(
                            this.vble_v.LiteralValue.GetValueAsInteger() ) );
            });
            
            Assert.AreEqual( 1, vbles.Count );
            
            mblk = vbles[ 0 ];
            Assert.AreSame( this.vm.TypeSystem.GetPtrType( this.int_t ), mblk.Type );
            Assert.True( mblk is ArrayVariable );
            
            var array = mblk as ArrayVariable;
            Assert.AreSame( this.int_t, array.ElementsType );
            Assert.AreEqual( (BigInteger) NumElements, array.Count );
        }
        
        [Test]
        public void NotTypedArrayCreation()
        {
            const int NumElements = 5;
            Variable mblk = null;
            var vbles = new List<Variable>();
            
            Assert.DoesNotThrow( () => {
                this.vm.Execute( @"v = malloc(" + NumElements + "*" + this.int_t.Size +")" );
                vbles.AddRange( this.vm.TDS.LookForAllVblesInAddress(
                            this.vble_v.LiteralValue.GetValueAsInteger() ) );
            });
            
            Assert.AreEqual( 1, vbles.Count );
            
            mblk = vbles[ 0 ];
            Assert.AreSame( this.vm.TypeSystem.GetPCharType(), mblk.Type );
            Assert.True( mblk is ArrayVariable );
            
            var array = mblk as ArrayVariable;
            Assert.AreSame( this.char_t, array.ElementsType );
            Assert.AreEqual( (BigInteger) ( NumElements * this.int_t.Size ), array.Count );
        }
        
        [Test]
        public void TestPtrArray()
        {
            const int NumElements = 2;
            PtrVariable v = null;
            Variable vble_x = null;
            ArrayVariable ptr_v;
            
            Assert.DoesNotThrow( () => {
                vble_x = this.vm.Execute( "int xx = 5" );
                this.vm.Execute( @"int ** ptr_v = new int*[" + NumElements + "]" );
                v = (PtrVariable) this.vm.TDS.LookUp( "ptr_v" );
                
                this.vm.Execute( @"ptr_v[ 0 ] = &xx" );
                this.vm.Execute( @"ptr_v[ 1 ] = &xx" );
            });
            
            List<Variable> vbles = this.vm.TDS.LookForAllVblesInAddress(
                                                v.LiteralValue.ToBigInteger() ).ToList();
            ptr_v = (ArrayVariable) vbles[ 0 ];
            int addrX = (int) vble_x.Address;
            int ptrX0 = (int) ptr_v.ExtractArrayElementsValues()[ 0 ].Value.ToBigInteger();
            int ptrX1 = (int) ptr_v.ExtractArrayElementsValues()[ 1 ].Value.ToBigInteger();
            
            Assert.AreEqual( addrX, ptrX0 );
            Assert.AreEqual( addrX, ptrX1 );
            
            Assert.DoesNotThrow( () => {
                this.vm.Execute( "*ptr_v[ 0 ] = *ptr_v[ 0 ] + 1" );
            });
            
            Assert.DoesNotThrow( () => {
                this.vm.Execute( "*ptr_v[ 1 ] = *ptr_v[ 1 ] + 1" );
            });
            
            Assert.AreEqual( 7, (int) vble_x.LiteralValue.ToBigInteger() );
        }
        
        private Machine vm;
        private Variable vble_v;
        private AType int_t;
        private AType char_t;
    }
}
