// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.Literals {
    using System.Numerics;
    
    /// <summary>
    /// Literals of type Int.
    /// </summary>
    public class UIntLiteral: Literal {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:CSim.Core.Literals.IntLiteral"/> class.
        /// </summary>
        /// <param name="m">The <see cref="Machine"/>.</param>
        /// <param name="x">A given integer.</param>
        public UIntLiteral(Machine m, object x)
            :this( m, GetValueAsUIntFromUnsigned( m, x ) )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CSim.Core.Literals.IntLiteral"/> class.
        /// </summary>
        /// <param name="m">The <see cref="Machine"/>.</param>
        /// <param name="x">A given integer.</param>
        public UIntLiteral(Machine m, BigInteger x)
            :base( m, x )
        {
        }

        /// <summary>
        /// Gets the type of the int literal.
        /// The type is <see cref="CSim.Core.Types.Primitives.Int"/>.
        /// </summary>
        /// <value>The <see cref="AType"/>.</value>
        public override AType Type {
            get {
                return this.Machine.TypeSystem.GetUIntType();
            }
        }

        /// <summary>
        /// The value stored, as an integer value.
        /// </summary>
        /// <value>The value.</value>
        public new BigInteger Value {
            get {
                return (BigInteger) base.Value;
            }
        }

        /// <summary>
        /// Gets the raw value of the literal, as sequence of bytes.
        /// </summary>
        /// <value>The raw value.</value>
        public override byte[] GetRawValue()
        {
            return this.Machine.Bytes.FromUIntToBytes( this.Value );
        }
        
        /// <summary>
        /// Gets the value as an integer.
        /// </summary>
        /// <returns>The value as a <see cref="BigInteger"/>.</returns>
        public override BigInteger GetValueAsInteger()
        {
            return GetValueAsUIntFromUnsigned( this.Machine, this.Value );
        }
        
        /// <summary>
        /// Returns a string appropriately formatted with the value of the literal.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="CSim.Core.Literals.IntLiteral"/>.</returns>
        public override string ToString()
        {
            return this.ToPrettyNumber();
        }
        
        /// <summary>
        /// Gets the value as an integral from an unsigned value.
        /// It uses the byte representation from the machine to do the conversion,
        /// since the standard library would throw an exception.
        /// </summary>
        /// <returns>The value as a <see cref="BigInteger"/>.</returns>
        /// <param name="m">The machine this value will be converted for.</param>
        /// <param name="x">The value itself.</param>
        public static BigInteger GetValueAsUIntFromUnsigned(Machine m, object x)
        {
            return m.Bytes.FromBytesToUInt( m.Bytes.FromIntToBytes( x.ToBigInteger() ) );
        }
    }
}
