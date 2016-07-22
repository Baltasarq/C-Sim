using System;

using CSim.Core;
using CSim.Core.Types;
using CSim.Core.Literals;

namespace CSim.Core.Variables
{
	public class PtrVariable: Variable {

		public PtrVariable(Id id, CSim.Core.Type t, Machine m)
			: base( id, m.TypeSystem.GetPtrType( t ), m )
		{
		}

		public PtrVariable(Id id, CSim.Core.Type t, Machine m, int address)
            : this( id, t, m )
        {
            this.Address = address;
        }

        public virtual CSim.Core.Type AssociatedType {
			get {
				return ( (Ptr) this.Type ).AssociatedType;
			}
		}

        /// <summary>
        /// Gets or sets the value associated to this pointer,
        /// reading or writing in memory.
        /// A pointer's value is actually always an integer, same size as
        /// memory width.
        /// </summary>
        /// <value>The value, as an <see cref="IntLiteral"/>.</value>
        public new IntLiteral LiteralValue {
            get {
                return (IntLiteral)
						this.Memory.CreateLiteral( this.Address, this.Machine.TypeSystem.GetIntType() );
            }
            set {
                this.Memory.Write( this.Address, value.GetRawValue( this.Machine ) );
            }
        }

        /// <summary>
        /// Access the memory pointed by the value of this variable.
        /// </summary>
        /// <value>The memory read or written.</value>
		public virtual int Access {
			get {
				return this.Machine.CnvtBytesToInt(
					this.Memory.Read( this.LiteralValue.Value, this.AssociatedType.Size
				) );
			}
            set {
				byte[] intValue = this.Machine.CnvtIntToBytes( value );
				this.Memory.Write( this.LiteralValue.Value, intValue );
			}
		}
	}
}
