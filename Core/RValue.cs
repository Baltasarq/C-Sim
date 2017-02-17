
namespace CSim.Core {
    /// <summary>
    /// RValues can be types, id's,literals or variables.
    /// <seealso cref="AType"/><seealso cref="Literal"/><seealso cref="Variable"/><seealso cref="Id"/>
    /// </summary>
    public abstract class RValue {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:CSim.Core.RValue"/> class.
        /// </summary>
        /// <param name="m">The <see cref="Machine"/> this RValue will be evaluated for.</param>
        public RValue(Machine m)
        {
            this.Machine = m;
        }
    
        /// <summary>
        /// Gets the type of this <see cref="RValue"/>.
        /// </summary>
        /// <returns>The <see cref="AType"/>.</returns>
        public abstract AType Type {
            get;
        }

		/// <summary>
		/// Gets the value of this RValue.
		/// </summary>
		/// <value>The value, as an <see cref="System.Object"/>.</value>
		public abstract object Value {
			get;
		}

        /// <summary>The machine this RValue will be evaluated for.</summary>
        /// <value>The <see cref="Machine"/>.</value>
        public Machine Machine {
            get; private set;
        }

        /// <summary>
        /// Returns a <see cref="Variable"/> (<see cref="Variables.TempVariable"/> or not), with the same value.
        /// </summary>        
        public abstract Variable SolveToVariable();
    }
}
    