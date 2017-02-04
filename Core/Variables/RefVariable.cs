
namespace CSim.Core.Variables {
	using CSim.Core.Literals;

	/// <summary>A reference <see cref="Variable"/>.</summary>
    public class RefVariable: PtrVariable {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Variables.RefVariable"/> class.
		/// </summary>
		/// <param name="id">The <see cref="Id"/> (identifier).</param>
		/// <param name="t">The <see cref="Type"/>.</param>
		/// <param name="m">The <see cref="Machine"/> this variable will live in.</param>
		/// <param name="address">The address of this variable.</param>
        public RefVariable(Id id, CSim.Core.Type t, Machine m, long address)
            : this( id, t, m )
        {
            this.Address = address;
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Variables.RefVariable"/> class.
		/// </summary>
		/// <param name="id">The <see cref="Id"/> for this reference.</param>
		/// <param name="t">The associated type of the reference.</param>
		/// <param name="m">The <see cref="Machine"/> this reference will live in.</param>
        public RefVariable(Id id, CSim.Core.Type t, Machine m)
            : base( id, t, m )
        {
            this.SetType( m.TypeSystem.GetRefType( t ) );
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
                    this.pointedVble = value;
					this.LiteralValue = new IntLiteral( this.Machine, value.Address );
                } else {
                    throw new EngineException( L18n.Get( L18n.Id.ErrRefDoubleSet ) );
                }
            }
        }

		/// <summary>
		/// Gets the type of the referenced variable,
		/// not the type of the refernce itself.
		/// I.e.. it is aware of references.
		/// (accessing a reference is the same as accessing the target)
		/// </summary>
		/// <returns>The type of the associated type.</returns>
		public override Type GetTargetType()
		{
			return this.AssociatedType;
		}

        private Variable pointedVble;
    }
}
