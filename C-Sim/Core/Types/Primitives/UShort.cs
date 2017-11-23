// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.Types.Primitives {
   using CSim.Core.Literals;

    /// <summary>
    /// Represents the UShort type, for unsigned short integer numbers.
    /// </summary>
    public class UShort: Primitive {
        /// <summary>
        /// The name of the type.
        /// </summary>
        public const string TypeName = "ushort";

        /// <summary>
        /// Initializes a new instance of the <see cref="CSim.Core.Types.Primitives.UShort"/> class.
        /// Its size is guaranteed to be the half the width of the system.
        /// </summary>
        private UShort(Machine m)
            :base( m, TypeName, m.WordSize >> 1 )
        {
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="CSim.Core.Types.Primitives.UShort"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="CSim.Core.Types.Primitives.UShort"/>.</returns>
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
            return new UShortLiteral( this.Machine, this.Machine.Bytes.FromBytesToShort( raw ) );
        }

        /// <summary>
        /// Creates a literal for this type, given a value.
        /// </summary>
        /// <returns>The literal, as an appropriate object of a class inheriting from Literal.</returns>
        /// <param name="v">The given value.</param>
        public override Literal CreateLiteral(object v)
        {
            return new UShortLiteral( this.Machine, v );
        }
        
        /// <summary>
        /// Gets the only instance for this <see cref="AType"/>.
        /// </summary>
        /// <returns>The <see cref="AType"/>.</returns>
        /// <param name="m">The <see cref="Machine"/> this type will live in.</param>
        public static UShort Get(Machine m)
        {
            if ( instance == null ) {
                instance = new UShort( m );
            }

            return instance;
        }

        /// <summary>The only instance for this type.</summary>
        protected static UShort instance;
    }
}

