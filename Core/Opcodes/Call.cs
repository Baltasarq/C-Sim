using System;
using System.Collections.ObjectModel;

using CSim.Core;
using CSim.Core.FunctionLibrary;

namespace CSim.Core.Opcodes {

	/// <summary>
	/// Represents function calls.
	/// </summary>
	public class Call: Opcode {

		/// <summary>
		/// Some function calls are performed to a function without
		/// parameters.
		/// </summary>
		/// <param name="id">The identifier of the function to call, as a string.</param>
		public Call(Machine m, string id): this( m, id, null )
		{
		}

		/// <summary>
		/// Function calls are defined by an id
		/// and a list of typed parameters.
		/// These are represented by local variables.
		/// </summary>
		/// <param name="id">The identifier of the function to call, as a string.</param>
        /// <param name="realParams">The parameters to pass to the function to call, as a vector.</param>
		public Call(Machine m, string id, RValue[] realParams)
            : base( m )
		{
            if ( id != null ) {
                id = id.Trim();
            }

            if ( string.IsNullOrEmpty( id ) ) {
				throw new ArgumentException( "void id in fn. call" );
			}

			if ( realParams == null ) {
				realParams = new RValue[0];
			}

			this.realParams = new ReadOnlyCollection<RValue>( realParams );
			this.id = id;
		}

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
		/// Gets the formal parameters, as a ReadOnly collection.
		/// </summary>
		/// <value>The formal parameters.</value>
		public ReadOnlyCollection<RValue> RealParams {
			get {
				return this.realParams;
			}
		}

		/// <summary>
		/// Executes the function call.
		/// </summary>
		public override Variable Execute()
		{
			return this.Machine.Api.Match( this.Id, this.RealParams ).Execute( this.RealParams );
		}

		private ReadOnlyCollection<RValue> realParams;
		private string id;
	}
}

