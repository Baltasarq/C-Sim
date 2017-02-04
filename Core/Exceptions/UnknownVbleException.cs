
namespace CSim.Core.Exceptions {
	/// <summary>
	/// Unknown vble exception.
	/// </summary>
	public class UnknownVbleException: EngineException {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Exceptions.UnknownVbleException"/> class.
		/// </summary>
		/// <param name="s">The message.</param>
		public UnknownVbleException(string s)
            : base( L18n.Get( L18n.Id.ExcUnknownVble ) + ": " + s )
		{
		}
	}
}
