// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.Literals {
    using System.Numerics;
    
    using Exceptions;
    
    /// <summary>
    /// Literals of type char*.
    /// </summary>
    public class StrLiteral: Literal {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Literals.StrLiteral"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/>.</param>
		/// <param name="x">A given string.</param>
        public StrLiteral(Machine m, string x)
			:base( m, x )
        {
        }

        /// <summary>
        /// Gets the type of the char* literal.
        /// The type is Ptr to <see cref="Types.Primitives.Char"/>.
        /// </summary>
        /// <value>The <see cref="AType"/>.</value>
        public override AType Type {
            get {
				return this.Machine.TypeSystem.GetPCharType();
            }
        }

        /// <summary>
        /// The value stored, of type string.
        /// </summary>
        /// <value>The value.</value>
        public new string Value {
            get {
                return (string) base.Value;
            }
        }

        /// <summary>
        /// Gets the raw value of the literal, as sequence of bytes.
        /// </summary>
        /// <value>The raw value.</value>
        public override byte[] GetRawValue()
		{
            return Machine.TextEncoding.GetBytes( this.Value.ToCharArray() );
        }
        
        /// <summary>
        /// Gets the value as an integer.
        /// </summary>
        /// <returns>Does not return any value.</returns>
        /// <exception cref="EngineException">Always.</exception>
        public override BigInteger GetValueAsInteger()
        {
            throw new RuntimeException( "cannot transform literal to int" );
        }

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Literals.StrLiteral"/>.
		/// </summary>
		/// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Literals.StrLiteral"/>.</returns>
		public override string ToString()
		{
			return string.Format( "\"{0}\"", this.Value );
		}
    }
}

