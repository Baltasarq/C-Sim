using System;

using CSim.Core;
using CSim.Core.Types;
using CSim.Core.Literals;

namespace CSim.Core.Variables
{
    public class RefVariable: PtrVariable
    {
        public RefVariable(Id id, CSim.Core.Type t, Machine m, int address)
            : this( id, t, m )
        {
            this.Address = address;
        }

        public RefVariable(Id id, CSim.Core.Type t, Machine m)
            : base( id, t, m )
        {
            this.SetType( m.TypeSystem.GetRefType( t ) );
            this.pointedVble = null;
        }

        public bool IsSet()
        {
            return ( this.pointedVble != null );
        }

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
