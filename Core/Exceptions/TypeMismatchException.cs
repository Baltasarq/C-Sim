using System;

namespace CSim.Core.Exceptions {
	/// <summary>
	/// Type mismatch exceptions.
	/// </summary>
	public class TypeMismatchException: EngineException {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Exceptions.TypeMismatchException"/> class.
		/// </summary>
		/// <param name="msg">Message.</param>
		public TypeMismatchException(string msg)
            : base( L18n.Get( L18n.Id.ExcTypeMismatch ) + ": " + msg )
		{
		}
	}
}
