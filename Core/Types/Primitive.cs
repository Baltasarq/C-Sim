
namespace CSim.Core.Types {
    /// <summary>
    /// Represents primitive types
    /// </summary>
    public abstract class Primitive: Type {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Types.Primitive"/> class.
		/// </summary>
		/// <param name="name">The name of the type.</param>
		/// <param name="size">The size of the type.</param>
        protected Primitive(string name, int size): base( name, size )
        {
        }
    }
}

