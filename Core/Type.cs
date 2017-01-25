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
		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Type"/> class.
		/// </summary>
		/// <param name="n">The name of the type.</param>
		/// <param name="s">Its size, in bytes</param>
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

		/// <summary>
		/// Creates a literal for this type, given a byte sequence.
		/// </summary>
		/// <returns>The literal, as an appropriate object of a class inheriting from Literal.</returns>
		/// <param name="raw">The sequence of bytes containing the value in memory.</param>
		/// <param name="m">The <see cref="Machine"/> to create this literal in.</param>
		public abstract Literal CreateLiteral(Machine m, byte[] raw);

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="CSim.Core.Type"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="CSim.Core.Type"/>.</returns>
		public override string ToString()
		{
			return string.Format( "[Type: name={0}, size={1}]", this.Name, this.Size );
		}

		/// <summary>
		/// Determines whether the type is arithmetic.
		/// </summary>
		/// <returns><c>true</c> if this instance is arithmetic; otherwise, <c>false</c>.</returns>
		public bool IsArithmetic() {
			return ( this is Core.Types.Primitives.Int
				  || this is Core.Types.Primitives.Char
				  || this is Core.Types.Primitives.Double );
		}

		/// <summary>
		/// Determines whether this type is any,
		/// or a "de facto" <see cref=">Any"/>.
		/// </summary>
		/// <returns><c>true</c> if this instance is any or similar; otherwise, <c>false</c>.</returns>
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

		/// <summary>
		/// Determines whether this instance is compatible with another, given one.
		/// </summary>
		/// <returns><c>true</c> if this instance is compatible with the specified other; otherwise, <c>false</c>.</returns>
		/// <param name="other">The other <see cref="Type"/>.</param>
		public bool IsCompatibleWith(Type other)
		{
			bool toret = ( this.IsAny() || other.IsAny() );

			// Maybe they are just the same
			if ( !toret ) {
				toret = ( this == other );
			}

			// They are both pointers
			if ( !toret ) {
				toret = ( this is Ptr && other is Ptr );
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
