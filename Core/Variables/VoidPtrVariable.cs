using System;

using CSim.Core.Types;
using CSim.Core.Exceptions;

namespace CSim.Core.Variables {
	/// <summary>
	/// Represents 'void *' pointers.
	/// </summary>
	public class VoidPtrVariable: PtrVariable {
		public VoidPtrVariable(Id id, Machine m)
			:base( id, Any.Get(), m )
		{
		}

		public VoidPtrVariable(Id id, Machine m, int address)
			: this( id, m )
		{
			this.Address = address;
		}

		public override CSim.Core.Type AssociatedType {
			get {
				return Any.Get();
			}
		}

		/// <summary>
		/// Access the memory pointed by the value of this variable.
		/// This is not possible for this type.
		/// </summary>
		/// <value>The memory read or written.</value>
		public override int Access {
			get {
				throw new TypeMismatchException( L18n.Get( L18n.Id.ErrDerreferencedVoid ) );
			}
			set {
				throw new TypeMismatchException( L18n.Get( L18n.Id.ErrDerreferencedVoid ) );
			}
		}
	}
}

