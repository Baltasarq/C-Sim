
namespace CSim.Core.Exceptions {
	/// <summary>
	/// Invalid max memory exception.
	/// </summary>
    public class InvalidMaxMemoryException: EngineException {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Exceptions.InvalidMaxMemoryException"/> class.
		/// </summary>
		/// <param name="s">The message.</param>
        public InvalidMaxMemoryException(string s)
            : base( L18n.Id.ExcInvalidMaxMemory + ": " + s )
        {
        }
    }
}

