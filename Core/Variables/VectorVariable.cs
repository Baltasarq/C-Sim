using System;

using CSim.Core;
using CSim.Core.Types;
using CSim.Core.Literals;

namespace CSim.Core.Variables
{
	public class VectorVariable: Variable {

		public VectorVariable(Id id, CSim.Core.Type t, Machine m, long count)
			: base( id, m.TypeSystem.GetPtrType( t ), m )
		{
			this.Count = count;
		}

        public virtual CSim.Core.Type AssociatedType {
			get {
				return ( (Ptr) this.Type ).AssociatedType;
			}
		}

        /// <summary>
        /// Gets or sets the values associated to this pointer,
        /// reading or writing in memory.
        /// A pointer's value is actually always an integer, same size as
        /// memory width.
        /// </summary>
        /// <value>The value, as an <see cref="IntLiteral"/>.</value>
        public new VectorLiteral LiteralValue {
            get {
                return (VectorLiteral)
						this.Memory.CreateLiteral( this.Address, this.Type );
            }
            set {
                this.Memory.Write( this.Address, value.GetRawValue() );
            }
        }

		/// <summary>
		/// Gets the size of the vector, in elements, not bytes.
		/// This size is totally dependent on the type.
		/// </summary>
		/// <value>The size, as an int.</value>
		public long Count {
			get; set;
		}
	}
}
