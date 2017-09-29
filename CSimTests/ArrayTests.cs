namespace CSimTests {
    using System.Numerics;
    using System.Collections.Generic;
    using NUnit.Framework;
    
    using CSim.Core;
    using CSim.Core.Variables;

    [TestFixture]
    public class ArrayTests {
        [OneTimeSetUp]
        public void Init()
        {
            this.vm = new Machine();
            
            Assert.DoesNotThrow( () => {
                this.int_t = this.vm.TypeSystem.GetIntType();
                this.char_t = this.vm.TypeSystem.GetCharType();
                this.vm.Execute( @"int * x" );
                this.vble_x = this.vm.TDS.LookUp( @"x" );
            });
        }
        
        [Test]
        public void TypedArrayCreation()
        {
            const int NumElements = 5;
            Variable mblk = null;
            var vbles = new List<Variable>();
            
            Assert.DoesNotThrow( () => {
                this.vm.Execute( @"x = new int[" + NumElements + "]" );
                vbles.AddRange( this.vm.TDS.LookForAllVblesInAddress(
                            this.vble_x.LiteralValue.GetValueAsInteger() ) );
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
                this.vm.Execute( @"x = malloc(" + NumElements + "*" + this.int_t.Size +")" );
                vbles.AddRange( this.vm.TDS.LookForAllVblesInAddress(
                            this.vble_x.LiteralValue.GetValueAsInteger() ) );
            });
            
            Assert.AreEqual( 1, vbles.Count );
            
            mblk = vbles[ 0 ];
            Assert.AreSame( this.vm.TypeSystem.GetPtrType( this.char_t ), mblk.Type );
            Assert.True( mblk is ArrayVariable );
            
            var array = mblk as ArrayVariable;
            Assert.AreSame( this.char_t, array.ElementsType );
            Assert.AreEqual( (BigInteger) ( NumElements * this.int_t.Size ), array.Count );
        }
        
        private Machine vm;
        private Variable vble_x;
        private AType int_t;
        private AType char_t;
    }
}
