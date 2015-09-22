using System;

namespace CSim.Core.Literals {
    /// <summary>
    /// Literals of type Char.
    /// </summary>
    public class CharLiteral: Literal {
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
        public override byte[] GetRawValue(Machine m)
		{
			return m.CnvtCharToBytes( this.Value );
        }

		public override string ToString()
		{
			char value = this.Value;
			string toret = string.Format( "({0})", Literal.ToPrettyNumber( value ) );

			if ( !char.IsControl( value ) ) {
				toret = String.Format( "'{0}' ", char.ToString( value ) ) + toret;
			}

			return toret;
		}
    }
}

