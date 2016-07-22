
namespace CSim {
	using System;
	using System.Collections.Generic;

	using CSim.Core;

	public class ExecutionStack {
		public ExecutionStack()
		{
			this.stack = new List<RValue>();
		}

		public void Push(RValue op) {
			this.stack.Add( op );
		}

		public RValue Peek() {
			RValue toret = null;

			if ( this.Count > 0 ) {
				toret = this.stack[ this.stack.Count - 1 ];
			}

			return toret;
		}

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

		public void Clear() {
			this.stack.Clear();
		}

		public int Count {
			get { return this.stack.Count; }
		}

		private List<RValue> stack;
	}
}

