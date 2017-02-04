using System;

using CSim.Core.Literals;

namespace CSim.Core.Types.Primitives
{
	/// <summary>
	/// Represents the Int type, for integer numbers.
	/// </summary>
    public class Int: Primitive
    {
		/// <summary>
		/// The name of the type.
		/// </summary>
        public const string TypeName = "int";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Types.Primitives.Int"/> class.
		/// It's size is guaranteed to be the same for the width of the system.
		/// </summary>
        internal Int(int wordSize): base( TypeName, wordSize )
        {
        }

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="CSim.Core.Types.Primitives.Int"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="CSim.Core.Types.Primitives.Int"/>.</returns>
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
			return new IntLiteral( m, m.Bytes.FromBytesToInt( raw ) );
		}
    }
}

