using System;

namespace CSim.Core.Variables {
	/// <summary>
	/// Temporary variables, which hold a Literal of any kind,
	/// which is not stored in any memory.
	/// </summary>
	public class NoPlaceTempVariable: TempVariable {
        /// <summary>
        /// Creates a new temporal variable, given a type.
        /// </summary>
        /// <param name="t">The Type object t from which to create the temp.</param>
		public NoPlaceTempVariable(Type t)
			:base( t, null )
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="CSim.Core.Variables.TempVariable"/> class,
        /// from a Literal object.
        /// </summary>
        /// <param name="v">V.</param>
		public NoPlaceTempVariable(Literal v)
			:base( v.Type, v.Machine )
        {
			this.LiteralValue = v;
        }

        /// <summary>
        /// Gets or sets the value associated to this temp variable.
        /// Note that it does not have a place in memory.
        /// </summary>
        /// <value>The value, as a literal object.</value>
		public override Literal LiteralValue {
			get; set;
		}
	}
}

