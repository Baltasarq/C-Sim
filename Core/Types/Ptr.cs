using System;
using System.Linq;
using System.Collections.Generic;

using CSim.Core.Literals;

namespace CSim.Core.Types {
	/// <summary>
	/// Represents the pointer type.
	/// </summary>
	public class Ptr: Type {
		/// <summary>The ptr's type name part.</summary>
		public const string PtrTypeNamePart = "*";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Types.Ptr"/> class.
		/// </summary>
		/// <param name="n">The name of the type.</param>
		/// <param name="associatedType">The associated type.</param>
		/// <param name="wordSize">The word size of the machine.</param>
		internal Ptr(string n, Type associatedType, int wordSize)
			:base( n, wordSize )
		{
			this.AssociatedType = associatedType;
			this.IndirectionLevel = 1;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Types.Ptr"/> class.
		/// </summary>
		/// <param name="indirections">Number of indirections.</param>
		/// <param name="associatedType">Associated type.</param>
		/// <param name="wordSize">Word size.</param>
        internal Ptr(int indirections, Type associatedType, int wordSize)
			: base(
				associatedType.Name
					+ string.Concat( Enumerable.Repeat( PtrTypeNamePart, indirections ) ),
				wordSize )
        {
            this.AssociatedType = associatedType;
			this.IndirectionLevel = indirections;
        }

		/// <summary>
		/// Gets the associated basic type.
		/// </summary>
		/// <value>A <see cref="Type"/>.</value>
        public Type AssociatedType {
			get; private set;
        }

		/// <summary>
		/// Gets the type once a derreference is done.
		/// If <see cref="IndirectionLevel"/> == 1, then the
		/// result is the same as in <see cref="AssociatedType"/>.
		/// Otherwise, the result is a <see cref="Ptr"/>.
		/// </summary>
		/// <value>The type of the derreferenced.</value>
		public Type DerreferencedType {
			get {
				Type toret = this.AssociatedType;

				if ( this.IndirectionLevel > 1 ) {
					toret = new Ptr( this.IndirectionLevel - 1, this.AssociatedType, this.Size );
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
		/// Returns a <see cref="System.String"/> that represents the current <see cref="CSim.Core.Types.Ptr"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="CSim.Core.Types.Ptr"/>.</returns>
		public override string ToString()
		{
			return this.Name;
		}

		/// <summary>
		/// Creates a literal for this type, given a byte sequence.
		/// </summary>
		/// <returns>The literal, as an appropriate object of a class inheriting from Literal.</returns>
		/// <param name="raw">The sequence of bytes containing the value in memory.</param>
		/// <param name="m">M.</param>
		public override Literal CreateLiteral(Machine m, byte[] raw)
		{
			return new IntLiteral( m, m.Bytes.FromBytesToInt( raw ) );
		}
	}
}

