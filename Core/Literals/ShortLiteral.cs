
namespace CSim.Core.Literals {
    using System.Numerics;
    
    /// <summary>
    /// Literals of type Int.
    /// </summary>
    public class ShortLiteral: Literal {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Literals.IntLiteral"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/>.</param>
		/// <param name="x">A given integer.</param>
		public ShortLiteral(Machine m, object x)
			:this( m, x.ToBigInteger() )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Literals.IntLiteral"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/>.</param>
		/// <param name="x">A given integer.</param>
		public ShortLiteral(Machine m, BigInteger x)
			:base( m, x )
        {
        }

        /// <summary>
        /// Gets the type of the int literal.
        /// The type is <see cref="CSim.Core.Types.Primitives.Int"/>.
        /// </summary>
        /// <value>The <see cref="AType"/>.</value>
        public override AType Type {
            get {
                return this.Machine.TypeSystem.GetShortType();
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
			return this.Machine.Bytes.FromShortToBytes( this.Value );
        }
        
        /// <summary>
        /// Gets the value as an integer.
        /// </summary>
        /// <returns>The value as a <see cref="BigInteger"/>.</returns>
        public override BigInteger GetValueAsInteger()
        {
            return this.Value;
        }
        
		/// <summary>
		/// Returns a string appropriately formatted with the value of the literal.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="CSim.Core.Literals.IntLiteral"/>.</returns>
		public override string ToString()
		{
			return this.ToPrettyNumber();
		}
    }
}
