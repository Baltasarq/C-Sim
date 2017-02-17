
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
		public NoPlaceTempVariable(AType t)
			:base( t )
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="NoPlaceTempVariable"/> class,
        /// from a Literal object.
        /// </summary>
        /// <param name="v">The literal that will give its value to the <see cref="Variable"/>.</param>
		public NoPlaceTempVariable(Literal v)
            :base( v.Type )
        {
			this.litValue = v;
        }

        /// <summary>
        /// Gets or sets the value associated to this temp variable.
        /// Note that it does not have a place in memory.
        /// It is completely implemented to avoid a virtual call in ctor.
        /// </summary>
        /// <value>The value, as a literal object.</value>
		public override Literal LiteralValue {
            get {
                return this.litValue;
            }
            set {
                this.litValue = value;
            }
		}

        // This is used to avoid a virtual call in ctor.
        private Literal litValue;
	}
}

