// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Functions;
	using CSim.Core.Variables;
    using CSim.Core.Literals;
    
    using System.Numerics;

	/// <summary>
	/// This is the atoi function.
	/// Signature: int atoi(char * x);
	/// </summary>
	public sealed class StrCmp: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "strcmp";

		/// <summary>
		/// Initializes a new instance of the <see cref="EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private StrCmp(Machine m)
			: base( m, Name, m.TypeSystem.GetIntType(), strcmpFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static StrCmp Get(Machine m)
		{       
			if ( instance == null ) {
				strcmpFormalParams = new Variable[] {
					new PtrVariable( new Id( m, @"s1" ),
                                     m.TypeSystem.GetPCharType() ),
                    new PtrVariable( new Id( m, @"s2" ),
                                     m.TypeSystem.GetPCharType() )
				};

				instance = new StrCmp( m );
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
            int result = 0;
            byte[] str1;
            byte[] str2;
            var paramDst = realParams[ 0 ].SolveToVariable();
            var paramSrc = realParams[ 1 ].SolveToVariable();
            
            this.Chk( paramSrc, paramDst );

            // Get source            
            if ( paramSrc.IsTemp() ) {
                if ( paramSrc.LiteralValue is StrLiteral strLit ) {
                    str1 = strLit.GetRawValue();
                } else {
                    throw new Exceptions.TypeMismatchException( "s1 lit??" );
                }
            } else {
                if ( paramSrc.IsIndirection() ) {
                    var ptr1 = (IndirectVariable) paramSrc;
                    str1 = this.Machine.Memory.ReadStringFromMemory( ptr1.PointedAddress );
                } else {
                    throw new Exceptions.TypeMismatchException( "s1 char*??" );
                }
            }
            
            // Get destination            
            if ( paramDst.IsTemp() ) {
                if ( paramDst.LiteralValue is StrLiteral strLit ) {
                    str2 = strLit.GetRawValue();
                } else {
                    throw new Exceptions.TypeMismatchException( "s2 lit??" );
                }
            } else {
                if ( paramDst.IsIndirection() ) {
                    var ptr2 = (IndirectVariable) paramDst;
                    str2 = this.Machine.Memory.ReadStringFromMemory( ptr2.PointedAddress );
                } else {
                    throw new Exceptions.TypeMismatchException( "s2 char*??" );
                }
            }
            
            // Compare
            int str1Length = str1.Length;
            int str2Length = str2.Length;
            
            if ( str1Length != str2Length ) {
                result = str1Length - str2Length;
            } else {
                for(int i = 0; i < str1Length; ++i) {
                    byte chr1 = str1[ i ];
                    byte chr2 = str2[ i ];
                    
                    if ( chr1 != chr2 ) {
                        result = chr1 - chr2;
                        break;
                    }
                }
            }
            
			var resultVble = Variable.CreateTempVariable( this.Machine,
                                                          result.ToBigInteger() );
			this.Machine.ExecutionStack.Push( resultVble );
		}

		private static StrCmp instance;
		private static Variable[] strcmpFormalParams;
	}
}
