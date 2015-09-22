using System;
using System.Collections.ObjectModel;

using CSim.Core.Variables;

namespace CSim.Core.Functions {

	/// <summary>
	/// Represents functions which are programmed by the user.
	/// </summary>
	public abstract class UserFunction: Function {

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.EmbeddedFunction"/> class.
		/// Functions are defined by a return type, an indetifier, and a collection of
		/// formal parameters. Some functions don't accept any param.
		/// </summary>
		/// <param name="id">The identifier of the function, as a string.</param>
		/// <param name="returnType">The return type, as a CSim.Core.Type.</param>
		/// <param name="formalParams">The formal parameters, as a vector.</param>
		public UserFunction(Machine m, string id, CSim.Core.Type returnType)
			: base( m, id, returnType, null )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Function"/> class.
		/// Functions are defined by a return type, an indetifier, and a collection of
		/// formal parameters.
		/// </summary>
		/// <param name="id">The identifier of the function, as a string.</param>
		/// <param name="returnType">The return type, as a CSim.Core.Type.</param>
		/// <param name="formalParams">The formal parameters, as a vector.</param>
		public UserFunction(Machine m, string id, CSim.Core.Type returnType, Variable[] formalParams)
			: base( m, id, returnType, formalParams )
		{
		}

	}
}
