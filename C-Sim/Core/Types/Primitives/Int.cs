﻿// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.Types.Primitives {
    using CSim.Core.Literals;

	/// <summary>
	/// Represents the Int type, for integer numbers.
	/// </summary>
    public class Int: Primitive {
		/// <summary>
		/// The name of the type.
		/// </summary>
        public const string TypeName = "int";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Types.Primitives.Int"/> class.
		/// Its size is guaranteed to be the same for the width of the system.
		/// </summary>
        private Int(Machine m)
            :base( m, TypeName, m.WordSize )
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
		/// <param name="raw">The raw bytes needed to build the literal.</param>
		public override Literal CreateLiteral(byte[] raw)
		{
			return new IntLiteral( this.Machine, this.Machine.Bytes.FromBytesToInt( raw ) );
		}

		/// <summary>
		/// Creates a literal for this type, given a value.
		/// </summary>
		/// <returns>The literal, as an appropriate object of a class inheriting from Literal.</returns>
		/// <param name="v">The given value.</param>
		public override Literal CreateLiteral(object v)
		{
			return new IntLiteral( this.Machine, v );
		}
        
        /// <summary>
        /// Gets the only instance for this <see cref="AType"/>.
        /// </summary>
        /// <returns>The <see cref="AType"/>.</returns>
        /// <param name="m">The <see cref="Machine"/> this type will live in.</param>
        public static Int Get(Machine m)
        {
            if ( instance == null ) {
                instance = new Int( m );
            }

            return instance;
        }

        /// <summary>The only instance for this type.</summary>
        protected static Int instance;
    }
}

