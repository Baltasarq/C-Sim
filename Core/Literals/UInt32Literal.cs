
namespace CSim.Core.Literals {
    using System.Numerics;
	using Core.Types.Primitives;
    
    /// <summary>
    /// Literals of type UInt32.
    /// </summary>
	/// <seealso cref="Core.Types.Primitives.UInt32"/>
	public class UInt32Literal: Literal {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:UInt32Literal"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/>.</param>
		/// <param name="x">A given integral value.</param>
		public UInt32Literal(Machine m, object x)
			:this( m, x.ToBigInteger() )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:UInt32Literal"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/>.</param>
		/// <param name="x">A given integer.</param>
		public UInt32Literal(Machine m, BigInteger x)
			:base( m, x )
        {
        }

        /// <summary>
        /// Gets the type of the int literal.
        /// The type is <see cref="UInt32"/>.
        /// </summary>
        /// <value>The <see cref="AType"/>.</value>
        public override AType Type {
            get {
				return this.Machine.TypeSystem.GetBasicType( UInt32.TypeName );
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
			return this.Machine.Bytes.FromUnsignedIntegralToBytes( this.Value,  UInt32.LengthInBytes );
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
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="UInt32Literal"/>.</returns>
		public override string ToString()
		{
			return this.ToPrettyNumber();
		}
    }
}
