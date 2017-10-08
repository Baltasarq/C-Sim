namespace CSimTests {
    using NUnit.Framework;
    
    using System;
    using CSim.Core;
    using CSim.Core.Exceptions;
    
    [TestFixture]
    public class FunctionTests {
        [OneTimeSetUp]
        public void Init()
        {
            Console.WriteLine( "Tests pass individually, but not globally, "
                               + "due to static contents of functions." );
                               
            this.machine = new Machine();
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"int int_v;" );
                this.machine.Execute( @"double dbl_v;" );
                this.machine.Execute( @"char chr_v;" );
                
                this.int_v = this.machine.TDS.LookUp( "int_v" );
                this.dbl_v = this.machine.TDS.LookUp( "dbl_v" );
                this.chr_v = this.machine.TDS.LookUp( "chr_v" );
	        });
        }
    
        [Test]
        public void TestAtof()
        {
            Assert.Throws<TypeMismatchException>( () => {
                this.machine.Execute( "dbl_v = atof(\"kk\");" );
            });

            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = atof(\"1\");" );
            });
            
            Assert.AreEqual( 1.0, this.dbl_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = atof(\"1.5\");" );
            });
            
            Assert.AreEqual( 1.5, this.dbl_v.LiteralValue.Value );
        }
        
        [Test]
        public void TestAtoi()
        {
            Assert.Throws<TypeMismatchException>( () => {
                this.machine.Execute( "int_v = atoi(1);" );
            });
            
            Assert.Throws<TypeMismatchException>( () => {
                this.machine.Execute( "int_v = atoi(\"1.1\");" );
            });
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "int_v = atoi(\"1\");" );
            });
            
            Assert.AreEqual( 1.ToBigInteger(), this.int_v.LiteralValue.Value );            
        }
        
        [Test]
        public void TestCharCast()
        {
            Assert.Throws<TypeMismatchException>( () => {
                this.machine.Execute( "chr_v = char(\"kk\");" );
            });
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"chr_v = char(7.8);" );
            });
            
            Assert.AreEqual( (char) 7, this.chr_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "chr_v = char(97);" );
            });
            
            Assert.AreEqual( (char) 97, this.chr_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "chr_v = char('a');" );
            });
            
            Assert.AreEqual( (char) 97, this.chr_v.LiteralValue.Value );
        }
        
        [Test]
        public void TestIntCast()
        {
            Assert.Throws<TypeMismatchException>( () => {
                this.machine.Execute( "int_v = int(\"kk\");" );
            });
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"int_v = int(7.8);" );
            });
            
            Assert.AreEqual( 7.ToBigInteger(), this.int_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "int_v = int('a');" );
            });
            
            Assert.AreEqual( 97.ToBigInteger(), this.int_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "int_v = int(-1);" );
            });
            
            Assert.AreEqual( -1.ToBigInteger(), this.int_v.LiteralValue.Value );
        }
        
        [Test]
        public void TestDoubleCast()
        {
            Assert.Throws<TypeMismatchException>( () => {
                this.machine.Execute( "dbl_v = double(\"kk\");" );
            });
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"dbl_v = double(7.8);" );
            });
            
            Assert.AreEqual( 7.8, this.dbl_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = double(97);" );
            });
            
            Assert.AreEqual( 97.0, this.dbl_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = double('a');" );
            });
            
            Assert.AreEqual( 97.0, this.dbl_v.LiteralValue.Value );
        }
        
        [Test]
        public void TestCeil()
        {
            Assert.Throws<TypeMismatchException>( () => {
                this.machine.Execute( "dbl_v = ceil(\"kk\");" );
            });
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"dbl_v = ceil(7.8);" );
            });
            
            Assert.AreEqual( 8.0, this.dbl_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = ceil(1.1);" );
            });
            
            Assert.AreEqual( 2.0, this.dbl_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = ceil('a');" );
            });
            
            Assert.AreEqual( 97.0, this.dbl_v.LiteralValue.Value );
        }
        
        [Test]
        public void TestExp()
        {
            Assert.Throws<TypeMismatchException>( () => {
                this.machine.Execute( "dbl_v = exp(\"kk\");" );
            });
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"dbl_v = exp(0);" );
            });
            
            Assert.AreEqual( 1.0, this.dbl_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = exp(1);" );
            });
            
            Assert.AreEqual( 3.0, Math.Round( this.dbl_v.LiteralValue.ToDouble() ) );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = exp('a' - 95);" );
            });
            
            Assert.AreEqual( 7.0, Math.Round( this.dbl_v.LiteralValue.ToDouble() ) );
        }
        
        [Test]
        public void TestFloor()
        {
            Assert.Throws<TypeMismatchException>( () => {
                this.machine.Execute( "dbl_v = floor(\"kk\");" );
            });
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"dbl_v = floor(9.7);" );
            });
            
            Assert.AreEqual( 9.0, this.dbl_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = floor(9);" );
            });
            
            Assert.AreEqual( 9.0, this.dbl_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = floor('a');" );
            });
            
            Assert.AreEqual( 97.0, this.dbl_v.LiteralValue.Value );
        }
        
        [Test]
        public void TestLog()
        {
            Assert.Throws<TypeMismatchException>( () => {
                this.machine.Execute( "dbl_v = log(\"kk\");" );
            });
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"dbl_v = log(1);" );
            });
            
            Assert.AreEqual( 0.0, this.dbl_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = log(2);" );
            });
            
            Assert.AreEqual( 1.0, Math.Round( this.dbl_v.LiteralValue.ToDouble() ) );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = log('a');" );
            });
            
            Assert.AreEqual( 5.0, Math.Round( this.dbl_v.LiteralValue.ToDouble() ) );
        }
        
        [Test]
        public void TestLog10()
        {
            Assert.Throws<TypeMismatchException>( () => {
                this.machine.Execute( "dbl_v = log10(\"kk\");" );
            });
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"dbl_v = log10(1);" );
            });
            
            Assert.AreEqual( 0.0, this.dbl_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = log10(10);" );
            });
            
            Assert.AreEqual( 1.0, this.dbl_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = log10('a');" );
            });
            
            Assert.AreEqual( 2.0, Math.Round( this.dbl_v.LiteralValue.ToDouble() ) );
        }
        
        [Test]
        public void TestSin()
        {
            Assert.Throws<TypeMismatchException>( () => {
                this.machine.Execute( "dbl_v = sin(\"kk\");" );
            });
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"dbl_v = sin(1);" );
            });
            
            Assert.AreEqual( 1.0, Math.Round( this.dbl_v.LiteralValue.ToDouble() ) );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = sin(10);" );
            });
            
            Assert.AreEqual( -1.0, Math.Round( this.dbl_v.LiteralValue.ToDouble() ) );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = sin('a');" );
            });
            
            Assert.AreEqual( 0.0, Math.Round( this.dbl_v.LiteralValue.ToDouble() ) );
        }
        
        [Test]
        public void TestSqrt()
        {
            Assert.Throws<TypeMismatchException>( () => {
                this.machine.Execute( "dbl_v = sqrt(\"kk\");" );
            });
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"dbl_v = sqrt(1);" );
            });
            
            Assert.AreEqual( 1.0, this.dbl_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = sqrt(16);" );
            });
            
            Assert.AreEqual( 4.0, this.dbl_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = sqrt('a');" );
            });
            
            Assert.AreEqual( 10.0, Math.Round( this.dbl_v.LiteralValue.ToDouble() ) );
        }
        
        [Test]
        public void TestTan()
        {
            Assert.Throws<TypeMismatchException>( () => {
                this.machine.Execute( "dbl_v = tan(\"kk\");" );
            });
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"dbl_v = tan(100);" );
            });
            
            Assert.AreEqual( -1.0, Math.Round( this.dbl_v.LiteralValue.ToDouble() ) );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = tan(100.0);" );
            });
            
            Assert.AreEqual( -1.0, Math.Round( this.dbl_v.LiteralValue.ToDouble() ) );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = tan('a');" );
            });
            
            Assert.AreEqual( 0.0, Math.Round( this.dbl_v.LiteralValue.ToDouble() ) );
        }
        
        [Test]
        public void TestAbs()
        {
            Assert.Throws<TypeMismatchException>( () => {
                this.machine.Execute( "int_v = abs(\"kk\");" );
            });
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"int_v = abs(-1);" );
            });
            
            Assert.AreEqual( 1.ToBigInteger(), this.int_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "int_v = abs(-1.0);" );
            });
            
            Assert.AreEqual( 1.ToBigInteger(), this.int_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "int_v = abs('a');" );
            });
            
            Assert.AreEqual( 97.ToBigInteger(), this.int_v.LiteralValue.Value );
        }
        
        [Test]
        public void TestFMod()
        {
            Assert.Throws<TypeMismatchException>( () => {
                this.machine.Execute( "dbl_v = fmod(\"kk\", 2);" );
            });
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"dbl_v = fmod(10.0, 3.0);" );
            });
            
            Assert.AreEqual( 1.0, this.dbl_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = fmod(10, 3);" );
            });
            
            Assert.AreEqual( 1.0, this.dbl_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = fmod(10.0, 3);" );
            });
            
            Assert.AreEqual( 1.0, this.dbl_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = fmod(10, 3.0);" );
            });
            
            Assert.AreEqual( 1.0, this.dbl_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = fmod('z', 'a');" );
            });
            
            Assert.AreEqual( 25.0, this.dbl_v.LiteralValue.Value );
        }
        
        [Test]
        public void TestPow()
        {
            Assert.Throws<TypeMismatchException>( () => {
                this.machine.Execute( "dbl_v = pow(\"kk\", 2);" );
            });
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"dbl_v = pow(4.0, 2.0);" );
            });
            
            Assert.AreEqual( 16.0, this.dbl_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = pow(4, 2);" );
            });
            
            Assert.AreEqual( 16.0, this.dbl_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = pow(4.0, 2);" );
            });
            
            Assert.AreEqual( 16.0, this.dbl_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = pow(4, 2.0);" );
            });
            
            Assert.AreEqual( 16.0, this.dbl_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = pow('a', 2);" );
            });
            
            Assert.AreEqual( 9409.0, this.dbl_v.LiteralValue.Value );
        }
        
        [Test]
        public void TestCos()
        {
            Assert.Throws<TypeMismatchException>( () => {
                this.machine.Execute( "dbl_v = cos(\"kk\");" );
            });
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"dbl_v = cos(97);" );
            });
            
            Assert.AreEqual( -1.0, Math.Round( this.dbl_v.LiteralValue.ToDouble() ) );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = cos(97.0);" );
            });
            
            Assert.AreEqual( -1.0, Math.Round( this.dbl_v.LiteralValue.ToDouble() ) );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "dbl_v = cos('a');" );
            });
            
            Assert.AreEqual( -1.0, Math.Round( this.dbl_v.LiteralValue.ToDouble() ) );
        }
                
        private Machine machine;
        private Variable int_v;
        private Variable dbl_v;
        private Variable chr_v;
    }
}
