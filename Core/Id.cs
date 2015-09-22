using System;

using CSim.Core.Exceptions;

namespace CSim.Core {
	/// <summary>
	/// Represents identifiers.
	/// </summary>
	public class Id: RValue {
		public const int MaxLength = 64;

		public Id(string id)
		{
			this.Value = id;
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

			if ( !id.StartsWith( SymbolTable.MemBlockName, StringComparison.InvariantCulture ) )
			{
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
								+ L18n.Get( L18n.Id.ErrNotAllowedChar )
								+ ": " + id[ i ] + ")" );
						}

						if ( isValuableChar ) {
							++countRealChars;
						}
					}
				} else {
					throw new InvalidIdException( id
					                             + " ( "
					                             + L18n.Get( L18n.Id.ErrFirstChar )
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
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public string Value {
			get {
				return this.id;
			}
			set {
				Id.Chk( value );
				this.id = value.Trim();
			}
		}

		/// <summary>
		/// Gets the type of the rvalue.
		/// </summary>
		/// <value>The type.</value>
		public override CSim.Core.Type Type {
			get {
				throw new TypeMismatchException( this.Value + "??" );
			}
		}

		/// <summary>
		/// Sets the name without checking.
		/// This is needed internally, not for users.
		/// </summary>
		/// <param name="id">The new name, as a string.</param>
		internal void SetIdWithoutChecks(string id)
		{
			this.id = id.Trim();
		}

		public bool IsHeapId()
		{
			return this.id.StartsWith( SymbolTable.MemBlockName, StringComparison.InvariantCulture );
		}

		private string id;
	}
}

