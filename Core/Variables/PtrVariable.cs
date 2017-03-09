
namespace CSim.Core.Variables {
    using System.Numerics;
    using CSim.Core.Types;
    using CSim.Core.Literals;

	/// <summary>Ptr <see cref="Variable"/>s.</summary>
	public class PtrVariable: Variable {

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Variables.PtrVariable"/> class.
		/// </summary>
		/// <param name="id">Identifier, as a string</param>
        /// <param name="t">The associated <see cref="AType"/> for this pointer.</param>
		/// <param name="indirectionLevel">The number of stars in the type.</param>
		public PtrVariable(Id id, AType t, int indirectionLevel = -1)
			: base( id, t.Machine.TypeSystem.GetPtrType( t, indirectionLevel ) )
		{
		}

		/// <summary>
		/// Gets the associated type. If this is "int *",
		/// then the answer is "int".
		/// </summary>
		/// <value>The associated <see cref="AType"/>.</value>
        public virtual AType AssociatedType {
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

		/// <summary>
		/// Gets or sets the value as an int.
		/// </summary>
		/// <value>The int value.</value>
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
		public virtual BigInteger Access {
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
