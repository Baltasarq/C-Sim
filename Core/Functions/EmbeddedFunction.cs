namespace CSim.Core.Functions {

	/// <summary>
	/// Represents functions which are provided by the interpreter.
	/// </summary>
	public abstract class EmbeddedFunction: Function {

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Functions.EmbeddedFunction"/> class.
		/// Functions are defined by a return type, an indetifier, and a collection of
		/// formal parameters. Some functions don't accept any param.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/> this function will be executed in.</param>
		/// <param name="id">The identifier of the function, as a string.</param>
        /// <param name="returnType">The return type, as a <see cref="AType"/>.</param>
		public EmbeddedFunction(Machine m, string id, AType returnType)
			: base( m, id, returnType, null )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Function"/> class.
		/// Functions are defined by a return type, an indetifier, and a collection of
		/// formal parameters.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/> this function will be executed in.</param>
		/// <param name="id">The identifier of the function, as a string.</param>
        /// <param name="returnType">The return type, as a <see cref="AType"/>.</param>
		/// <param name="formalParams">The formal parameters, as a vector.</param>
		public EmbeddedFunction(Machine m, string id, AType returnType, Variable[] formalParams)
			: base( m, id, returnType, formalParams )
		{
		}
	}
}
