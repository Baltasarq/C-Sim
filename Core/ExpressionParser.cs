
namespace CSim.Core {
	/// <summary>
	/// Expression parser. It parser RValues
	/// </summary>
	public class ExpressionParser: Parser {
		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.ExpressionParser"/> class.
		/// </summary>
		/// <param name="input">The expression to parse.</param>
		/// <param name="m">The machine for which to parse the expression.</param>
		public ExpressionParser(string input, Machine m)
			:base( input, m )
		{
		}

		/// <summary>
		/// Parsing starts here, but just for expressions.
		/// </summary>
		public override Opcode[] Parse()
		{
			this.ParseExpression();
			return this.Opcodes.ToArray();
		}
	}
}
