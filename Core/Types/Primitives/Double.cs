
namespace CSim.Core.Types.Primitives {
	using CSim.Core.Literals;

	/// <summary>The type representing double values.</summary>
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

		/// <summary>
		/// Creates the corresponding literal.
		/// </summary>
		/// <returns>The <see cref="Literal"/>.</returns>
		/// <param name="m">The <see cref="Machine"/>.</param>
		/// <param name="raw">The raw bytes needed to build the literal.</param>
		public override Literal CreateLiteral(Machine m, byte[] raw)
		{
			return new DoubleLiteral( m, m.Bytes.FromBytesToDouble( raw ) );
		}
    }
}
