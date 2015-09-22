using System;

using CSim.Core.Literals;

namespace CSim.Core.Types.Primitives {
    public class Double: Primitive {
        /// <summary>
        /// The name of the type.
        /// </summary>
        public const string TypeName = "double";

        /// <summary>
        /// Initializes a new instance of the <see cref="CSim.Core.Types.Primitives.Int"/> class.
        /// It's size is guaranteed to be the same for the width of the system.
        /// </summary>
        internal Double(int wordSize): base( TypeName, wordSize * 2 )
        {
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="CSim.Core.Types.Primitives.Double"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="CSim.Core.Types.Primitives.Double"/>.</returns>
        public override string ToString()
        {
            return this.Name;
        }

		public override Literal CreateLiteral(Machine m, byte[] raw)
		{
			return new DoubleLiteral( m, m.CnvtBytesToDouble( raw ) );
		}
    }
}
