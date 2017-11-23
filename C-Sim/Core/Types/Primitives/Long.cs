// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.Types.Primitives {
   using CSim.Core.Literals;

    /// <summary>
    /// Represents the Long type, for long integer numbers.
    /// </summary>
    public class Long: Primitive {
        /// <summary>
        /// The name of the type.
        /// </summary>
        public const string TypeName = "long";

        /// <summary>
        /// Initializes a new instance of the <see cref="CSim.Core.Types.Primitives.Long"/> class.
        /// Its size is guaranteed to be the double of the width of the system.
        /// </summary>
        private Long(Machine m)
            :base( m, TypeName, m.WordSize << 1 )
        {
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="CSim.Core.Types.Primitives.Long"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="CSim.Core.Types.Primitives.Long"/>.</returns>
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
            return new LongLiteral( this.Machine, this.Machine.Bytes.FromBytesToLong( raw ) );
        }

		/// <summary>
		/// Creates a literal for this type, given a value.
		/// </summary>
		/// <returns>The literal, as an appropriate object of a class inheriting from Literal.</returns>
		/// <param name="v">The given value.</param>
		public override Literal CreateLiteral(object v)
		{
			return new LongLiteral( this.Machine, v );
		}
        
        /// <summary>
        /// Gets the only instance for this <see cref="AType"/>.
        /// </summary>
        /// <returns>The <see cref="AType"/>.</returns>
        /// <param name="m">The <see cref="Machine"/> this type will live in.</param>
        public static Long Get(Machine m)
        {
            if ( instance == null ) {
                instance = new Long( m );
            }

            return instance;
        }

        /// <summary>The only instance for this type.</summary>
        protected static Long instance;
    }
}

