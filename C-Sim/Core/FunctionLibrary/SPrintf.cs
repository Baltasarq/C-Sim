// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.FunctionLibrary {
	using Functions;
    using Exceptions;
    using Literals;
    using Types;
    
    using System;
    using System.Globalization;
    using System.Numerics;
    using System.Text;

	/// <summary>
	/// This is the print function.
	/// Signature: void print(x); // x can be anything
	/// </summary>
	public sealed class SPrintf: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "sprintf";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Functions.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private SPrintf(Machine m)
			: base( m, Name, m.TypeSystem.GetIntType(), sprintFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static SPrintf Get(Machine m)
		{
			if ( instance == null ) {
                var ellipsis_id = new Id( m, @"x" );
                ellipsis_id.SetIdWithoutChecks( "..." );
                
				sprintFormalParams = new Variable[] {
                    new Variable( ellipsis_id, Any.Get( m ) )
				};

				instance = new SPrintf( m );
			}

			return instance;
		}

		/// <summary>
		/// Execute this <see cref="Function"/> with
		/// the specified parameters (<see cref="RValue"/>'s).
		/// </summary>
		/// <param name="realParams">The parameters.</param>
		public override void Execute(RValue[] realParams)
		{
            if ( realParams.Length > 1 ) {
                var paramTargetString = realParams[ 0 ].SolveToVariable();
	            var paramFormatString = realParams[ 1 ].SolveToVariable();
	            AType pchar_t = this.Machine.TypeSystem.GetPCharType();
                string data;
	            
                if ( paramTargetString.Type == this.Machine.TypeSystem.GetPCharType() )
                {
                    BigInteger address = paramTargetString.Value.ToBigInteger();
                    
                    this.Machine.Memory.CheckAddressFits( address );
                    
		            if ( paramFormatString.Type == pchar_t ) {
                        var prms = new RValue[ Math.Max( 0, realParams.Length - 2 ) ];
                        Array.Copy( realParams, 2, prms, 0, prms.Length );
	                    data = FormatParams( paramFormatString, prms );
		            } else {
	                    data = paramFormatString.LiteralValue.Value.ToString();
		            }

                    // Return value	
	                this.Machine.ExecutionStack.Push(
	                    new IntLiteral( this.Machine, data.Length ).SolveToVariable()
	                );
                    
                    // Copy textual data
                    this.Machine.Memory.CheckSizeFits( address, data.Length );
                    
                    this.Machine.Memory.Write( address, Encoding.ASCII.GetBytes( data ) );
                } else {
                    throw new RuntimeException(
                        paramTargetString.Name
                        + ": " + paramTargetString.Type + "??" );
                }
            }
            
            return;
		}
        
        /// <summary>
        /// Formats the given parameters, following the format string.
        /// </summary>
        /// <returns>A single string with all the formatted info.</returns>
        /// <param name="param0">
        /// The format string, as a <see cref="Variable"/> which must point
        /// to an array of chars, or be a temp variable with a literal string.
        /// </param>
        /// <param name="prms">An array of <see cref="RValue"/>'s.</param>
        public static string FormatParams(Variable param0, RValue[] prms)
        {
            string formatString;

            if ( param0.IsTemp() ) {
                formatString = (string) param0.LiteralValue.Value;
            } else {
                formatString = Encoding.ASCII.GetString(
                                param0.Machine.Memory.ReadStringFromMemory(
                                    param0.LiteralValue.ToBigInteger() )
                               );
            }
            
            return FormatParams( formatString, prms );
        }
        
        /// <summary>
        /// Formats the given parameters, following the format string.
        /// </summary>
        /// <returns>A single string with all the formatted info.</returns>
        /// <param name="formatString">The format string.</param>
        /// <param name="prms">An array of <see cref="RValue"/>'s.</param>
        public static string FormatParams(string formatString, RValue[] prms)
        {
            int numParam = 0;
            string toret = "";
            int frmLen = formatString.Length;
            
            for(int i = 0; i < frmLen; ++i) {
                if ( formatString[ i ] == '\\'
                  && ( i + 1 ) < frmLen )
                {
                    toret += FormatSpecialChar( formatString[ i + 1 ] );
                    i += 1;
                }
                else
                if ( formatString[ i ] == '%'
                  && ( i + 1 ) < frmLen )
                {
	                toret +=
	                    FormatDataSpecifier(
	                        formatString[ i + 1 ],
	                        RetrieveParamOrFail( ref numParam, prms ) );
                    i += 1;
                } else {
                    toret += formatString[ i ];
                }
            }
            
            return toret;
        }
        
        private static Variable RetrieveParamOrFail(ref int numParam, RValue[] prms)
        {
            Variable toret;
            
            if ( numParam < prms.Length ) {
                toret = prms[ numParam ].SolveToVariable();
                ++numParam;
            } else {
                throw new Exceptions.RuntimeException(
                                string.Format(
                                    L10n.Get( L10n.Id.ErrMissingArguments ) ) );
            }
            
            return toret;
        }
        
        /// <summary>
        /// Returns the true char behind a code such as \n
        /// </summary>
        /// <returns>The true char, or '?'.</returns>
        /// <param name="format">
        /// The special char codification, without the '\' prefix.
        /// </param>
        public static char FormatSpecialChar(char format)
        {
            char toret = '?';
            
            switch( format ) {
                case '\\':
                    toret = '\\';
                    break;
                case '"':
                    toret = '"';
                    break;
                case '\'':
                    toret = '\'';
                    break;
                case 'r':
                case 'n':
                    toret = '\n';
                    break;
                case 't':
                    toret = '\t';
                    break;
                case 'a':
                    toret = '\a';
                    break;
                case 'b':
                    toret = '\b';
                    break;
            }
            
            return toret;
        }
        
        /// <summary>
        /// Formats a given value following the data specifier.
        /// </summary>
        /// <returns>The value, as a string following the format.</returns>
        /// <param name="format">
        /// The format, such as 'd' (int), or 'f' (double)...
        /// </param>
        /// <param name="v">
        /// The value to format, as a <see cref="Variable"/>.
        /// </param>
        public static string FormatDataSpecifier(char format, Variable v)
        {
            string toret = "";
            
            switch( char.ToLower( format ) ) {
                case '%':
                    toret = "%";
                    break;
                case 'i':
                case 'd':
                    toret = v.LiteralValue.ToBigInteger().ToString();
                    break;
                case 'g':
                case 'e':
                case 'f':
                    toret = v.LiteralValue.ToFloat().ToString(
                                                CultureInfo.InvariantCulture );
                    break;                    
                case 's':
                    toret = RemoveQuotesIfPresent( GetStringFrom( v ) );
                    break;
                case 'c':
                    toret = v.LiteralValue.ToChar().ToString();
                    break;
                case 'p':
                case 'x':
                    toret = v.LiteralValue.ToPrettyHex();
                    break;
            }
            
            return toret;
        }
        
        private static string RemoveQuotesIfPresent(string s)
        {
            int start = 0;
            int finish = s.Length;
            
            if ( s[ 0 ] == '"'
              && s[ s.Length - 1 ] == '"' )
            {
                start = 1;
                finish -= 2;
            }
            
            return s.Substring( start, finish );
        }
        
        private static string GetStringFrom(Variable v)
        {
            string toret = v.LiteralValue.ToString();
            
            if ( !( v.LiteralValue is StrLiteral )
              && v.Type == v.Machine.TypeSystem.GetPCharType() )
            {
                BigInteger address = v.LiteralValue.ToBigInteger();
                toret = Encoding.ASCII.GetString(
                    v.Machine.Memory.ReadStringFromMemory( address ) );
            }
            
            return toret;
        }

		private static SPrintf instance;
		private static Variable[] sprintFormalParams;
	}
}
