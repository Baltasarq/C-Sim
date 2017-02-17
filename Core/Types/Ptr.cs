using CSim.Core.Variables;

namespace CSim.Core.Types {
    using System.Linq;
    using CSim.Core.Literals;

	/// <summary>
	/// Represents the pointer type.
	/// </summary>
	public class Ptr: AType {
		/// <summary>The ptr's type name part.</summary>
		public const string PtrTypeNamePart = "*";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Types.Ptr"/> class.
		/// </summary>
		/// <param name="n">The name of the type.</param>
		/// <param name="associatedType">The associated type.</param>
		internal Ptr(string n, AType associatedType)
			:base( associatedType.Machine, n, associatedType.Machine.WordSize )
		{
			this.AssociatedType = associatedType;
			this.IndirectionLevel = 1;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Types.Ptr"/> class.
		/// </summary>
		/// <param name="indirections">Number of indirections.</param>
		/// <param name="associatedType">Associated type.</param>
        internal Ptr(int indirections, AType associatedType)
			: this(
				associatedType.Name
					+ string.Concat( Enumerable.Repeat( PtrTypeNamePart, indirections ) ),
                associatedType )
        {
			this.IndirectionLevel = indirections;
        }

		/// <summary>
		/// Gets the associated basic type.
		/// </summary>
		/// <value>A <see cref="AType"/>.</value>
        public AType AssociatedType {
			get; private set;
        }

		/// <summary>
		/// Gets the type once a derreference is done.
		/// If <see cref="IndirectionLevel"/> == 1, then the
		/// result is the same as in <see cref="AssociatedType"/>.
		/// Otherwise, the result is a <see cref="Ptr"/>.
		/// </summary>
		/// <value>The type of the derreferenced.</value>
		public AType DerreferencedType {
			get {
				AType toret = this.AssociatedType;

				if ( this.IndirectionLevel > 1 ) {
					toret = new Ptr( this.IndirectionLevel - 1, this.AssociatedType );
				}

				return toret;
			}
		}

		/// <summary>
		/// Gets the level of indirection.
		/// </summary>
		/// <value>The indirection level, as an int.</value>
		public int IndirectionLevel {
			get; private set;
		}

		/// <summary>
		/// Creates a literal for this type, given a byte sequence.
		/// </summary>
		/// <returns>The literal, as an appropriate object of a class inheriting from Literal.</returns>
		/// <param name="raw">The sequence of bytes containing the value in memory.</param>
		public override Literal CreateLiteral(byte[] raw)
		{
			return new IntLiteral( this.Machine, this.Machine.Bytes.FromBytesToInt( raw ) );
		}

		/// <summary>
		/// Creates a corresponding variable.
		/// </summary>
		/// <returns>A <see cref="Variable"/> with this <see cref="AType"/>.</returns>
		/// <param name="strId">The identifier for the variable.</param>
		public override Variable CreateVariable(string strId)
		{
			return new PtrVariable( new Id( this.Machine, strId ), this, this.IndirectionLevel );
		}
        
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="CSim.Core.Types.Ptr"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="CSim.Core.Types.Ptr"/>.</returns>
        public override string ToString()
        {
            return this.Name;
        }
	}
}

