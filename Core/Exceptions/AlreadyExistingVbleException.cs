
namespace CSim.Core.Exceptions {
	/// <summary>
	/// Already existing vble exception.
	/// </summary>
	public class AlreadyExistingVbleException: EngineException {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Exceptions.AlreadyExistingVbleException"/> class.
		/// </summary>
		/// <param name="s">The message.</param>
        public AlreadyExistingVbleException(string s)
            : base( L18n.Get( L18n.Id.ExcDuplicatedVble ) + ": " + s )
        {
        }
    }
}

