using System;

namespace CSim.Core.Exceptions {
	public class AlreadyExistingVbleException: EngineException {
        public AlreadyExistingVbleException(string s)
            : base( L18n.Get( L18n.Id.ExcDuplicatedVble ) + ": " + s )
        {
        }
    }
}

