
namespace CSim.Core {
	/// <summary>The engine's exception. This is the parent exception for the rest.</summary>
	public class EngineException: System.Exception {
		
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.EngineException"/> class.
		/// </summary>
		/// <param name="s">The message.</param>
		public EngineException(string s): base( s )
		{
		}
	}
}
