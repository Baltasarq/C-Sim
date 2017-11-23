// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.Literals {
    using System.Numerics;
	using Core.Types.Primitives;
    
    /// <summary>
    /// Literals of type Int8.
    /// </summary>
	/// <seealso cref="Core.Types.Primitives.Int8"/>
    public class Int8Literal: Literal {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:Int8Literal"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/>.</param>
		/// <param name="x">A given integral value.</param>
		public Int8Literal(Machine m, object x)
			:this( m, x.ToBigInteger() )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Int8Literal"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/>.</param>
		/// <param name="x">A given integer.</param>
		public Int8Literal(Machine m, BigInteger x)
			:base( m, x )
        {
        }

        /// <summary>
        /// Gets the type of the int literal.
        /// The type is <see cref="Int8"/>.
        /// </summary>
        /// <value>The <see cref="AType"/>.</value>
        public override AType Type {
            get {
				return this.Machine.TypeSystem.GetBasicType( Int8.TypeName );
            }
        }

        /// <summary>
        /// The value stored, as an integer value.
        /// </summary>
        /// <value>The value.</value>
        public new BigInteger Value {
            get {
                return (BigInteger) base.Value;
            }
        }

        /// <summary>
        /// Gets the raw value of the literal, as sequence of bytes.
        /// </summary>
        /// <value>The raw value.</value>
        public override byte[] GetRawValue()
        {
			return this.Machine.Bytes.FromIntegralToBytes( this.Value,  Int8.LengthInBytes );
        }
        
        /// <summary>
        /// Gets the value as an integer.
        /// </summary>
        /// <returns>The value as <see cref="BigInteger"/>.</returns>
        public override BigInteger GetValueAsInteger()
        {
            return this.Value;
        }
        
		/// <summary>
		/// Returns a string appropriately formatted with the value of the literal.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="T:Int8Literal"/>.</returns>
		public override string ToString()
		{
			return this.ToPrettyNumber();
		}
    }
}
