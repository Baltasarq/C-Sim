using System;

namespace CSim.Core.Variables {
	/// <summary>
	/// Temporary variables, which hold a Literal of any kind,
	/// which is not stored in any memory.
	/// </summary>
	public class TempVariable: Variable {
		public const string EtqTempVariable = "_aux__";

        /// <summary>
        /// Creates a new temporal variable, given a type.
        /// </summary>
        /// <param name="t">The Type object t from which to create the temp.</param>
		public TempVariable(Type t)
			:base( new Id( "aux" ), t, null )
		{
			this.SetNameWithoutChecking( EtqTempVariable );
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="CSim.Core.Variables.TempVariable"/> class,
        /// from a Literal object.
        /// </summary>
        /// <param name="v">V.</param>
		public TempVariable(Literal v)
            :base( new Id( "aux" ), v.Type, null )
        {
            this.SetNameWithoutChecking( EtqTempVariable );
			this.value = v;
        }

        /// <summary>
        /// Gets or sets the value associated to this temp variable.
        /// Note that it does not have a place in memory.
        /// </summary>
        /// <value>The value, as a Literla object.</value>
		public override Literal LiteralValue {
            get {
                return this.value;
            }
            set {
                this.value = value;
            }
		}

        private Literal value;
	}
}

