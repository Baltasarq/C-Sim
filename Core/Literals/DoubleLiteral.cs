using System;
using System.Globalization;

using CSim.Core.Types.Primitives;

namespace CSim.Core.Literals {
    /// <summary>
    /// Literals of type <see cref="Double"/>.
    /// </summary>
    public class DoubleLiteral: Literal {
        public DoubleLiteral(Machine m, double x)
			:base( m, x )
        {
        }

        /// <summary>
        /// Gets the type of the double literal.
        /// The type is <see cref="CSim.Core.Types.Primitives.Double"/>
        /// </summary>
        /// <value>The type.</value>
        public override CSim.Core.Type Type {
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
        public override byte[] GetRawValue(Machine m)
        {
            return m.CnvtDoubleToBytes( this.Value );
        }

		public override string ToString()
		{
			return this.Value.ToString( "N2", CultureInfo.InvariantCulture );
		}
    }
}
