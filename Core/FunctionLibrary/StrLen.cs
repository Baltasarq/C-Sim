// Prys CSim - Copyright (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Functions;
	using CSim.Core.Variables;
	using CSim.Core.Literals;
    
    using System.Numerics;

	/// <summary>
	/// This is the atof function.
	/// Signature: double atof(char * x);
	/// </summary>
	public sealed class StrLen: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "strlen";

		/// <summary>
		/// Initializes a new instance of the <see cref="StrLen"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private StrLen(Machine m)
			: base( m, Name, m.TypeSystem.GetIntType(), strLenFormalParameters )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static StrLen Get(Machine m)
		{
			if ( instance == null ) {
				strLenFormalParameters = new Variable[] {
					new PtrVariable( new Id( m, @"s" ), m.TypeSystem.GetPCharType() )
				};

				instance = new StrLen( m );
			}

			return instance;
		}
        
        /// <summary>
        /// Chk the specified param for being a char */> 
        /// </summary>
        /// <param name="param">The parameter <see cref="Variable"/>.</param>
        private void Chk(Variable param)
        {
            AType pchar_t = this.Machine.TypeSystem.GetPCharType();
            
            if ( pchar_t != param.Type ) {
                throw new Exceptions.TypeMismatchException(
                                                pchar_t
                                                + " != " + param.Type );
            }
            
            return;
        }

		/// <summary>
		/// Execute this <see cref="Function"/> with
		/// the specified parameters (<see cref="RValue"/>'s).
		/// </summary>
		/// <param name="realParams">The parameters.</param>
		public override void Execute(RValue[] realParams)
		{
            BigInteger startAddress;
            BigInteger address;
			var param = realParams[ 0 ].SolveToVariable();
            
            this.Chk( param );            

            if ( param.IsTemp() ) {
                if ( param.LiteralValue is StrLiteral strLit ) {
                    startAddress = 0;
                    address = strLit.Value.Length;
                } else {
                    throw new Exceptions.TypeMismatchException( "s lit??" );
                }
            }
            else {
                // Count in memory until reach zero
                startAddress = address = param.Value.ToBigInteger();
                
                while( this.Machine.Memory.Read( address, 1 ) [ 0 ] != 0 ) {
                    ++address;
                    
                    if ( address >= this.Machine.Memory.Max ) {
                        throw new Exceptions.IncorrectAddressException(
                                            L18n.Get( L18n.Id.ExcInvalidMemory )
                                            + this.Machine.Memory.Max );
                    }
                }
            }
			
            var result = new IntLiteral( this.Machine, address - startAddress );
            var resultVble = Variable.CreateTempVariable( result );
			this.Machine.ExecutionStack.Push( resultVble );
		}

		private static StrLen instance = null;
		private static Variable[] strLenFormalParameters;
	}
}
