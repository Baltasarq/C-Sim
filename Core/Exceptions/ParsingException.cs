using System;

namespace CSim.Core.Exceptions {
	public class ParsingException: EngineException {
		public ParsingException(string s)
			: base( L18n.Get( L18n.Id.ExcParsing ) + ": " + s )
		{
		}
	}
}

