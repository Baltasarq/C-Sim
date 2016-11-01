using System;

using CSim.Core.Types;

namespace CSim.Core.Literals {
    /// <summary>
    /// Literals of type char*.
    /// </summary>
    public class StrLiteral: Literal {
        public StrLiteral(Machine m, string x)
			:base( m, x )
        {
        }

        /// <summary>
        /// Gets the type of the char* literal.
        /// The type is Ptr with CSim.Core.Types.Primitives.Char
        /// </summary>
        /// <value>The type.</value>
        public override CSim.Core.Type Type {
            get {
				return this.Machine.TypeSystem.GetPtrType(
							this.Machine.TypeSystem.GetCharType() );
            }
        }

        /// <summary>
        /// The value stored, of type string.
        /// </summary>
        /// <value>The value.</value>
        public new string Value {
            get {
                return (string) base.Value;
            }
        }

        /// <summary>
        /// Gets the raw value of the literal, as sequence of bytes.
        /// </summary>
        /// <value>The raw value.</value>
        public override byte[] GetRawValue()
		{
            return Machine.TextEncoding.GetBytes( this.Value.ToCharArray() );
        }

		public override string ToString()
		{
			return String.Format( "\"{0}\"", this.Value );
		}
    }
}

