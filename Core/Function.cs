using System;
using System.Collections.ObjectModel;
using System.Text;

namespace CSim.Core {
	/// <summary>
	/// Represents functions which are callable.
	/// </summary>
	public abstract class Function {

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Function"/> class.
		/// Functions are defined by a return type, an indetifier, and a collection of
		/// formal parameters. Some functions don't accept any param.
		/// </summary>
		/// <param name="id">The identifier of the function, as a string.</param>
		/// <param name="returnType">The return type, as a CSim.Core.Type.</param>
		protected Function(Machine m, string id, CSim.Core.Type returnType)
			:this( m, id, returnType, null )
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
        protected Function(Machine m, string id, CSim.Core.Type returnType, Variable[] formalParams)
		{
            if ( id != null ) {
                id = id.Trim();
            }
			
            if ( string.IsNullOrEmpty( id ) ) {
				throw new ArgumentException( "void id in fn." );
			}

			if ( formalParams == null ) {
				formalParams = new Variable[]{};
			}

			this.machine = m;
			this.formalParams = formalParams;
			this.id = id;
			this.returnType = returnType;
		}

		/// <summary>
		/// Execute this <see cref="Function"/> with
		/// the specified parameters (<see cref="RValue"/>'s).
		/// </summary>
		/// <param name="realParams">The parameters.</param>
		public abstract void Execute(RValue[] realParams);

		/// <summary>
		/// Gets the identifier of the function to call.
		/// </summary>
		/// <value>The identifier, as a string.</value>
		public string Id {
			get {
				return this.id;
			}
		}

		/// <summary>
		/// Gets the parameter count.
		/// </summary>
		/// <value>The parameter count, as an int.</value>
		public int ParamCount {
			get { return this.formalParams.Length; }
		}

		/// <summary>
		/// Gets the formal parameters, as a ReadOnly collection.
		/// </summary>
		/// <value>The formal parameters.</value>
		public ReadOnlyCollection<Variable> FormalParams {
			get {
				return new ReadOnlyCollection<Variable>( this.formalParams );
			}
		}

		/// <summary>
		/// Gets the return type of the function.
		/// </summary>
		/// <value>The return type of the function, as a CSim.Core.Type object.</value>
		public CSim.Core.Type ReturnType {
			get {
				return this.returnType;
			}
		}

		/// <summary>
		/// Gets the machine this function pertains to.
		/// </summary>
		/// <value>The machine.</value>
		public Machine Machine {
			get {
				return this.machine;
			}
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="CSim.Core.Function"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="CSim.Core.Function"/>.</returns>
		public override string ToString()
		{
			var formalArgs = this.FormalParams;
			var strFormalArgs = new StringBuilder();

			// Build the list of formal arguments
			for (int i = 0; i < formalArgs.Count; ++i) {
				strFormalArgs.Append( formalArgs [i].ToString() );

				if ( i < formalArgs.Count - 1 ) {
					strFormalArgs.Append( ',' );
				}
			}

			return string.Format( "{0} {1}({2})",
			                     this.ReturnType.ToString(),
			                     this.Id,
			                     strFormalArgs.ToString()
			);
		}

		private Variable[] formalParams;
		private string id;
		private CSim.Core.Type returnType;
		private Machine machine;
	}
}

