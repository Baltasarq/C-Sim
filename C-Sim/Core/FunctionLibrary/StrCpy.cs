// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Functions;
	using CSim.Core.Variables;
    using CSim.Core.Literals;
    
    using System.Numerics;
    using System.Text;

	/// <summary>
	/// This is the atoi function.
	/// Signature: int atoi(char * x);
	/// </summary>
	public sealed class StrCpy: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "strcpy";

		/// <summary>
		/// Initializes a new instance of the <see cref="EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private StrCpy(Machine m)
			: base( m, Name, m.TypeSystem.GetPCharType(), strcpyFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static StrCpy Get(Machine m)
		{       
			if ( instance == null ) {
				strcpyFormalParams = new Variable[] {
					new PtrVariable( new Id( m, @"s1" ),
                                     m.TypeSystem.GetPCharType() ),
                    new PtrVariable( new Id( m, @"s2" ),
                                     m.TypeSystem.GetPCharType() )
				};

				instance = new StrCpy( m );
			}

			return instance;
		}
        
        /// <summary>
        /// Chk the specified parameters so its assured they are valid.
        /// </summary>
        /// <param name="paramSrc">A source char * <see cref="Variable"/>.</param>
        /// <param name="paramDst">A target char * <see cref="Variable"/>.</param>
        private void Chk(Variable paramSrc, Variable paramDst)
        {
            AType pchar_t = this.Machine.TypeSystem.GetPCharType();

            // Chk destination type
            if ( pchar_t != paramDst.Type ) {
                throw new Exceptions.TypeMismatchException(
                                              pchar_t 
                                              + " != s1 - " + paramDst.Type );
            }
            
            // Destination address must be valid
            if ( paramDst.IsTemp() ) {
                throw new Exceptions.IncorrectAddressException(
                    L10n.Get( L10n.Id.ExcInvalidMemory ) + paramDst.Value
                );
            }
            
            // Chk source type
            if ( pchar_t != paramSrc.Type ) {
                throw new Exceptions.TypeMismatchException(
                                              pchar_t 
                                              + " != s2 - " + paramSrc.Type );
            }
        }

		/// <summary>
		/// Execute this <see cref="Function"/> with
		/// the specified parameters (<see cref="RValue"/>'s).
		/// </summary>
		/// <param name="realParams">The parameters.</param>
		public override void Execute(RValue[] realParams)
		{
            var paramDst = realParams[ 0 ].SolveToVariable();
            var paramSrc = realParams[ 1 ].SolveToVariable();
            
            this.Chk( paramSrc, paramDst );
            
            // Do the copy from literal
            byte value;
            BigInteger dstIndex = paramDst.Value.ToBigInteger();
            
            if ( paramSrc.IsTemp() ) {
                if ( paramSrc.LiteralValue is StrLiteral strLit ) {
                    string s2 = strLit.Value;
                    BigInteger length = BigInteger.Min(
                                    s2.Length,
                                    ( this.Machine.Memory.Max - dstIndex ) );
                                    
                    this.Machine.Memory.CheckSizeFits( dstIndex, (int) length );
                    this.Machine.Memory.Write(  dstIndex,
                                                Encoding.ASCII.GetBytes( s2 ) );

                    // End mark ('\0')
                    this.Machine.Memory.Write( dstIndex + length, new byte[]{ 0 } );
                } else {
                    throw new Exceptions.TypeMismatchException( "s1 lit??" );
                }
            }
            else {
                // Copy from another memory address
	            BigInteger srcIndex = paramSrc.Value.ToBigInteger();
	            
	            do {
	                value = this.Machine.Memory.Read( srcIndex, 1 )[ 0 ];
	                this.Machine.Memory.Write( dstIndex, new byte[]{ value } );
	                
	                ++srcIndex;
	                ++dstIndex;
                    
                    if ( srcIndex >= this.Machine.Memory.Max
                      || dstIndex >= this.Machine.Memory.Max )
                    {
                        throw new Exceptions.IncorrectAddressException(
                                        L10n.Get( L10n.Id.ExcInvalidMemory )
                                        + this.Machine.Memory.Max );
                    }
	            } while( value != 0 );
            }
            
			var result = Variable.CreateTempVariable( paramDst.LiteralValue );
			this.Machine.ExecutionStack.Push( result );
		}

		private static StrCpy instance;
		private static Variable[] strcpyFormalParams;
	}
}
