namespace CSim.Core.Variables {
    using System.Numerics;
    using CSim.Core.Literals;

	/// <summary>Ptr <see cref="Variable"/>s.</summary>
	public class PtrVariable: IndirectVariable {

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
        /// Gets or sets the value associated to this pointer,
        /// reading or writing in memory.
        /// A pointer's value is actually always an integer, same size as
        /// memory width.
        /// </summary>
        /// <value>The value, as an <see cref="IntLiteral"/>.</value>
        public override Literal LiteralValue {
            get {
                Literal toret;
                
                if ( this.IsTemp() ) {
                    toret = base.LiteralValue;
                } else {
                    toret = this.Memory.CreateLiteral(
                                        this.Address,
                                        this.Machine.TypeSystem.GetIntType() );
                }
                
                return toret;
            }
            set {
                if ( this.IsTemp() ) {
                    base.LiteralValue = value;
                } else {
                    this.Memory.Write( this.Address, value.GetRawValue() );
                }
                
                return;
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
        /// Gets the address this pointer points to.
        /// </summary>
        /// <value>The pointed address, as a <see cref="BigInteger"/>.</value>
        public override BigInteger PointedAddress {
            get {
                return this.IntValue.ToBigInteger();
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
