// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSimTests {
    using NUnit.Framework;

    using CSim.Core;

    [TestFixture]
    public class NumericTests {
        [OneTimeSetUp]
        public void Init()
        {
            this.machine = new Machine();

            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"int i1;" );
                this.machine.Execute( @"int i2;" );
                this.machine.Execute( @"double d1;" );
                this.machine.Execute( @"double d2;" );
                
                this.i1_v = this.machine.TDS.LookUp( "i1" );
                this.d1_v = this.machine.TDS.LookUp( "d1" );
            });
        }
        
        [SetUp]
        public void InitForEachTest()
        {
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"i1 = 5;" );
                this.machine.Execute( @"i2 = 6;" );
                this.machine.Execute( @"d1 = 7;" );
                this.machine.Execute( @"d2 = 8;" );
            });
        }
        
        [Test]
        public void TestIntSum()
        {
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"i1 = i1 + i2" );
            });
            
            Assert.AreEqual( 11.ToBigInteger(), this.i1_v.Value.ToBigInteger() );
        }
        
        [Test]
        public void TestIntSub()
        {
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"i1 = i1 - i2" );
            });
            
            Assert.AreEqual( -1.ToBigInteger(), this.i1_v.Value.ToBigInteger() );
        }
        
        [Test]
        public void TestIntMul()
        {
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"i1 = i1 * i2" );
            });
            
            Assert.AreEqual( 30.ToBigInteger(), this.i1_v.Value.ToBigInteger() );
        }
        
        [Test]
        public void TestIntDiv()
        {
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"i1 = i2 / i1" );
            });
            
            Assert.AreEqual( 1.ToBigInteger(), this.i1_v.Value.ToBigInteger() );
        }
        
        [Test]
        public void TestDblSum()
        {
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"d1 = d1 + d2" );
            });
            
            Assert.AreEqual( 15.ToBigInteger(), this.d1_v.Value.ToBigInteger() );
        }
        
        [Test]
        public void TestDblSub()
        {
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"d1 = d1 - d2" );
            });
            
            Assert.AreEqual( -1.ToBigInteger(), this.d1_v.Value.ToBigInteger() );
        }
        
        [Test]
        public void TestDblMul()
        {
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"d1 = d1 * d2" );
            });
            
            Assert.AreEqual( 56.ToBigInteger(), this.d1_v.Value.ToBigInteger() );
        }
        
        [Test]
        public void TestDblDiv()
        {
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"d1 = d2 / d1" );
            });
            
            Assert.AreEqual( 1.ToBigInteger(), this.d1_v.Value.ToBigInteger() );
        }
        
        [Test]
        public void TestMix1Sum()
        {
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"i1 = i1 + d2" );
            });
            
            Assert.AreEqual( 13.ToBigInteger(), this.i1_v.Value.ToBigInteger() );
        }
        
        [Test]
        public void TestMix1Sub()
        {
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"i1 = i1 - d2" );
            });
            
            Assert.AreEqual( -3.ToBigInteger(), this.i1_v.Value.ToBigInteger() );
        }
        
        [Test]
        public void TestMix1Mul()
        {
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"i1 = i1 * d2" );
            });
            
            Assert.AreEqual( 40.ToBigInteger(), this.i1_v.Value.ToBigInteger() );
        }
        
        [Test]
        public void TestMix1Div()
        {
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"i1 = i2 / d1" );
            });
            
            Assert.AreEqual( 0.ToBigInteger(), this.i1_v.Value.ToBigInteger() );
        }
        
        [Test]
        public void TestMix2Sum()
        {
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"i1 = d1 + i2" );
            });
            
            Assert.AreEqual( 13.ToBigInteger(), this.i1_v.Value.ToBigInteger() );
        }
        
        [Test]
        public void TestMix2Sub()
        {
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"i1 = d1 - i2" );
            });
            
            Assert.AreEqual( 1.ToBigInteger(), this.i1_v.Value.ToBigInteger() );
        }
        
        [Test]
        public void TestMix2Mul()
        {
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"i1 = d1 * i2" );
            });
            
            Assert.AreEqual( 42.ToBigInteger(), this.i1_v.Value.ToBigInteger() );
        }
        
        [Test]
        public void TestMix2Div()
        {
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"i1 = d2 / i1" );
            });
            
            Assert.AreEqual( 1.ToBigInteger(), this.i1_v.Value.ToBigInteger() );
        }
        
        [Test]
        public void TestFloatingMul()
        {
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"d1 = 0.85 * 100" );
            });
            
            Assert.AreEqual( 85.ToBigInteger(), this.d1_v.Value.ToBigInteger() );
        }

        private Machine machine;
        private Variable i1_v;
        private Variable d1_v;
    }
}
