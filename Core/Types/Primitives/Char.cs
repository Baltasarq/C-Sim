// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.Types.Primitives {
	using CSim.Core.Literals;

	/// <summary>
	/// Represents the Char type, for values from 0...255,
	/// it is the ISO-8859-1 code for characters.
	/// </summary>
    /// <seealso cref="Machine.TextEncoding"/>
    public class Char: Primitive
    {
		/// <summary>
		/// The name of the type.
		/// </summary>
        public const string TypeName = "char";
        
		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Types.Primitives.Char"/> class.
		/// It's size is guaranteed to be 1.
		/// </summary>
        private Char(Machine m)
            : base( m, TypeName, 1 )
        {
        }

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="CSim.Core.Types.Primitives.Char"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="CSim.Core.Types.Primitives.Char"/>.</returns>
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
			return new CharLiteral( this.Machine, this.Machine.Bytes.FromBytesToChar( raw ) );
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
        /// Gets the only instance for this <see cref="AType"/>.
        /// </summary>
        /// <returns>The <see cref="AType"/>.</returns>
        /// <param name="m">The <see cref="Machine"/> this type will be used in.</param>
        public static Char Get(Machine m)
        {
            if ( instance == null ) {
                instance = new Char( m );
            }

            return instance;
        }

        /// <summary>The only instance for this type.</summary>
        protected static Char instance;
   }
}

