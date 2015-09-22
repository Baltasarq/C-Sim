using System;

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
    }
}
    