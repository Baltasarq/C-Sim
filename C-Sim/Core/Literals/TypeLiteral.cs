// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.Literals {
    using System.Numerics;
    
    /// <summary>
    /// <see cref="AType"/> <see cref="Literal"/>'s.
    /// </summary>
    public class TypeLiteral: Literal {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Literals.TypeLiteral"/> class.
		/// </summary>
        /// <param name="m">The <see cref="Machine"/> .</param>
		/// <param name="v">The value for the literal.</param>
		public TypeLiteral(Machine m, byte v)
			:base( m, v )
		{
		}
        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:CSim.Core.Literals.TypeLiteral"/> class.
        /// </summary>
        /// <param name="v">The value for the literal.</param>
        public TypeLiteral(AType v)
			:base( v.Machine, v.Machine.Bytes.FromTypeToBytes( v )[ 0 ] )
        {
        }

        /// <summary>
        /// Gets the type of the <see cref="TypeLiteral"/>.
        /// The type is <see cref="Types.TypeType"/>.
        /// </summary>
        /// <value>The <see cref="AType"/>, which is <see cref="Types.TypeType"/>.</value>
        public override AType Type {
            get {
                return Types.TypeType.Get( this.Machine );
            }
        }

        /// <summary>
        /// The value stored, as a type.
        /// </summary>
        /// <value>The value.</value>
        public new AType Value {
            get {
				return this.Machine.Bytes.FromBytesToType(new [] { (byte) base.Value } );
            }
        }

        /// <summary>
        /// Gets the raw value of the literal, as sequence of bytes.
        /// </summary>
        /// <value>The raw value.</value>
        public override byte[] GetRawValue()
        {
            return new []{ (byte) base.Value };
        }
        
        /// <summary>
        /// Gets the value as an integer.
        /// </summary>
        /// <returns>The value as a <see cref="BigInteger"/>.</returns>
        public override BigInteger GetValueAsInteger()
        {
            return this.Value.Size.ToBigInteger();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:TypeLiteral"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:TypeLiteral"/>.
        /// </returns>
        public override string ToString()
        {
            return this.Value.ToString();
        }

        /// <summary>
        /// Creates a <see cref="TypeLiteral"/> from a given string.
        /// For example, it creates the corresponding TypeLiteral from "int **"
        /// </summary>
        /// <returns>The corresponding <see cref="TypeLiteral"/>.</returns>
        /// <param name="m">The <see cref="Machine"/> this literal will live in.</param>
        /// <param name="strTypeLit">The type literal, as a string.</param>
        public static TypeLiteral CreateFromString(Machine m, string strTypeLit)
        {
            TypeLiteral toret = null;
            AType type = m.TypeSystem.FromStringToType( strTypeLit );
            
            if ( type != null ) {
                toret = new TypeLiteral( type );
            }
            
            return toret;
        }
    }
}
