
namespace CSim.Core.Types.Primitives {
	using CSim.Core.Literals;

	/// <summary>
	/// Represents the Int type, for integer numbers.
	/// </summary>
	public class Int32: Primitive {
		/// <summary>The length in bytes for this type.</summary>
		public const int LengthInBytes = 4;

		/// <summary>
		/// The name of the type.
		/// </summary>
		public const string TypeName = "int32";

		/// <summary>
		/// Initializes a new instance of the <see cref="Int32"/> class.
		/// Its size is guaranteed to be the same for the width of the system.
		/// </summary>
		private Int32(Machine m)
			:base( m, TypeName, LengthInBytes )
		{
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Int32"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Int32"/>.</returns>
		public override string ToString()
		{
			return this.Name;
		}

		/// <summary>
		/// Creates the corresponding literal.
		/// </summary>
		/// <returns>The <see cref="Literal"/>.</returns>
		/// <param name="raw">The raw bytes needed to build the literal.</param>
		public override Literal CreateLiteral(byte[] raw)
		{
			return new Int32Literal(
				this.Machine,
				this.Machine.Bytes.FromBytesToIntegral( raw, LengthInBytes ) );
		}

		/// <summary>
		/// Creates a literal for this type, given a value.
		/// </summary>
		/// <returns>The literal, as an appropriate object of a class inheriting from Literal.</returns>
		/// <param name="v">The given value.</param>
		public override Literal CreateLiteral(object v)
		{
			return new Int32Literal( this.Machine, v );
		}

		/// <summary>
		/// Gets the only instance for this <see cref="AType"/>.
		/// </summary>
		/// <returns>The <see cref="AType"/>.</returns>
		/// <param name="m">The <see cref="Machine"/> this type will live in.</param>
		public static Int32 Get(Machine m)
		{
			if ( instance == null ) {
				instance = new Int32( m );
			}

			return instance;
		}

		/// <summary>The only instance for this type.</summary>
		protected static Int32 instance;
	}
}
