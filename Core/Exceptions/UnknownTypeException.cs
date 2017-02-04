
namespace CSim.Core.Exceptions {
	/// <summary>
	/// Unknown type exception.
	/// </summary>
	public class UnknownTypeException: EngineException {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Exceptions.UnknownTypeException"/> class.
		/// </summary>
		/// <param name="s">The message.</param>
		public UnknownTypeException(string s)
            : base( L18n.Get( L18n.Id.ExcUnknownType ) + ": " + s )
		{
		}
	}
}

