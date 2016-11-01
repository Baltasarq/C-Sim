
namespace CSim.Core.Opcodes {
	using System;

	using CSim.Core.Types;
	using CSim.Core.Variables;
	using CSim.Core.Exceptions;

	public class CreateOpcode: Opcode
	{
		public static char OpcodeValue = (char) 0xE3;

		protected delegate Variable CreateIt();

		public CreateOpcode(Machine m, Id n, Core.Type t)
            : base( m )
		{
			var ptrType = t as Ptr;
			var refType = t as Ref;

			this.Name = n;
			this.Type = t;
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

		public override void Execute()
		{
			this.Machine.ExecutionStack.Push( this.DoIt() );
		}

		protected Variable CreateVble()
		{
			return this.Machine.TDS.Add( this.Name, this.Type );
		}

		protected Variable CreateRef()
		{
			return this.Machine.TDS.AddRef( this.Name, this.Type );
		}

		protected Variable CreatePtr()
		{
			return this.Machine.TDS.Add(
				new PtrVariable( this.Name, this.Type, this.Machine, -1 ) );
		}

		public Id Name {
            get; set;
        }

		public Core.Type Type {
			get; set;
		}

		private CreateIt DoIt;
	}
}
