using System;

namespace CSim.Core.Exceptions
{
    public class ExhaustedMemoryException: EngineException {
        public ExhaustedMemoryException(string s)
            : base( L18n.Get( L18n.Id.ExcMemoryExhausted ) + ": " + s )
        {
        }
    }
}
