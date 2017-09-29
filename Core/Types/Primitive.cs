
namespace CSim.Core.Types {
    using System.Reflection;
    using System.Collections.Generic;

    /// <summary>
    /// Represents primitive types
    /// </summary>
    public abstract class Primitive: AType {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Types.Primitive"/> class.
		/// </summary>
        /// <param name="m">The <see cref="Machine"/> this primitive will live in.</param>
        /// <param name="name">The name of the <see cref="AType"/>.</param>
        /// <param name="size">The size of the <see cref="AType"/>.</param>
        protected Primitive(Machine m, string name, int size)
            :base( m, name, size )
        {
        }

		/// <summary>
		/// Creates a corresponding variable.
		/// </summary>
		/// <returns>A <see cref="Variable"/> with this <see cref="AType"/>.</returns>
		/// <param name="strId">The identifier for the variable.</param>
		public override Variable CreateVariable(string strId)
		{
			return new Variable( new Id( this.Machine, strId ), this );
		}

        /// <summary>
        /// Determines whether the given id is a <see cref="Core.Literals.TypeLiteral"/>
        /// </summary>
        /// <returns><c>true</c>, if primitive is a <see cref="Core.Literals.TypeLiteral"/>,
        /// <c>false</c> otherwise.</returns>
        /// <param name="strType">The type literal, as a string.</param>
        public static bool IsPrimitive(string strType)
        {
            BuildPrimitiveNamesCollection();
            return primitives.Contains( strType );
        }
        
        /// <summary>
        /// Gets the index of the <see cref="Primitive"/> type.
        /// </summary>
        /// <returns>The index, as an int.</returns>
        /// <param name="strType">The type, as its name.</param>
        /// <exception cref="T:System.ArgumentException">
        ///     when <paramref name="strType"/>
        ///     does not name a <see cref="Primitive"/>.
        /// </exception>
        public static byte GetPrimitiveNameIndex(string strType)
        {
            int toret;
            
            BuildPrimitiveNamesCollection();
            
            toret = primitives.IndexOf( strType );
            
            if ( toret < 0 ) {
                throw new System.ArgumentException(
                    "Type: '" + strType
                    + "' was not found in the primitive index list."
                );
            }
            
            return (byte) toret;
        }
        
        /// <summary>
        /// Gets the primitive at the given index.
        /// </summary>
        /// <returns>The name (as a<see cref="T:System.String"/>), of the type.</returns>
        /// <param name="index">The index, as an int.</param>
        /// <seealso cref="GetPrimitiveNameIndex"/>
        /// <seealso cref="PrimitiveNames"/>
        /// <exception cref="T:System.ArgumentException">
        /// When the <paramref name="index"/> is &lt; 0 or &gt;= # types.
        /// </exception>
        public static string GetPrimitiveNameAt(int index)
        {
            int MaxValue = primitives.Count;
            
            if ( index < byte.MinValue
              || index >= MaxValue )
	        {
                throw new System.ArgumentException(
                    "failed at trying primitive type name's index at: "
                    + index + "' should be 0 < "
                    + index + " < " + MaxValue );
	        }
        
            return primitives[ (byte) index ];
        }

        /// <summary>
        /// Gets all the <see cref="Primitive"/> <see cref="AType"/> names.
        /// </summary>
        /// <value>The primitive names, as a string[].</value>
        public static string[] PrimitiveNames {
            get {
                BuildPrimitiveNamesCollection();
                return primitives.ToArray();
            }
        }
        

        private static void BuildPrimitiveNamesCollection()
        {
            if ( primitives == null ) {
	            primitives = new List<string>();
                                	
	            // Add all primitive type instances
	            var asm = typeof( AType ).Assembly;
	
	            foreach(System.Type t in asm.GetTypes()) {
	                if ( t.IsClass
	                  && t.IsSubclassOf( typeof( Primitive ) ) )
	                {
	                    FieldInfo atrInfo = t.GetField( "TypeName" );
	                    primitives.Add( (string) atrInfo.GetValue( null ) );
	                }
	            }
            }
            
            if ( primitives.Count > byte.MaxValue ) {
                throw new System.ApplicationException( "too many primitive types" );
            }
            
            primitives.Sort();
            return;
        }

        private static List<string> primitives;
    }
}
