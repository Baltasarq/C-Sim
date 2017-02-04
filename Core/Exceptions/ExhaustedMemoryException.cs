
namespace CSim.Core.Exceptions {
	/// <summary>
	/// Exhausted memory exception.
	/// </summary>
    public class ExhaustedMemoryException: EngineException {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Exceptions.ExhaustedMemoryException"/> class.
		/// </summary>
		/// <param name="s">The message.</param>
        public ExhaustedMemoryException(string s)
            : base( L18n.Get( L18n.Id.ExcMemoryExhausted ) + ": " + s )
        {
        }
    }
}
