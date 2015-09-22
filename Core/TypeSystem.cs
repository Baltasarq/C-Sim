using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using CSim.Core.Types;
using CSim.Core.Types.Primitives;

namespace CSim.Core {
	/// <summary>
	/// Represents the type system.
	/// All types are retrieved through an object of this
	/// class, since types are dependent on the architecture (i.e., size).
	/// </summary>
	public class TypeSystem {
		public TypeSystem(Machine m)
		{
			this.machine = m;
			this.ptrTypeInstances = new Dictionary<Type, Ptr>();
			this.refTypeInstances = new Dictionary<Type, Ref>();
			this.vectorTypeInstances = new Dictionary<Type, Vector>();
			this.primitiveTypeInstances = new Dictionary<string, Type>();

			this.Reset();
		}

		/// <summary>
		/// Resets the type system.
		/// This is particularly useful when the wordsize changes.
		/// </summary>
		public void Reset()
		{
			this.ptrTypeInstances.Clear();
			this.refTypeInstances.Clear();
			this.vectorTypeInstances.Clear();
			this.primitiveTypeInstances.Clear();

			this.primitiveTypeInstances.Add( CSim.Core.Types.Primitives.Char.TypeName,
			                                new CSim.Core.Types.Primitives.Char() );
			this.primitiveTypeInstances.Add( CSim.Core.Types.Primitives.Int.TypeName,
			                                new CSim.Core.Types.Primitives.Int( this.Machine.WordSize ) );
			this.primitiveTypeInstances.Add( CSim.Core.Types.Primitives.Double.TypeName, 
			                                new CSim.Core.Types.Primitives.Double( this.Machine.WordSize ) );
		}

		/// <summary>
		/// Gets the int type.
		/// </summary>
		/// <returns>The int type, as a <see cref="Int"/> object.</returns>
		public Int GetIntType()
		{
			return (Int) this.GetPrimitiveType( Int.TypeName );
		}

		/// <summary>
		/// Gets the double type.
		/// </summary>
		/// <returns>The double type, as a <see cref="CSim.Core.Types.Primitives.Double"/> object.</returns>
		public CSim.Core.Types.Primitives.Double GetDoubleType()
		{
			return (CSim.Core.Types.Primitives.Double)
				this.GetPrimitiveType( CSim.Core.Types.Primitives.Double.TypeName );
		}

		/// <summary>
		/// Gets the char type.
		/// </summary>
		/// <returns>The char type, as a <see cref="CSim.Core.Types.Primitives.Char"/> object.</returns>
		public CSim.Core.Types.Primitives.Char GetCharType()
		{
			return (CSim.Core.Types.Primitives.Char)
				this.GetPrimitiveType( CSim.Core.Types.Primitives.Char.TypeName );
		}

		/// <summary>
		/// Gets any type supported by this machine, given its name.
		/// </summary>
		/// <returns>The type, as a <see cref="Type"/> object.</returns>
		/// <param name="strType">A string with the type's name.</param>
		public Type GetPrimitiveType(string strType)
		{
			return this.primitiveTypeInstances[ strType ];
		}

		/// <summary>
		/// Determines whether a given string denotes a type, using reflection to run
		/// all over subclasses of Type.
		/// </summary>
		/// <returns>
		/// <c>true</c> if this instance denotes a type; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='typeName'>
		/// A string possibliy containing a type name.
		/// </param>
		public bool IsPrimitiveType(string typeName)
		{
			return this.primitiveTypeInstances.ContainsKey( typeName );
		}

		/// <summary>
		/// Returns a collection of identifiers for all primitive types.
		/// </summary>
		/// <returns>The primitive types.</returns>
		public ReadOnlyCollection<string> GetPrimitiveTypes()
		{
			return new ReadOnlyCollection<string>(
						new List<string>( this.primitiveTypeInstances.Keys ) );
		}

		/// <summary>
		/// Retrieves a pointer type, such as int *
		/// </summary>
		/// <returns>The pointer type, as a <see cref="Ptr"/> object.</returns>
		/// <param name="t">The regular type to build the pointer on.</param>
		public Ptr GetPtrType(Type t)
		{
			Ptr toret = null;

			if ( !( this.ptrTypeInstances.TryGetValue( t, out toret ) ) ) {
				toret = new Ptr( t.Name + Ptr.PtrTypeNamePart, t, this.Machine.WordSize );
				this.ptrTypeInstances.Add( t, toret );
			}

			return toret;
		}

		/// <summary>
		/// Retrieves a reference type, such as int &
		/// </summary>
		/// <returns>The reference type, as a <see cref="Ref"/> object.</returns>
		/// <param name="t">The regular type to build the reference on.</param>
		public Ref GetRefType(Type t)
		{
			Ref toret = null;

			if ( !( this.refTypeInstances.TryGetValue( t, out toret ) ) ) {
				toret = new Ref( t.Name + Ref.RefTypeNamePart, t, this.Machine.WordSize );
				this.refTypeInstances.Add( t, toret );
			}

			return toret;
		}

		/// <summary>
		/// Retrieves a reference type, such as int[]
		/// </summary>
		/// <returns>The vector type, as a <see cref="Vector"/> object.</returns>
		/// <param name="t">The regular type to build the vector on.</param>
		public Vector GetVectorType(Type t)
		{
			Vector toret = null;

			if ( !( this.vectorTypeInstances.TryGetValue( t, out toret ) ) ) {
				toret = new Vector( t.Name + Vector.TypeName, t, this.Machine.WordSize );
				this.vectorTypeInstances.Add( t, toret );
			}

			return toret;
		}

		/// <summary>
		/// Creates a literal of the given type.
		/// </summary>
		/// <returns>The literal, as a <see cref="Literal"/> object.</returns>
		/// <param name="t">The type, as a <see cref="Type"/> object.</param>
		/// <param name="raw">A vector of bytes.</param>
		public Literal CreateLiteral(Type t, byte[] raw)
		{
			return t.CreateLiteral( this.Machine, raw );
		}

		/// <summary>
		/// Gets the machine all the types depend on.
		/// </summary>
		/// <value>The machine.</value>
		public Machine Machine {
			get {
				return this.machine;
			}
		}

		private Machine machine;
		private Dictionary<Type, Ptr> ptrTypeInstances;
		private Dictionary<Type, Ref> refTypeInstances;
		private Dictionary<Type, Vector> vectorTypeInstances;
		private Dictionary<string, Type> primitiveTypeInstances;
	}
}

