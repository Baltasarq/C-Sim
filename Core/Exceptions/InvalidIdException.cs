
namespace CSim.Core.Exceptions {
	/// <summary>
	/// Invalid identifier exception.
	/// </summary>
    public class InvalidIdException: EngineException {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Exceptions.InvalidIdException"/> class.
		/// </summary>
		/// <param name="s">The message.</param>
        public InvalidIdException(string s)
            : base( L18n.Get( L18n.Id.ExcInvalidId ) + ": " + s )
        {
        }
    }
}

