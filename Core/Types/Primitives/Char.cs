using System;

using CSim.Core.Literals;

namespace CSim.Core.Types.Primitives
{
	/// <summary>
	/// Represents the Char type, for values from 0...255,
	/// it is the ISO-8859-1 code for characters.
	/// </summary>
    /// <seealso cref="Machine.TextEncoding"/>
    public class Char: Primitive
    {
		/// <summary>
		/// The name of the type.
		/// </summary>
        public const string TypeName = "char";
        
		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Types.Primitives.Char"/> class.
		/// It's size is guaranteed to be 1.
		/// </summary>
        internal Char(): base( TypeName, 1 )
        {
        }

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="CSim.Core.Types.Primitives.Char"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="CSim.Core.Types.Primitives.Char"/>.</returns>
		public override string ToString()
		{
			return this.Name;
		}

		public override Literal CreateLiteral(Machine m, byte[] raw)
		{
			return new CharLiteral( m, m.CnvtBytesToChar( raw ) );
		}
   }
}

