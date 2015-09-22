using System;

namespace CSim.Core.Exceptions
{
    public class InvalidMaxMemoryException: EngineException
    {
        public InvalidMaxMemoryException(string s)
            : base( L18n.Id.ExcInvalidMaxMemory + ": " + s )
        {
        }
    }
}

