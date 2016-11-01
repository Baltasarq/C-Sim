using System;

using CSim.Core.Types.Primitives;

namespace CSim.Core.Literals {
    /// <summary>
    /// Literals of type Int.
    /// </summary>
    public class IntLiteral: Literal {
        public IntLiteral(Machine m, long x)
			:base( m, x )
        {
        }

        /// <summary>
        /// Gets the type of the int literal.
        /// The type is CSim.Core.Types.Primitives.Int
        /// </summary>
        /// <value>The type.</value>
        public override CSim.Core.Type Type {
            get {
                return this.Machine.TypeSystem.GetIntType();
            }
        }

        /// <summary>
        /// The value stored, as an integer value.
        /// </summary>
        /// <value>The value.</value>
        public new long Value {
            get {
                return (long) base.Value;
            }
        }

        /// <summary>
        /// Gets the raw value of the literal, as sequence of bytes.
        /// </summary>
        /// <value>The raw value.</value>
        public override byte[] GetRawValue()
        {
			return this.Machine.Bytes.FromIntToBytes( this.Value );
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

