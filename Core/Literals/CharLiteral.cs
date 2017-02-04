using System;

namespace CSim.Core.Literals {
    /// <summary>
    /// Literals of type Char.
    /// </summary>
    public class CharLiteral: Literal {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Literals.CharLiteral"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/>.</param>
		/// <param name="ch">A given char.</param>
        public CharLiteral(Machine m, char ch)
			:base( m, ch )
        {
        }

        /// <summary>
        /// Gets the type of the char literal.
        /// The type is CSim.Core.Types.Primitives.Char
        /// </summary>
        /// <value>The type.</value>
        public override CSim.Core.Type Type {
            get {
				return this.Machine.TypeSystem.GetCharType();
            }
        }

        /// <summary>
        /// The value stored, of primitive type char.
        /// </summary>
        /// <value>The value.</value>
        public new char Value {
            get {
                return (char) base.Value;
            }
        }

        /// <summary>
        /// Gets the raw value of the literal, as secquence of bytes.
        /// </summary>
        /// <value>The raw value.</value>
        public override byte[] GetRawValue()
		{
			return this.Machine.Bytes.FromCharToBytes( this.Value );
        }

		/// <summary>
		/// Converts the info to the shortest possible string.
		/// </summary>
		/// <returns>The array element, represented as a string.</returns>
		public override string AsArrayElement()
		{
			char value = this.Value;
			string toret = string.Format( "{0}", this.ToHex() );

			if ( !char.IsControl( value ) ) {
				toret = String.Format( "{0}", char.ToString( value ) );
			}

			return toret;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="CSim.Core.Literals.CharLiteral"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="CSim.Core.Literals.CharLiteral"/>.</returns>
		public override string ToString()
		{
			char value = this.Value;
			string toret = string.Format( "({0})", this.ToPrettyNumber() );

			if ( !char.IsControl( value ) ) {
				toret = String.Format( "'{0}' ", char.ToString( value ) ) + toret;
			}

			return toret;
		}
    }
}

