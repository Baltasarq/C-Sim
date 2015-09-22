using System;

namespace CSim.Core.Exceptions
{
	public class TypeMismatchException: EngineException
	{
		public TypeMismatchException(string msg)
            : base( L18n.Get( L18n.Id.ExcTypeMismatch ) + ": " + msg )
		{
		}
	}
}

