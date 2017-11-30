// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.Variables {
    using System.Numerics;
	using Types;
	using Exceptions;

	/// <summary>
	/// Represents 'void *' pointers.
	/// </summary>
	public class VoidPtrVariable: PtrVariable {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Variables.VoidPtrVariable"/> class.
		/// </summary>
		/// <param name="id">An <see cref="Id"/> for this variable.</param>
		public VoidPtrVariable(Id id)
            :base( id, Any.Get( id.Machine ) )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Variables.VoidPtrVariable"/> class.
		/// </summary>
		/// <param name="id">An <see cref="Id"/> for this variable.</param>
		/// <param name="address">The address of the variable.</param>
		public VoidPtrVariable(Id id, int address)
			: this( id )
		{
			this.Address = address;
		}

		/// <summary>
		/// Access the memory pointed by the value of this variable.
		/// This is not possible for this type.
		/// </summary>
		/// <value>The memory read or written.</value>
		public override BigInteger Access {
			get {
				throw new TypeMismatchException(
                                    L10n.Get( L10n.Id.ErrDerreferencedVoid ) );
			}
			set {
				throw new TypeMismatchException(
                                    L10n.Get( L10n.Id.ErrDerreferencedVoid ) );
			}
		}
	}
}

