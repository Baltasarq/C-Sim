using System;

namespace CSim.Core.Exceptions {
	/// <summary>
	/// Incorrect address exception.
	/// </summary>
    public class IncorrectAddressException: EngineException {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Exceptions.IncorrectAddressException"/> class.
		/// </summary>
		/// <param name="s">The message.</param>
        public IncorrectAddressException(string s)
            :base( L18n.Get( L18n.Id.ExcInvalidMemory ) + ": " + s )
        {
        }
    }
}

