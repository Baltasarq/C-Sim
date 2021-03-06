﻿// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core {
    using System;
    
    using Exceptions;
    
	/// <summary>
	/// Represents identifiers.
	/// </summary>
	public class Id: RValue {
		/// <summary>Maximum length of any id.</summary>
		public const int MaxLength = 64;

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Id"/> class.
		/// </summary>
        /// <param name="m">The <see cref="Machine"/> this id will be used in.</param>
		/// <param name="id">The identifier to be, as a string.</param>
		public Id(Machine m, string id)
            :base( m )
		{
			this.Text = id;
		}

		/// <summary>
		/// Cheks the specified id.
		/// (_|A..Z|a..z|)+[_|0..9|A..Z|a..z]
		/// i.e., x, x_1, x1, numVertexes, num_vertexes...
		/// Must be of a length less than <see cref="MaxLength"/>.
		/// </summary>
		/// <param name="id">The identifier, as a string.</param>
		/// <exception cref="InvalidIdException">if does not adheres to the riles for id's</exception>
		public static void Chk(string id)
		{
			int countRealChars = 0;

			if ( id == null ) {
				throw new InvalidIdException( "''" );
			}

			id = id.Trim();

			if ( id.Length == 0 ) {
				throw new InvalidIdException( "''" );
			}

			if ( !Reserved.IsReserved( id ) ) {
				bool isValuableChar = Char.IsLetter( id[ 0 ] );

				countRealChars += isValuableChar ? 1 : 0;

				if ( isValuableChar
			      || id[ 0 ] == '_' )
				{
					for(int i = 1;  i < id.Length; ++i) {
						isValuableChar = Char.IsLetterOrDigit( id[ i ] );

						if ( !isValuableChar
						    && id[ i ] != '_' )
						{
							throw new InvalidIdException(
								id + " ("
								+ L10n.Get( L10n.Id.ErrNotAllowedChar )
								+ ": " + id[ i ] + ")" );
						}

						if ( isValuableChar ) {
							++countRealChars;
						}
					}
				} else {
					throw new InvalidIdException( id
					                             + " ( "
					                             + L10n.Get( L10n.Id.ErrFirstChar )
					                             + ")"
					                             );
				}

				// Check at least one char is letter or digit
				if ( countRealChars == 0 ) {
					throw new InvalidIdException( "'" + id + "')" );
				}
			}

			return;
		}

		/// <summary>
		/// Gets or sets the id text.
		/// </summary>
		/// <value>The name, as string.</value>
		public string Text {
			get {
				return this.id;
			}
			set {
				Id.Chk( value );
				this.id = value.Trim();
			}
		}

		/// <summary>
		/// Gets the id itself.
		/// </summary>
		/// <value>The id, as a string.</value>
		public override object Value {
			get {
				return this.Text;
			}
		}

		/// <summary>
        /// Gets the <see cref="AType"/> of the <see cref="RValue"/>.
		/// </summary>
        /// <value>The <see cref="AType"/>.</value>
		public override AType Type {
			get {
				throw new TypeMismatchException( this.Value + "??" );
			}
		}

		/// <summary>
		/// Sets the name without checking.
		/// This is needed internally, not for users.
		/// </summary>
		/// <param name="newId">The new name, as a string.</param>
		internal void SetIdWithoutChecks(string newId)
		{
			this.id = newId.Trim();
		}

		/// <summary>
		/// Determines whether this instance is a heap identifier.
		/// </summary>
		/// <returns><c>true</c> if this instance is heap identifier; otherwise, <c>false</c>.</returns>
		public bool IsHeapId()
		{
			return this.id.StartsWith( Reserved.PrefixMemBlockName,
                                       StringComparison.InvariantCulture );
		}

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Id"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Id"/>.</returns>
        public override string ToString()
        {
            return this.Text;
        }
        
        /// <summary>
        /// Solves to variable.
        /// </summary>
        /// <returns>The to variable.</returns>
        public override Variable SolveToVariable()
        {
            return this.Machine.TDS.LookUp( this.Text );
        }

		private string id;
	}
}

