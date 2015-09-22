using System;

using CSim.Core.Types;
using CSim.Core.Exceptions;

namespace CSim.Core.Opcodes
{
	public class Create: Opcode
	{
		public static char OpcodeValue = (char) 0xE2;

		protected delegate Variable CreateIt();

		public Create(Machine m, Id n, Type t)
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
				this.Type = ptrType.AssociatedType;
			}

			return;
		}

		public override Variable Execute()
		{
			return this.DoIt();
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
			return this.Machine.TDS.AddPtr( this.Name, this.Type );
		}

		public Id Name {
            get; set;
        }

		public Type Type {
			get; set;
		}

		private CreateIt DoIt;
	}
}

