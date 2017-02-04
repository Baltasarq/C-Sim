
namespace CSim.Core.Variables {
	using CSim.Core.Types;
	using CSim.Core.Exceptions;

	/// <summary>
	/// Represents 'void *' pointers.
	/// </summary>
	public class VoidPtrVariable: PtrVariable {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Variables.VoidPtrVariable"/> class.
		/// </summary>
		/// <param name="id">An <see cref="Id"/> for this variable.</param>
		/// <param name="m">The <see cref="Machine"/> this variable will live in.</param>
		public VoidPtrVariable(Id id, Machine m)
			:base( id, Any.Get(), m )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Variables.VoidPtrVariable"/> class.
		/// </summary>
		/// <param name="id">An <see cref="Id"/> for this variable.</param>
		/// <param name="m">The <see cref="Machine"/> this variable will live in.</param>
		/// <param name="address">The address of the variable.</param>
		public VoidPtrVariable(Id id, Machine m, int address)
			: this( id, m )
		{
			this.Address = address;
		}

		/// <summary>
		/// Gets the associated type, which is <see cref="Any"/>.
		/// </summary>
		/// <value>The associated <see cref="CSim.Core.Type"/>.</value>
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
		public override long Access {
			get {
				throw new TypeMismatchException( L18n.Get( L18n.Id.ErrDerreferencedVoid ) );
			}
			set {
				throw new TypeMismatchException( L18n.Get( L18n.Id.ErrDerreferencedVoid ) );
			}
		}
	}
}

