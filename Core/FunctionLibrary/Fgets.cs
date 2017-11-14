// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.FunctionLibrary {
    using Exceptions;
    using Types;
    using Literals;
    
    using System;
    using System.Numerics;
    using System.Text;

    /// <summary>
    /// This is the fgets() std function.
    /// Signature: char * fgets(char *str, int num, int stream)
    /// </summary>
    public class Fgets: Functions.EmbeddedFunction {
        /// <summary>
        /// The identifier for the function.
        /// </summary>
        public const string Name = "fgets";

        /// <summary>
        /// Initializes a new <see cref="T:Fgets"/>.
        /// This is not intended to be used directly.
        /// </summary>
        private Fgets(Machine m)
            : base( m, Name, m.TypeSystem.GetIntType(), fgetsFormalParams )
        {
        }

        /// <summary>
        /// Returns the only instance of this function.
        /// </summary>
        public static Fgets Get(Machine m)
        {
            if ( instance == null ) {
                fgetsFormalParams = new Variable[] {
                    new Variable( m, @"str", m.TypeSystem.GetPCharType() ),
                    new Variable( m, @"num", m.TypeSystem.GetIntType() ),
                    new Variable( m, @"stream", m.TypeSystem.GetIntType() )
                };

                instance = new Fgets( m );
            }

            return instance;
        }
        
        private void Chk(Variable str, Variable num, Variable stream)
        {
            AType pchar_t = this.Machine.TypeSystem.GetPCharType();
            AType int_t = this.Machine.TypeSystem.GetIntType();

            // Chk
            if ( str.Type != this.Machine.TypeSystem.GetPCharType() ) {
                throw new TypeMismatchException( pchar_t + " != " + str.Type );
            }

            if ( !( num.Type is Primitive ) ) {
                throw new TypeMismatchException( int_t + " != " + num.Type );
            }
            
            if ( !( stream.Type is Primitive ) ) {
                throw new TypeMismatchException( int_t + " != " + stream.Type );
            }
            
            return;
        }

        /// <summary>
        /// Execute this <see cref="Function"/> with the specified parameters.
        /// </summary>
        /// <param name="realParams">
        /// The parameters, as <see cref="RValue"/>'s.
        /// </param>
        public override void Execute(RValue[] realParams)
        {
            Variable str = realParams[ 0 ].SolveToVariable();
            Variable num = realParams[ 1 ].SolveToVariable();
            Variable stream = realParams[ 2 ].SolveToVariable();
            BigInteger address = str.LiteralValue.ToBigInteger();

            Chk( str, num, stream );

            // Do it
            int max = ( (int) num.LiteralValue.ToBigInteger() ) - 1;
            
            if ( stream.Value.ToBigInteger() == 0 ) {
                string input = this.Machine.Inputter( "" );
                input = input.Substring( 0, Math.Min( input.Length, max ) );
                input += '\0';
                this.Machine.Memory.Write( address,
                                            Encoding.ASCII.GetBytes( input ) );
            } else {
                address = 0;
            }

            // Result
            var litResult = new IntLiteral( this.Machine, address );
            this.Machine.ExecutionStack.Push(
                                    Variable.CreateTempVariable( litResult ) );
        }

        private static Fgets instance = null;
        private static Variable[] fgetsFormalParams;
    }
}
