
namespace CSim.Core.Exceptions {
	/// <summary>
	/// Parsing exception.
	/// </summary>
	public class ParsingException: EngineException {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Exceptions.ParsingException"/> class.
		/// </summary>
		/// <param name="s">The message.</param>
		public ParsingException(string s)
			: base( L18n.Get( L18n.Id.ExcParsing ) + ": " + s )
		{
		}
	}
}

