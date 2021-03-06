﻿// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.Literals {
    using System.Numerics;
    
    /// <summary>
    /// Literals of type Char.
    /// </summary>
    public class CharLiteral: Literal {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Literals.CharLiteral"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/>.</param>
		/// <param name="ch">A given char.</param>
		public CharLiteral(Machine m, object ch)
			:this( m, ch.ToChar() )
		{
		}

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
        /// Gets the type of the <see cref="CharLiteral"/>.
        /// The type is <see cref="Core.Types.Primitives.Char"/>.
        /// </summary>
        /// <value>The <see cref="AType"/>.</value>
        public override AType Type {
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
        /// Gets the value as an integer.
        /// </summary>
        /// <returns>The value as <see cref="BigInteger"/>.</returns>
        public override BigInteger GetValueAsInteger()
        {
            return new BigInteger( this.Value );
        }

		/// <summary>
		/// Converts the info to the shortest possible string.
		/// </summary>
		/// <returns>The array element, represented as a string.</returns>
		public override string AsArrayElement()
		{
			char value = this.Value;
			var toret = string.Format( "{0}", this.ToHex() );

			if ( !char.IsControl( value ) ) {
				toret = string.Format( "{0}", char.ToString( value ) );
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
			var toret = string.Format( "({0})", this.ToPrettyNumber() );

			if ( !char.IsControl( value ) ) {
				toret = string.Format( "'{0}' ", char.ToString( value ) ) + toret;
			}

			return toret;
		}
    }
}

