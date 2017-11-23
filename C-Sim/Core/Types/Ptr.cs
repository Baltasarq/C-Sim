// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.Types {
    using System.Linq;
    
    using Variables;

	/// <summary>
	/// Represents the pointer type.
	/// </summary>
	public class Ptr: Indirection {
		/// <summary>The ptr's type name part.</summary>
		public const string PtrTypeNamePart = "*";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Types.Ptr"/> class.
		/// </summary>
		/// <param name="n">The name of the type.</param>
		/// <param name="associatedType">The associated type.</param>
		internal Ptr(string n, AType associatedType)
			:base( associatedType.Machine, n )
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
		/// Gets the type once a derreference is done.
		/// If <see cref="IndirectionLevel"/> == 1, then the
		/// result is the same as in <see cref="T:Indirection.AssociatedType"/>.
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
		/// Creates a corresponding variable.
		/// </summary>
		/// <returns>A <see cref="Variable"/> with this <see cref="AType"/>.</returns>
		/// <param name="strId">The identifier for the variable.</param>
		public override Variable CreateVariable(string strId)
		{
			return new PtrVariable(
                            new Id( this.Machine, strId ),
                            this,
                            this.IndirectionLevel );
		}
	}
}

