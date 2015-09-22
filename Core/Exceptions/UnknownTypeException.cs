using System;

namespace CSim.Core.Exceptions
{
	public class UnknownTypeException: EngineException
	{
		public UnknownTypeException(string s)
            : base( L18n.Get( L18n.Id.ExcUnknownType ) + ": " + s )
		{
		}
	}
}

