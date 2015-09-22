using System;

namespace CSim.Core.Types {
    /// <summary>
    /// Represents primitive types
    /// </summary>
    public abstract class Primitive: Type {
        public Primitive(string name, int size): base( name, size )
        {
        }
    }
}

