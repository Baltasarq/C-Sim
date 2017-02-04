
namespace CSim.Core.Opcodes {
	using CSim.Core.Types;
	using CSim.Core.Variables;

	/// <summary>
	/// Create opcode. Allows to create different kinds of variables.
	/// </summary>
	public class CreateOpcode: Opcode {
		/// <summary>The opcode's representing value.</summary>
		public static char OpcodeValue = (char) 0xE3;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Opcodes.CreateOpcode"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/> this opcode will be executed in.</param>
		/// <param name="n">The name of the <see cref="Variable"/>.</param>
		/// <param name="t">The <see cref="CSim.Core.Type"/> of the new <see cref="Variable"/>.
		/// It can be complex (i.e., a <see cref="CSim.Core.Types.Ref"/>
		/// or a <see cref="CSim.Core.Types.Ptr"/></param>
		public CreateOpcode(Machine m, Id n, Core.Type t)
            : base( m )
		{
			// Assign simple properties
			this.Name = n;
			this.Type = t;

			// Decide the kind of variable to create
			var ptrType = t as Ptr;
			var refType = t as Ref;

			this.DoIt = this.CreateVble;

			if ( refType != null ) {
				this.DoIt = this.CreateRef;
				this.Type = refType.AssociatedType;
			}
			else
			if ( ptrType != null ) {
				this.DoIt = this.CreatePtr;
				this.Type = ptrType;
			}

			return;
		}

		/// <summary>
		/// Execute the opcode.
		/// </summary>
		public override void Execute()
		{
			this.Machine.ExecutionStack.Push( this.DoIt() );
		}

		/// <summary>
		/// Create the differet kinds of variable.
		/// </summary>
		protected delegate Variable CreateIt();

		/// <summary>
		/// Creates a vble.
		/// </summary>
		/// <seealso cref="Name"/><seealso cref="Type"/>
		/// <returns>The vble.</returns>
		protected Variable CreateVble()
		{
			return this.Machine.TDS.Add( this.Name, this.Type );
		}

		/// <summary>
		/// Creates a ref.
		/// </summary>
		/// <seealso cref="Name"/><seealso cref="Type"/>
		/// <returns>The vble.</returns>
		protected Variable CreateRef()
		{
			return this.Machine.TDS.AddRef( this.Name, this.Type );
		}

		/// <summary>
		/// Creates a pointer.
		/// </summary>
		/// <seealso cref="Name"/><seealso cref="Type"/>
		/// <returns>The vble.</returns>
		protected Variable CreatePtr()
		{
			return this.Machine.TDS.Add(
				new PtrVariable( this.Name, this.Type, this.Machine, -1 ) );
		}

		/// <summary>
		/// Gets or sets the name of the new <see cref="Variable"/>
		/// </summary>
		/// <value>The name.</value>
		public Id Name {
            get; set;
        }

		/// <summary>
		/// Gets or sets the type of the new <see cref="Variable"/>
		/// </summary>
		/// <value>The type, as a <see cref="CSim.Core.Type"/>.</value>
		public Core.Type Type {
			get; set;
		}

		private CreateIt DoIt;
	}
}
