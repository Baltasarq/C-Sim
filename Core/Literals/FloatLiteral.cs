// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.Literals {
	using System;
    using System.Numerics;
	using System.Globalization;

    /// <summary>
    /// Literals of type <see cref="double"/>.
    /// </summary>
    public class FloatLiteral: Literal {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Literals.FloatLiteral"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/>.</param>
		/// <param name="x">A given double.</param>
		public FloatLiteral(Machine m, object x)
			:this( m, x.ToFloat() )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Literals.FloatLiteral"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/>.</param>
		/// <param name="x">A given double.</param>
        public FloatLiteral(Machine m, float x)
			:base( m, x )
        {
        }

        /// <summary>
		/// Gets the type of the <see cref="FloatLiteral"/>.
        /// The type is <see cref="CSim.Core.Types.Primitives.Float"/>
        /// </summary>
        /// <value>The <see cref="AType"/>.</value>
        public override AType Type {
            get {
				return this.Machine.TypeSystem.GetFloatType();
            }
        }

        /// <summary>
        /// The value stored, as an integer value.
        /// </summary>
        /// <value>The value.</value>
        public new double Value {
            get {
				return base.Value.ToDouble();
            }
        }

        /// <summary>
        /// Gets the raw value of the literal, as secquence of bytes.
        /// </summary>
        /// <value>The raw value.</value>
        public override byte[] GetRawValue()
        {
			return this.Machine.Bytes.FromFloatToBytes( this.Value );
        }
        
        /// <summary>
        /// Gets the value as an integer.
        /// </summary>
        /// <returns>The value as <see cref="BigInteger"/>.</returns>
        public override BigInteger GetValueAsInteger()
        {
            return this.Value.ToBigInteger();
        }

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Literals.FloatLiteral"/>.
		/// </summary>
		/// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Literals.FloatLiteral"/>.</returns>
		public override string ToString()
		{
			return this.Value.ToString( "N2", CultureInfo.InvariantCulture );
		}
    }
}
