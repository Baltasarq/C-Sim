
namespace CSim.Core.Literals {
	using System.Globalization;

    /// <summary>
    /// Literals of type <see cref="double"/>.
    /// </summary>
    public class DoubleLiteral: Literal {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Literals.DoubleLiteral"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/>.</param>
		/// <param name="x">A given double.</param>
        public DoubleLiteral(Machine m, double x)
			:base( m, x )
        {
        }

        /// <summary>
        /// Gets the type of the <see cref="DoubleLiteral"/>.
        /// The type is <see cref="CSim.Core.Types.Primitives.Double"/>
        /// </summary>
        /// <value>The <see cref="AType"/>.</value>
        public override AType Type {
            get {
				return this.Machine.TypeSystem.GetDoubleType();
            }
        }

        /// <summary>
        /// The value stored, as an integer value.
        /// </summary>
        /// <value>The value.</value>
        public new double Value {
            get {
                return (double) base.Value;
            }
        }

        /// <summary>
        /// Gets the raw value of the literal, as secquence of bytes.
        /// </summary>
        /// <value>The raw value.</value>
        public override byte[] GetRawValue()
        {
			return this.Machine.Bytes.FromDoubleToBytes( this.Value );
        }

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Literals.DoubleLiteral"/>.
		/// </summary>
		/// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Literals.DoubleLiteral"/>.</returns>
		public override string ToString()
		{
			return this.Value.ToString( "N2", CultureInfo.InvariantCulture );
		}
    }
}
