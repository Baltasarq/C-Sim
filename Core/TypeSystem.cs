
namespace CSim.Core {
    using System.Reflection;
	using System.Collections.ObjectModel;
	using System.Collections.Generic;

	using CSim.Core.Types;
	using CSim.Core.Types.Primitives;

	/// <summary>
	/// Represents the type system.
	/// All types are retrieved through an object of this
	/// class, since types are dependent on the architecture (i.e., size).
	/// </summary>
	public class TypeSystem {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.TypeSystem"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/> for this type system.</param>
		public TypeSystem(Machine m)
		{
			this.Machine = m;
			this.ptrTypeInstances = new Dictionary<AType, List<Ptr>>();
			this.refTypeInstances = new Dictionary<AType, Ref>();
			this.primitiveTypeInstances = new Dictionary<string, AType>();

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
			this.primitiveTypeInstances.Clear();

            // Add all primitive type instances
            var asm = typeof( AType ).Assembly;

            foreach(System.Type t in asm.GetTypes()) {
                if ( t.IsClass
                  && t.IsSubclassOf( typeof( Primitive ) ) )
                {
                    // Erase the old instance so the type is re-created
                    FieldInfo fldInfo = t.GetField( "instance", BindingFlags.Static | BindingFlags.NonPublic );
                    fldInfo.SetValue( null, null );
                
                    // Create the type
                    MethodInfo mthInfo = t.GetMethod( "Get" );
                    var primitive = (Primitive) mthInfo.Invoke( null, new object[] { this.Machine } );
                    this.primitiveTypeInstances.Add( primitive.Name, primitive );
                }
            }

            // Add the type type.
            this.primitiveTypeInstances.Add( TypeType.TypeName, TypeType.Get( this.Machine ) );
		}
        
        /// <summary>
        /// Gets the <see cref="TypeType"/>.
        /// </summary>
        /// <returns>The type <see cref="TypeType"/>.</returns>
        public TypeType GetTypeType()
        {
            return (TypeType) this.GetPrimitiveType( TypeType.TypeName );
        }
        
        /// <summary>
        /// Gets the char type.
        /// </summary>
        /// <returns>The char type, as a <see cref="CSim.Core.Types.Primitives.Char"/> object.</returns>
        public Char GetCharType()
        {
            return (Char) this.GetPrimitiveType( Char.TypeName );
        }
        
        /// <summary>
        /// Gets the short type.
        /// </summary>
        /// <returns>The short type, as a <see cref="Short"/> object.</returns>
        public Short GetShortType()
        {
            return (Short) this.GetPrimitiveType( Short.TypeName );
        }
        
        /// <summary>
        /// Gets the unsigned short type.
        /// </summary>
        /// <returns>The ushort type, as a <see cref="UShort"/> object.</returns>
        public UShort GetUShortType()
        {
            return (UShort) this.GetPrimitiveType( UShort.TypeName );
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
        /// Gets the unsigned int type.
        /// </summary>
        /// <returns>The unsigned int type, as a <see cref="UInt"/> object.</returns>
        public UInt GetUIntType()
        {
            return (UInt) this.GetPrimitiveType( UInt.TypeName );
        }
        
        /// <summary>
        /// Gets the long int type.
        /// </summary>
        /// <returns>The int type, as a <see cref="Long"/> object.</returns>
        public Long GetLongType()
        {
            return (Long) this.GetPrimitiveType( Long.TypeName );
        }
        
        /// <summary>
        /// Gets the unsigned long int type.
        /// </summary>
        /// <returns>The int type, as a <see cref="ULong"/> object.</returns>
        public ULong GetULongType()
        {
            return (ULong) this.GetPrimitiveType( ULong.TypeName );
        }

		/// <summary>
		/// Gets the float type.
		/// </summary>
		/// <returns>The float type, as a <see cref="Float"/> object.</returns>
		public Float GetFloatType()
		{
			return (Float) this.GetPrimitiveType( Float.TypeName );
		}
        
        /// <summary>
        /// Gets the double type.
        /// </summary>
        /// <returns>The double type, as a <see cref="Double"/> object.</returns>
        public Double GetDoubleType()
        {
            return (Double) this.GetPrimitiveType( CSim.Core.Types.Primitives.Double.TypeName );
        }
        
		/// <summary>
		/// Gets any type supported by this machine, given its name.
		/// </summary>
		/// <returns>The type, as a <see cref="AType"/> object.</returns>
		/// <param name="strType">A string with the type's name.</param>
		public AType GetPrimitiveType(string strType)
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
        /// <param name="indirectionLevel">Number of stars in the type.</param>
		public Ptr GetPtrType(AType t, int indirectionLevel = -1)
		{
			List<Ptr> ptrTypes = null;
            var ptrType = t as Ptr;
            AType baseType = t;

            // Determine indirectionLevel
            if ( indirectionLevel < 1 ) {
                indirectionLevel = 1;            
                if ( ptrType != null ) {
                    indirectionLevel = ptrType.IndirectionLevel + 1;
                }
            }
            
            // Determine base type
            if ( ptrType != null ) {
                baseType = ptrType.AssociatedType;
            }

            // Create list of ptr types for that base type, if needed
			if ( !( this.ptrTypeInstances.TryGetValue( baseType, out ptrTypes ) ) )
            {
				ptrTypes = new List<Ptr>();
				this.ptrTypeInstances.Add( baseType, ptrTypes ); 
				ptrTypes.Add( new Ptr( 1, baseType ) ); // T *
			}
				
            // Create all intermediate ptr types, honoring the indirection level
			for(int i = ptrTypes.Count; i < indirectionLevel; ++i) {
				ptrTypes.Add( new Ptr( i + 1, baseType ) );
			}

			return ptrTypes[ indirectionLevel - 1 ];
		}

		/// <summary>
		/// Retrieves a reference type, such as int &amp;
		/// </summary>
		/// <returns>The reference type, as a <see cref="Ref"/> object.</returns>
		/// <param name="t">The regular type to build the reference on.</param>
		public Ref GetRefType(AType t)
		{
			Ref toret = null;

			if ( !( this.refTypeInstances.TryGetValue( t, out toret ) ) ) {
				toret = new Ref( t );
				this.refTypeInstances.Add( t, toret );
			}

			return toret;
		}

		/// <summary>
		/// Returns the type from a textual description.
		/// </summary>
		/// <returns>The <see cref="AType"/>corresponding to the given type.</returns>
		/// <param name="strType">String type.</param>
		public AType FromStringToType(string strType)
		{
			int numStars = 0;
			bool isRef = false;
			AType toret = null;
            var lexer = new Lexer( strType.Trim() );

            lexer.Pos = lexer.Length - 1;

			// Determine whether it is a reference or not
			while ( lexer.Pos > 0
			     && lexer.GetCurrentChar() == Ref.RefTypeNamePart[ 0 ] )
			{
				isRef = true;
				lexer.Advance( -1 );
                lexer.SkipSpaces( -1 );
			}

			// Determine the indirection level
			while( lexer.Pos > 0
			    && lexer.GetCurrentChar() == Ptr.PtrTypeNamePart[ 0 ] )
			{
				++numStars;
				lexer.Advance( -1 );
                lexer.SkipSpaces( -1 );
			}

			// Get the primitive type
			strType = lexer.Line.Substring( 0, lexer.Pos + 1 );
			AType primitive = this.GetPrimitiveType( strType );

			if ( isRef ) {
				if ( numStars == 0 ) {
					toret = this.GetRefType( primitive );
				} else {
					toret = this.GetRefType( this.GetPtrType( primitive, numStars ) );
				}
			}
			else
			if ( numStars > 0 ) {
				toret = this.GetPtrType( primitive, numStars );
			} else {
				toret = primitive;
			}

			return toret;
		}

		/// <summary>
		/// Creates a literal of the given type.
		/// </summary>
		/// <returns>The literal, as a <see cref="Literal"/> object.</returns>
		/// <param name="t">The type, as a <see cref="AType"/> object.</param>
		/// <param name="raw">A vector of bytes.</param>
		public Literal CreateLiteral(AType t, byte[] raw)
		{
			return t.CreateLiteral( raw );
		}

		/// <summary>
		/// Gets the machine all the types depend on.
		/// </summary>
		/// <value>The machine.</value>
		public Machine Machine {
			get; set;
		}

		private Dictionary<AType, List<Ptr>> ptrTypeInstances;
		private Dictionary<AType, Ref> refTypeInstances;
		private Dictionary<string, AType> primitiveTypeInstances;
	}
}

