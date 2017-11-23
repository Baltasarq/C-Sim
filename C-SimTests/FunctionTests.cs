// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSimTests {
    using System;
    using System.Numerics;
    using System.Text;
    using System.Globalization;
    using NUnit.Framework;
    
    using CSim.Core;
    using CSim.Core.Exceptions;
    
    [TestFixture]
    public class FunctionTests {
        [TestFixtureSetUp]
        public void Init()
        {
            Console.WriteLine( "Tests pass individually, but not globally, "
                               + "due to static contents of functions." );
                               
            this.machine = new Machine( outputter: (s) => {},
                                        inputter: (s) => "Baltasar" );
            
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
        public void TestFree()
        {
            Assert.Throws<IncorrectAddressException>( () => {
                this.machine.Execute( @"free(1)" );
            });
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( @"int * v = malloc(2)" );
                this.machine.Execute( @"free(v)" );
            });
        }
        
        [Test]
        public void TestFGets()
        {
            const int Max = 20;
            string cmd = "fgets(str, " + Max + ", stdin);";
            string expectedResult = "Baltasar";
            Variable str = null;
            Variable vble= null;
            
            Assert.DoesNotThrow( () => {
                str = this.machine.Execute( "char *str = malloc(" + Max + ")" );
                this.machine.Memory.CheckAddressFits( str.LiteralValue.ToBigInteger() );
            });
            
            Assert.Throws<TypeMismatchException>( () => {
                this.machine.Execute( "fgets(1, 2, 3);" );
            });
            
            Assert.Throws<TypeMismatchException>( () => {
                this.machine.Execute( "fgets(str, \"a\", \"b\" );" );
            });
            
            Assert.Throws<TypeMismatchException>( () => {
                this.machine.Execute( "fgets(str, 2, \"b\" );" );
            });
            
            Assert.DoesNotThrow( () => {
                vble = this.machine.Execute( cmd );
            });
            
            int address = (int) vble.LiteralValue.ToBigInteger();
            byte[] result = this.machine.Memory.ReadStringFromMemory( address );
            string dest = Encoding.ASCII.GetString( result );
            Assert.AreEqual( (int) str.LiteralValue.ToBigInteger(), address );
            Assert.AreEqual( expectedResult, dest );
        }
        
        [Test]
        public void TestSPrintf()
        {
            const int Max = 20;
            string msgFormat = "%s %d %f %c";
            string msg = "hola";
            int val1 = 42;
            string val2 = 42.51.ToString( CultureInfo.InvariantCulture );
            char val3 = (char) 42;
            Variable str = null;
            Variable vble = null;
            string cmd = "sprintf(s, \""
                         + msgFormat + "\", \"" + msg + "\", "
                         + val1 + ", " + val2 + ", '" + val3 + "');";
            string expectedResult = "hola 42 42.51 *";

            Assert.DoesNotThrow( () => {
                str = this.machine.Execute( "char *s = malloc(" + Max + ")" );
                vble = this.machine.Execute( cmd );
                this.machine.Memory.CheckAddressFits( str.LiteralValue.ToBigInteger() );
            });

            byte[] result = this.machine.Memory.ReadStringFromMemory(
                                                str.Value.ToBigInteger() );
                                                
            string dest = Encoding.ASCII.GetString( result );
            Assert.AreEqual( expectedResult.Length, (int) vble.LiteralValue.ToBigInteger() );
            Assert.AreEqual( expectedResult, dest );
        }
        
        [Test]
        public void TestStrCmp()
        {
            string msg1 = "hola";
            string msg2 = "adios";
            
            Assert.Throws<TypeMismatchException>( () => {
                this.machine.Execute( "strcmp('a', 2);" );
            });
            
            Assert.Throws<TypeMismatchException>( () => {
                this.machine.Execute( "strcmp(1, \"2\");" );
            });
            
            Assert.DoesNotThrow( () => {
                var vble1 = this.machine.Execute( "strcmp(\"" + msg1 + "\", \"" + msg1 + "\");" );
                var vble2 = this.machine.Execute( "strcmp(\"" + msg1 + "\", \"" + msg2 + "\");" );
                
                Assert.AreEqual( 0.ToBigInteger(), vble1.LiteralValue.ToBigInteger() );
                Assert.AreNotEqual( 0.ToBigInteger(), vble2.LiteralValue.ToBigInteger() );
            });
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "char *s_strcmp=malloc(10)" );
                this.machine.Execute( "strcpy(s_strcmp, \"" + msg1 + "\");" );

                var vble1 = this.machine.Execute( "strcmp(s_strcmp, " + "\"" + msg1 + "\");" );
                var vble2 = this.machine.Execute( "strcmp(\"" +  msg2 + "\", s_strcmp);" );
                var vble3 = this.machine.Execute( "strcmp(s_strcmp, \"" +  msg1 + "\");" );
                
                Assert.AreEqual( 0.ToBigInteger(), vble1.LiteralValue.ToBigInteger() );
                Assert.AreNotEqual( 0.ToBigInteger(), vble2.LiteralValue.ToBigInteger() );
                Assert.AreEqual( 0.ToBigInteger(), vble3.LiteralValue.ToBigInteger() );
            });
        }
        
        [Test]
        public void TestStrCat()
        {
            string msg1 = "hola, ";
            string msg2 = "mundo!";
            string msgComplete = msg1 + msg2; 
            
            Assert.Throws<TypeMismatchException>( () => {
                this.machine.Execute( "strcat('a', 2);" );
            });
            
            Assert.Throws<IncorrectAddressException>( () => {
                this.machine.Execute( "strcat(\"a\", 2);" );
            });
            
            Assert.Throws<TypeMismatchException>( () => {
                this.machine.Execute( "strcat(1, \"2\");" );
            });
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "char *s_strcat=malloc(10)" );
                this.machine.Execute( "strcpy(s_strcat, \"" + msg1 + "\");" );
                this.machine.Execute( "strcat(s_strcat, \"" + msg2 + "\");" );
            });

            Variable s = this.machine.TDS.LookUp( "s_strcat" );
            BigInteger addr = s.Value.ToBigInteger();
            BigInteger start = addr;
            byte value;
            int msgPos = 0;
            
            while( msgPos < msgComplete.Length ) {
                value = this.machine.Memory.Read( addr, 1 )[ 0 ];
                Assert.AreEqual( msgComplete[ msgPos ], (char) value );
                
                ++addr;
                msgPos = (int) ( addr - start );
            }
            
            value = this.machine.Memory.Read( addr, 1 )[ 0 ];
            Assert.AreEqual( 0, value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "char *s_strcat2 = malloc(10)" );
                this.machine.Execute( "strcpy( s_strcat2, \"" + msg2 + "\");" );
                this.machine.Execute( "strcpy( s_strcat, \"" + msg1 + "\");" );
                this.machine.Execute( "strcat( s_strcat, s_strcat2 );" );
            });
            
            msgPos = 0;
            addr = s.Value.ToBigInteger();
            while( msgPos < msgComplete.Length ) {
                value = this.machine.Memory.Read( addr, 1 )[ 0 ];
                Assert.AreEqual( msgComplete[ msgPos ], (char) value );
                
                ++addr;
                msgPos = (int) ( addr - start );
            }
            
            value = this.machine.Memory.Read( addr, 1 )[ 0 ];
            Assert.AreEqual( 0, value );
        }
        
        [Test]
        public void TestStrCpy()
        {
            string msg = "hola";
            
            Assert.Throws<TypeMismatchException>( () => {
                this.machine.Execute( "strcpy('a', 2);" );
            });
            
            Assert.Throws<IncorrectAddressException>( () => {
                this.machine.Execute( "strcpy(\"a\", 2);" );
            });
            
            Assert.Throws<TypeMismatchException>( () => {
                this.machine.Execute( "strcpy(1, \"2\");" );
            });
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "char *s_strcpy=malloc(10)" );
                this.machine.Execute( "strcpy(s_strcpy, \"" + msg + "\");" );
            });
            
            Variable s = this.machine.TDS.LookUp( "s_strcpy" );
            BigInteger addr = s.Value.ToBigInteger();
            BigInteger start = addr;
            byte value;
            int msgPos = 0;
            
            while( msgPos < msg.Length ) {
                value = this.machine.Memory.Read( addr, 1 )[ 0 ];
                Assert.AreEqual( msg[ msgPos ], value );
                
                ++addr;
                msgPos = (int) ( addr - start );
            }
            
            value = this.machine.Memory.Read( addr, 1 )[ 0 ];
            Assert.AreEqual( 0, value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "char *s_strcpy2 = malloc(10)" );
                this.machine.Execute( "strcpy( s_strcpy2, \"" + msg + "\");" );
                this.machine.Execute( "strcpy( s_strcpy, s_strcpy2 );" );
            });
            
            Variable s1 = this.machine.TDS.LookUp( "s_strcpy" );
            Variable s2 = this.machine.TDS.LookUp( "s_strcpy2" );
            BigInteger addr1 = s1.Value.ToBigInteger();
            BigInteger addr2 = s2.Value.ToBigInteger();
            byte value1 = this.machine.Memory.Read( addr1, 1 )[ 0 ];
            byte value2 = this.machine.Memory.Read( addr2, 1 )[ 0 ];
            
            while( value != 0 ) {
                Assert.AreEqual( value2, value1 );
                
                ++addr1;
                ++addr2;
                value1 = this.machine.Memory.Read( addr1, 1 )[ 0 ];
                value2 = this.machine.Memory.Read( addr2, 1 )[ 0 ];
            }
        }
        
        [Test]
        public void TestStrLen()
        {
            Assert.Throws<TypeMismatchException>( () => {
                this.machine.Execute( "int_v = strlen('a');" );
            });
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "int_v = strlen(\"\");" );
            });
            
            Assert.AreEqual( 0.ToBigInteger(), this.int_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "int_v = strlen(\"a\");" );
            });
            
            Assert.AreEqual( 1.ToBigInteger(), this.int_v.LiteralValue.Value );

            Assert.DoesNotThrow( () => {
                this.machine.Execute( "int_v = strlen(\"aa\");" );
            });
            
            Assert.AreEqual( 2.ToBigInteger(), this.int_v.LiteralValue.Value );
            
            Assert.DoesNotThrow( () => {
                this.machine.Execute( "char *s_strlen = malloc(6)" );
                this.machine.Execute( "s_strlen[0] = 'h'" );
                this.machine.Execute( "s_strlen[1] = 'o'" );
                this.machine.Execute( "s_strlen[2] = 'l'" );
                this.machine.Execute( "s_strlen[3] = 'a'" );
                this.machine.Execute( "s_strlen[4] = 0" );
                this.machine.Execute( "int_v = strlen(s_strlen);" );
            });
            
            Assert.AreEqual( 4.ToBigInteger(), this.int_v.LiteralValue.Value );
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
