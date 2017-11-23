// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.Types {
    using Variables;

    /// <summary>
    /// A type for a <see cref="Core.Variables.TypeVariable"/>'s.
    /// </summary>
    public class TypeType: AType {
        /// <summary>The name of the type.</summary>
        public const string TypeName = "type_t";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CSim.Core.Types.TypeType"/> class.
        /// <param name="m">The <see cref="Machine"/> this type will live in.</param>
        /// </summary>
        public TypeType(Machine m)
            : base( m, TypeName, 1 )
        {
        }

		/// <summary>
		/// Creates a corresponding variable.
		/// </summary>
		/// <returns>A <see cref="Variable"/> honoring this <see cref="AType"/>.</returns>
		/// <param name="strId">The identifier for the <see cref="Variable"/>.</param>
		public override Variable CreateVariable(string strId)
		{
			return new TypeVariable( new Id( this.Machine, strId ) );
		}

        /// <summary>
        /// Creates a new literal for this type.
        /// </summary>
        /// <returns>The literal, as a <see cref="Literals.TypeLiteral"/>.</returns>
        /// <param name="raw">A raw representation of the value in memory.</param>
        public override Literal CreateLiteral(byte[] raw)
        {
            return new Literals.TypeLiteral( this.Machine, raw[ 0 ] );
        }

		/// <summary>
		/// Creates a literal for this type, given a value.
		/// </summary>
		/// <returns>The literal, as an appropriate object of a class inheriting from Literal.</returns>
		/// <param name="v">The given value.</param>
		public override Literal CreateLiteral(object v)
		{
			return new Literals.TypeLiteral(
                                    this.Machine,
                                    v.ToBigInteger().ToByte() );
		}

        /// <summary>
        /// Returns the only instance for this <see cref="AType"/>.
        /// </summary>
        /// <param name="m">The <see cref="Machine"/> this type will live in.</param>
        /// <returns>The <see cref="TypeType"/> <see cref="AType"/>.</returns>
        public static TypeType Get(Machine m)
        {
            if ( typeTypeInstance == null ) {
                typeTypeInstance = new TypeType( m );
            }

            return typeTypeInstance;
        }

        private static TypeType typeTypeInstance;
    }
}
