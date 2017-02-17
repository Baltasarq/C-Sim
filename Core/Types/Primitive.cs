
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
        /// Gets all the <see cref="Primitive"/> <see cref="AType"/> names.
        /// </summary>
        /// <value>The primitive names, as a string[].</value>
        public static string[] PrimitiveNames {
            get {
                BuildPrimitiveNamesCollection();
                var toret = new string[ primitives.Count ];
                primitives.CopyTo( toret, 0 );
                return toret;
            }
        }

        private static void BuildPrimitiveNamesCollection()
        {
            primitives = new HashSet<string>();

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

        private static ISet<string> primitives;
    }
}

