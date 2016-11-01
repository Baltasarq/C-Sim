using System;
using System.Linq;
using System.Collections.Generic;

using CSim.Core.Literals;

namespace CSim.Core.Types
{
	public class Ptr: Type
	{
		public const string PtrTypeNamePart = "*";

		internal Ptr(string n, Type associatedType, int wordSize)
			:base( n, wordSize )
		{
			this.AssociatedType = associatedType;
			this.IndirectionLevel = 1;
		}

        internal Ptr(int indirections, Type associatedType, int wordSize)
			: base(
				associatedType.Name
					+ string.Concat( Enumerable.Repeat( PtrTypeNamePart, indirections ) ),
				wordSize )
        {
            this.AssociatedType = associatedType;
			this.IndirectionLevel = indirections;
        }

        public Type AssociatedType {
			get; private set;
        }

		public int IndirectionLevel {
			get; private set;
		}

		public override string ToString()
		{
			return this.Name;
		}

		public override Literal CreateLiteral(Machine m, byte[] raw)
		{
			return new IntLiteral( m, m.Bytes.FromBytesToInt( raw ) );
		}
	}
}

