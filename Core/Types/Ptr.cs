using System;
using System.Collections;
using System.Collections.Generic;

using CSim.Core.Literals;

namespace CSim.Core.Types
{
	public class Ptr: Type
	{
		public const string PtrTypeNamePart = "*";

        internal Ptr(string n, Type associatedType, int wordSize)
            : base( n, wordSize )
        {
            this.associatedType = associatedType;
        }

        public Type AssociatedType {
            get { return this.associatedType; }
        }

		public override string ToString()
		{
			return this.Name;
		}

		public override Literal CreateLiteral(Machine m, byte[] raw)
		{
			return new IntLiteral( m, m.CnvtBytesToInt( raw ) );
		}

        private Type associatedType;
	}
}
