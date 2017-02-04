
namespace CSim.Core {
	using System.Collections.Generic;

	/// <summary>
	/// The stack to use while executing <see cref="Opcode"/>'s in a <see cref="Machine"/>.
	/// </summary>
	public class ExecutionStack {
		/// <summary>
		/// Initializes a new instance of the <see cref="ExecutionStack"/> class.
		/// </summary>
		public ExecutionStack()
		{
			this.stack = new List<RValue>();
		}

		/// <summary>
		/// Adds an <see cref="RValue"/> to the stack.
		/// </summary>
		/// <param name="op">The <see cref="RValue"/> to add.</param>
		public void Push(RValue op) {
			this.stack.Add( op );
		}

		/// <summary>
		/// Returns the <see cref="RValue"/> at the top of the stack.
		/// </summary>
		public RValue Peek() {
			RValue toret = null;

			if ( this.Count > 0 ) {
				toret = this.stack[ this.stack.Count - 1 ];
			}

			return toret;
		}

		/// <summary>
		/// Pops the <see cref="RValue"/> at the top of the stack.
		/// It also returns it.
		/// </summary>
		/// <returns>
		/// The <see cref="RValue"/> at the top of the stack.
		/// </returns>
		public RValue Pop() {
			RValue toret = null;

			if ( this.Count > 0 ) {
				toret = this.Peek();
				this.stack.RemoveAt( this.stack.Count - 1 );
			} else {
				throw new EngineException( "empty stack: cannot pop" );
			}

			return toret;
		}

		/// <summary>
		/// Deletes all entries in the stack.
		/// </summary>
		public void Clear() {
			this.stack.Clear();
		}

		/// <summary>
		/// Gets the count of elements in the stack.
		/// </summary>
		/// <value>The count.</value>
		public int Count {
			get { return this.stack.Count; }
		}

		private List<RValue> stack;
	}
}
