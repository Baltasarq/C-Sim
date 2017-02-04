﻿
namespace CSim.Core {
    /// <summary>
    /// RValues can be literals or variables.
    /// </summary>
    public abstract class RValue {
        /// <summary>
        /// Gets the type of the rvalue.
        /// </summary>
        /// <value>The type.</value>
        public abstract CSim.Core.Type Type {
            get;
        }

		/// <summary>
		/// Gets the value of this RValue.
		/// </summary>
		/// <value>The value, as an <see cref="System.Object"/>.</value>
		public abstract object Value {
			get;
		}
    }
}
    