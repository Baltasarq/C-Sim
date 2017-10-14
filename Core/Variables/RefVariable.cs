
namespace CSim.Core.Variables {
	using CSim.Core.Literals;
    using CSim.Core.Types;

	/// <summary>A reference <see cref="Variable"/>.</summary>
    public class RefVariable: Variable {
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
        /// Gets the type associated to this reference.
        /// </summary>
        /// <value>A <see cref="AType"/>.</value>
        public AType AssociatedType {
            get {
                return ( (Ref) this.Type ).AssociatedType;
            }
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
                    this.pointedVble = value;
					this.LiteralValue = new IntLiteral( this.Machine, value.Address );
                } else {
                    throw new EngineException( L18n.Get( L18n.Id.ErrRefDoubleSet ) );
                }
            }
        }

        private Variable pointedVble;
    }
}
