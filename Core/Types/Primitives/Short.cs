// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.Types.Primitives {
   using CSim.Core.Literals;

    /// <summary>
    /// Represents the Short type, for short integer numbers.
    /// </summary>
    public class Short: Primitive {
        /// <summary>
        /// The name of the type.
        /// </summary>
        public const string TypeName = "short";

        /// <summary>
        /// Initializes a new instance of the <see cref="CSim.Core.Types.Primitives.Short"/> class.
        /// Its size is guaranteed to be the half the width of the system.
        /// </summary>
        private Short(Machine m)
            :base( m, TypeName, m.WordSize >> 1 )
        {
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="CSim.Core.Types.Primitives.Short"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="CSim.Core.Types.Primitives.Short"/>.</returns>
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
            return new ShortLiteral( this.Machine, this.Machine.Bytes.FromBytesToShort( raw ) );
        }

		/// <summary>
		/// Creates a literal for this type, given a value.
		/// </summary>
		/// <returns>The literal, as an appropriate object of a class inheriting from Literal.</returns>
		/// <param name="v">The given value.</param>
		public override Literal CreateLiteral(object v)
		{
			return new ShortLiteral( this.Machine, v );
		}
        
        /// <summary>
        /// Gets the only instance for this <see cref="AType"/>.
        /// </summary>
        /// <returns>The <see cref="AType"/>.</returns>
        /// <param name="m">The <see cref="Machine"/> this type will live in.</param>
        public static Short Get(Machine m)
        {
            if ( instance == null ) {
                instance = new Short( m );
            }

            return instance;
        }

        /// <summary>The only instance for this type.</summary>
        protected static Short instance;
    }
}

