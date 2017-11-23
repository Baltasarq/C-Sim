// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.Types {
    using Literals;
    
    /// <summary>
    /// A type for this <see cref="T:CSim.Core.Machine" />,
    /// referencing another <see cref="AType"/>.
    /// </summary>
    public abstract class Indirection: AType {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:CSim.Core.Types.Indirection"/> class, deriving from
        /// <see cref="AType"/>.
        /// </summary>
        /// <param name="m">The <see cref="Machine"/> for this type.</param>
        /// <param name="n">The name of this type.</param>
        protected Indirection(Machine m, string n)
            :base( m, n, m.WordSize )
        {
        }
        
        /// <summary>
        /// Creates a literal for this type, given a byte sequence.
        /// </summary>
        /// <returns>The literal, as an appropriate object of a class
        /// inheriting from <see cref="Literal"/>.</returns>        
        /// <param name="raw">The sequence of bytes containing the value in memory.</param>
        public override Literal CreateLiteral(byte[] raw)
        {
            return new IntLiteral( this.Machine, this.Machine.Bytes.FromBytesToInt( raw ) );
        }
        
        /// <summary>
        /// Creates a literal for this type, given a value.
        /// </summary>
        /// <returns>The literal, as an appropriate object of a class
        /// inheriting from <see cref="Literal"/>.</returns>
        /// <param name="v">The given value.</param>
        public override Literal CreateLiteral(object v)
        {
            return new IntLiteral( this.Machine, v );
        }
        
        /// <summary>
        /// Gets the associated basic type.
        /// Note that, for int *** ptr, returns int.
        /// Note that, for int &amp; ref, returns int.        
        /// </summary>
        /// <value>A <see cref="AType"/>.</value>
        public AType AssociatedType {
            get; protected set;
        }
    }
}
