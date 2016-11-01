using System;

using CSim.Core;
using CSim.Core.Types;
using CSim.Core.Literals;

namespace CSim.Core.Variables
{
	public class PtrVariable: Variable {

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Variables.PtrVariable"/> class.
		/// </summary>
		/// <param name="id">Identifier, as a string</param>
		/// <param name="t">The type. This type must include the pointer part,
		///  				not just the primitive.</param>
		/// <param name="m">The machine this variable will live on.</param>
		public PtrVariable(Id id, CSim.Core.Type t, Machine m)
			: base( id, t, m )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Variables.PtrVariable"/> class.
		/// </summary>
		/// <param name="id">Identifier, as a string</param>
		/// <param name="t">The type. This type must include the pointer part,
		///  				not just the primitive.</param>
		/// <param name="m">The machine this variable will live on.</param>
		/// <param name="address">The address this variable is placed on memory.</param>
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
        public override Literal LiteralValue {
            get {
                return (IntLiteral)
						this.Memory.CreateLiteral( this.Address, this.Machine.TypeSystem.GetIntType() );
            }
            set {
                this.Memory.Write( this.Address, value.GetRawValue() );
            }
        }

		public IntLiteral IntValue {
			get {
				return (IntLiteral) this.LiteralValue;
			}
			set {
				this.LiteralValue = value;
			}
		}

        /// <summary>
        /// Access the memory pointed by the value of this variable.
        /// </summary>
        /// <value>The memory read or written.</value>
		public virtual long Access {
			get {
				return this.Machine.Bytes.FromBytesToInt(
					this.Memory.Read( this.IntValue.Value, this.AssociatedType.Size
				) );
			}
            set {
				byte[] intValue = this.Machine.Bytes.FromIntToBytes( value );
				this.Memory.Write( this.IntValue.Value, intValue );
			}
		}
	}
}
