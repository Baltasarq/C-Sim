using System;
using System.Reflection;

using CSim.Core.Exceptions;
using CSim.Core.Types;
using CSim.Core.Types.Primitives;

namespace CSim.Core
{
    /// <summary>
    /// Represents all available types, i.e.: int, float, double...
    /// </summary>
    public abstract class Type
    {
        public Type(string n, int s)
        {
            this.name = n;
            this.size = s;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name of the type
        /// </value>
        public string Name {
            get { return this.name; }
        }

        /// <summary>
        /// Gets or sets the size
        /// </summary>
        /// <value>
        /// The size in bytes
        /// </value>
        public int Size {
            get { return this.size; }
        }

		/*
		/// <summary>
		/// Gets the type determined by the parameter, or null if it does not exist.
		/// </summary>
		/// <param name="typeName">The type name, as a string.</param>
		/// <returns>the type, as a Type object, if a type with that name exists, null otherwise.</returns>
		private static Type GetTypeOrNull(string typeName)
		{
			Type toret = null;

			foreach(var classInfo in typeof( Type ).Assembly.GetTypes())
			{
				if ( classInfo.IsSubclassOf( typeof( Type ) ) ) {
					FieldInfo classTypeNameField = classInfo.GetField( "TypeName" );
					string classTypeName;

					if ( classTypeNameField != null ) {
						classTypeName = (string) classTypeNameField.GetValue( null );

						if ( classTypeName == typeName ) {
							toret = (Type) classInfo.InvokeMember(
								"Get",
								BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod,
								null, null, null
								);
							break;
						}
					}
				}
			}

			return toret;
		}

		/// <summary>
		/// Gets the Type object associated with that name
		/// </summary>
		/// <returns>The type, as a Type object, with that name.</returns>
		/// <param name="typeName">The type name, as a string.</param>
		/// <exception cref="UnknownTypeException">If no type with that name exists.</exception>
        public static Type GetType(string typeName)
        {
            Type toret = GetTypeOrNull( typeName );

			if ( toret == null ) {
				throw new UnknownTypeException( typeName );
			}

            return toret;
        }
	*/
		/// <summary>
		/// Creates a literal for this type, given a byte sequence.
		/// </summary>
		/// <returns>The literal, as an appropriate object of a class inheriting from Literal.</returns>
		/// <param name="raw">The sequence of bytes containing the value in memory.</param>
		public abstract Literal CreateLiteral(Machine m, byte[] raw);

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="CSim.Core.Type"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="CSim.Core.Type"/>.</returns>
		public override string ToString()
		{
			return string.Format( "[Type: name={0}, size={1}]", this.Name, this.Size );
		}

		public bool IsArithmetic() {
			return ( this is Core.Types.Primitives.Int
				  || this is Core.Types.Primitives.Char
				  || this is Core.Types.Primitives.Double );
		}

		public bool IsAny() {
			bool toret = ( this is Any );

			// "any" type (void*) or pointer to any.
			if ( !toret ) {
				Ptr ptr = this as Ptr;

				if ( ptr != null ) {
					toret = ( ptr.AssociatedType is Any );
				}
			}

			return toret;
		}

		public bool IsCompatibleWith(Type other)
		{
			bool toret = ( this.IsAny() || other.IsAny() );

			// Maybe they are just the same
			if ( !toret ) {
				toret = ( this == other );
			}

			// One of them is a pointer and the other a numeric value
			if ( !toret ) {
				toret = ( ( this is Ptr && other.IsArithmetic() )
				  	   || ( this.IsArithmetic() && other is Ptr ) );
			}

			// Compatibility between arithmetic types
			if ( !toret ) {
				toret = ( this.IsArithmetic() || other.IsArithmetic() );
			}
 
			return toret;
		}

        private string name;
        private int size;
    }
}
