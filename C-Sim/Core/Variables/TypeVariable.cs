// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.Variables {
    /// <summary>Variables that can store types.</summary>
    public class TypeVariable: Variable {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:CSim.Core.Variables.TypeVariable"/> class.
        /// </summary>
        /// <param name="id">An <see cref="Id"/>.</param>
        public TypeVariable(Id id)
            :base( id, Types.TypeType.Get( id.Machine ) )
        {
        }
    }
}
