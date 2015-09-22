using System;

namespace CSim.Core.Exceptions
{
	public class UnknownVbleException: EngineException
	{
		public UnknownVbleException(string s)
            : base( L18n.Get( L18n.Id.ExcUnknownVble ) + ": " + s )
		{
		}
	}
}

