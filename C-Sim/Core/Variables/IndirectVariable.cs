// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.Variables {
    using Types;
    
    using System.Numerics;

    /// <summary>
    /// Variables like pointers or references.
    /// </summary>
    public abstract class IndirectVariable: Variable {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:CSim.Core.Variables.IndirectVariable"/> class.
        /// </summary>
        /// <param name="id">An Identifier.</param>
        /// <param name="t">A <see cref="AType"/>.</param>
        protected IndirectVariable(Id id, AType t)
            : base( id, t )
        {
            if ( !( t is Indirection ) ) {
                throw new Exceptions.RuntimeException(
                    "Indirect variable without indirect type" );
            }
        }
        
        /// <summary>
        /// Gets the type associated to this reference.
        /// </summary>
        /// <value>A <see cref="AType"/>.</value>
        public AType AssociatedType {
            get {
                return ( (Indirection) this.Type ).AssociatedType;
            }
        }
        
        /// <summary>
        /// Gets the address this indirection points to.
        /// </summary>
        /// <value>The pointed address, as a <see cref="BigInteger"/>.</value>
        public abstract BigInteger PointedAddress {
            get;
        }
    }
}
