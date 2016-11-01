using System;
using System.Collections;
using System.Collections.Generic;

namespace CSim.Core.Types
{
    /// <summary>
    /// Represents references. References are a kind of pointer,
    /// that cannot be changed to point to another variable, and does not
    /// need special operators, such as * and &.
    /// </summary>
	public class Ref: Ptr
	{
		public const string RefTypeNamePart = "&";

        internal Ref(Type associatedType, int wordSize)
			: base( associatedType.Name + RefTypeNamePart, associatedType, wordSize )
        {
        }

		public override string ToString()
		{
			return this.Name;
		}
	}
}

