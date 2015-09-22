using System;

namespace CSim.Core.Exceptions
{
    public class InvalidIdException: EngineException {
        public InvalidIdException(string s)
            : base( L18n.Get( L18n.Id.ExcInvalidId ) + ": " + s )
        {
        }
    }
}

