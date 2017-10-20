
namespace CSim.Core.Variables {
	using CSim.Core.Literals;
    
    using System.Numerics;

	/// <summary>A reference <see cref="Variable"/>.</summary>
    public class RefVariable: IndirectVariable {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Variables.RefVariable"/> class.
		/// </summary>
		/// <param name="id">The <see cref="Id"/> for this reference.</param>
        /// <param name="t">The associated <see cref="AType"/> for this reference.</param>
        public RefVariable(Id id, AType t)
            : base( id, t.Machine.TypeSystem.GetRefType( t ) )
        {
            this.pointedVble = null;
        }

		/// <summary>
		/// Has the reference been set?
		/// </summary>
		/// <returns><c>true</c>, if it was set, <c>false</c> otherwise.</returns>
        public bool IsSet()
        {
            return ( this.pointedVble != null );
        }
        
		/// <summary>
		/// Gets or sets the pointed vble,
		/// honoring the value of this reference.
		/// </summary>
		/// <value>The pointed vble.</value>
        public Variable PointedVble {
            get { 
                if ( this.pointedVble != null ) {
                    return this.pointedVble;
                } else {
                    throw new EngineException( L18n.Get( L18n.Id.ErrRefNotSet ) );
                }
            }
            set {
                if ( this.pointedVble == null ) {
                    this.pointedVble = ReachRealVariable( value );
                    base.LiteralValue = new IntLiteral( this.Machine,
                                                        this.PointedVble.Address );
                } else {
                    throw new EngineException( L18n.Get( L18n.Id.ErrRefDoubleSet ) );
                }
            }
        }
        
        /// <summary>
        /// Reachs the real variable, in case of referencing a reference.
        /// </summary>
        /// <returns>The real <see cref="Variable"/>.</returns>
        /// <param name="vble">The given <see cref="Variable"/>,
        /// maybe a reference.</param>
        private Variable ReachRealVariable(Variable vble)
        {
            while( vble is RefVariable refVble ) {
                if ( !refVble.IsSet() ) {
                    throw new EngineException(
                                            L18n.Get( L18n.Id.ErrRefNotSet )
                                            + ": " + refVble.Name.Name );
                }
                
                vble = refVble.PointedVble;
            }
            
            return vble;
        }
        
        /// <summary>
        /// Gets the address this pointer points to.
        /// </summary>
        /// <value>The pointed address, as a <see cref="BigInteger"/>.</value>
        public override BigInteger PointedAddress {
            get {
                return this.PointedVble.Address;
            }
        }
        
        /// <summary>
        /// Gets or sets the value associated to this variable,
        /// reading or writing in memory.
        /// </summary>
        /// <value>The value, as a literal.</value>
        public override Literal LiteralValue {
            get {
                return this.PointedVble.LiteralValue;
            }
            set {
                this.PointedVble.LiteralValue = value;
            }
        }
        
        /// <summary>
        /// Solves the type to a variable with the <see cref="Literals.TypeLiteral"/> as value.
        /// </summary>
        /// <returns>A suitable <see cref="Variable"/>.</returns>
        public override Variable SolveToVariable()
        {
            return this.PointedVble;
        }

        private Variable pointedVble;
    }
}
