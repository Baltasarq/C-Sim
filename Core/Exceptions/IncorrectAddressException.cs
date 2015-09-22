using System;

namespace CSim.Core.Exceptions {
    public class IncorrectAddressException: EngineException {
        public IncorrectAddressException(string s)
            :base( L18n.Get( L18n.Id.ExcInvalidMemory ) + ": " + s )
        {
        }
    }
}

