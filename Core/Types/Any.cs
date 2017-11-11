// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.Types {
	using CSim.Core.Literals;
	using CSim.Core.Variables;

	/// <summary>
	/// Infinite union of all types.
	/// </summary>
	public class Any: AType
	{
		/// <summary>The name of the type.</summary>
		public const string TypeName = "any";

		private Any(Machine m)
			: base( m, TypeName, 1 )
		{
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="CSim.Core.Types.Any"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> with the name of the type: <see cref="TypeName"/>.</returns>
		public override string ToString()
		{
			return this.Name;
		}

		/// <summary>
		/// Creates a corresponding variable.
		/// </summary>
		/// <returns>A <see cref="Variable"/> with this <see cref="AType"/>.</returns>
		/// <param name="strId">The identifier for the variable.</param>
		public override Variable CreateVariable(string strId)
		{
			return new VoidPtrVariable( new Id( this.Machine, strId ) );
		}

		/// <summary>
		/// Creates a literal for this type, given a byte sequence.
		/// The particularity here is that this type is Any, so the more agnostic type is char.
		/// </summary>
		/// <returns>A <see cref="CharLiteral"/>.</returns>
		/// <param name="raw">The sequence of bytes containing the value in memory. Only the fist one matters.</param>
		public override Literal CreateLiteral(byte[] raw)
		{
			return new CharLiteral( this.Machine, (char) raw[0] );
		}

		/// <summary>
		/// Creates a literal for this type, given a value.
		/// </summary>
		/// <returns>The literal, as an appropriate object of a class inheriting from Literal.</returns>
		/// <param name="v">The given value.</param>
		public override Literal CreateLiteral(object v)
		{
			return new CharLiteral( this.Machine, v );
		}

		/// <summary>
		/// Gets the Any type. This type does not depend
        /// in any form from the machine's structure.
        /// <returns>The <see cref="AType"/>.</returns>
        /// <param name="m">The <see cref="Machine"/> this type will live in.</param>
		/// </summary>
		public static Any Get(Machine m)
		{
            if ( instance == null ) {
                instance = new Any( m );
            }

            return (Any) instance;
        }

        /// <summary>The only instance for this type.</summary>
        protected static AType instance;
	}
}

