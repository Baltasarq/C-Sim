using System;

using CSim.Core;
using CSim.Core.Types;
using CSim.Core.Literals;

namespace CSim.Core.Variables
{
	public class VectorVariable: Variable {

		public VectorVariable(Id id, CSim.Core.Type t, Machine m, int count)
			: base( id, m.TypeSystem.GetVectorType( t ), m )
		{
			this.Count = count;
		}

        public virtual CSim.Core.Type AssociatedType {
			get {
				return ( (Vector) this.Type ).AssociatedType;
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
                this.Memory.Write( this.Address, value.GetRawValue( this.Machine ) );
            }
        }

		/// <summary>
		/// Gets the size of the vector, in elements, not bytes.
		/// This size is totally dependent on the type.
		/// </summary>
		/// <value>The size, as an int.</value>
		public int Count {
			get; set;
		}
	}
}
