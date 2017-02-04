
namespace CSim.Core {
	using System;

	/// <summary>
	/// The lexer.
	/// </summary>
    public class Lexer {
		/// <summary>Special characters.</summary>
        public const string SpecialCharacter = ";.,=(){}";
		/// <summary>Boolean literals.</summary>
		public const string BooleanLiterals = " true false ";
		/// <summary>Allowed digits in hexadecimal notation.</summary>
		public const string HexDigits = "0123456789abcdef";
		/// <summary>End of sentence.</summary>
        public const char EOS = ';';

		/// <summary>Token types</summary>
        public enum TokenType {
			/// <summary>Identifier token type</summary>
			Id,
			/// <summary>Boolean literal token type</summary>
			BooleanValue,
			/// <summary>Hexadecimal number token type.</summary>
			HexNumber,
			/// <summary>Decimal numbers token type.</summary>
			IntNumber,
			/// <summary>Real number token type.</summary>
			RealNumber,
			/// <summary>Character token type.</summary>
			Char,
			/// <summary>Text token type.</summary>
			Text,
			/// <summary>Special character token type.</summary>
			SpecialCharacter,
			/// <summary>Invalid token type.</summary>
			Invalid
		};
        
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Lexer"/> class.
		/// </summary>
		/// <param name="ln">The line to extract tokens from.</param>
        public Lexer(string ln)
        {
            this.Line = ln.Trim();
            this.Pos = 0;
        }
        
		/// <summary>
		/// Advance a position in the input.
		/// </summary>
        public void Advance()
        {
            Advance( 1 );
        }
        
		/// <summary>
		/// Advance the specified amount of positions in input.
		/// </summary>
		/// <param name="value">The number of positions.</param>
        public void Advance(int value)
        {
            this.Pos += value;
        }
        
		/// <summary>
		/// Skips the following spaces in the input
		/// </summary>
        public void SkipSpaces()
        {
            while( !this.IsEOL()
                && char.IsWhiteSpace( Line[ this.Pos ] ) )
            {
                this.Advance();
            }
            
            return;
        }
        
		/// <summary>
		/// Determines whether it is the end of sentence or not.
		/// </summary>
		/// <returns><c>true</c>, if end of sentence, <c>false</c> otherwise.</returns>
        public bool IsEOL()
        {
            return ( this.Pos >= Length );
        }
        
		/// <summary>
		/// Gets the next token.
		/// </summary>
		/// <returns>The token, as a string.</returns>
        public string GetToken()
        {
			this.CurrentToken = "";
            SkipSpaces();
            char ch = GetCurrentChar();
            
            // Chk first char for special tokens
            if ( ch == '"' ) {
                this.GetStringLiteral();
            }
            else
            if ( ch == '\'' ) {
                this.GetCharacter();
            }
            else
            if ( Char.IsDigit( ch )
                || ch == '+'
                || ch == '-' )
            {
                this.GetNumber();
            }
            else {
                while( ( Char.IsLetterOrDigit( ch )
                        || ch == '_' )
                      && Pos < this.Length )
                {
					this.CurrentToken += ch;
                    this.Advance();
                    ch = this.GetCurrentChar();
                }
            }
            
            this.SkipSpaces();
			return this.CurrentToken;
        }

        /// <summary>
        /// Reads a number in the source, in the format: (9)+[.(9)+]
        /// </summary>
        /// <returns>
        /// The number, as a string
        /// </returns>
        public string GetNumber()
        {
			string token = "";
            SkipSpaces();
            char ch = GetCurrentChar();
            
            // Read the first sign, if exists
            if ( ch == '+'
              || ch == '-' )
            {
				token += ch;
                this.Advance();
                ch = GetCurrentChar();
            }
            
            while( Char.IsDigit( ch )
                || ch == '.' )
            {
				token  += ch;
                this.Advance();
                ch = GetCurrentChar();
            }
            
			this.CurrentToken = token;
            return token;
        }

		/// <summary>
        /// Reads a number in the source, in the format: (9,a...d)+[.(9,a...d)+]
        /// </summary>
        /// <returns>
        /// The number, as a string
        /// </returns>
        public string GetHexNumber()
        {
			bool pointRead = false;

            string token = "";
            SkipSpaces();
            char ch = GetCurrentChar();
            
            // Read the first sign, if exists
            if ( ch == '+'
              || ch == '-' )
            {
                token += ch;
                this.Advance();
                ch = GetCurrentChar();
            }
            
			// Read the number itself
            while( HexDigits.IndexOf( ch ) >= 0
			    || ( ch == '.'
			      && !pointRead ) )
			{
				if ( ch == '.' ) {
					pointRead = true;
				}

                token += ch;
                this.Advance();
                ch = GetCurrentChar();
            }
            
			this.CurrentToken = token;
            return token;
        }
        
        /// <summary>
        /// Gets the next character literal in the source code.
        /// </summary>
        /// <returns>
        /// A string formed by a ', char, and '
        /// </returns>
        public string GetCharacter()
        {
            this.SkipSpaces();
            int oldPos = Pos;
			this.CurrentToken = "";
            
			if ( this.GetCurrentChar() == '\'' ) {
                this.Advance();
                
				this.CurrentToken += '\'';
				this.CurrentToken += this.GetCurrentChar();
                
				if ( this.GetCurrentChar() == '\\' ) {
					this.Advance();
					this.CurrentToken += this.GetCurrentChar();
                }
                
				this.Advance();
				if ( this.GetCurrentChar() != '\'' ) {
                    this.Pos = oldPos;
                }
                
				this.CurrentToken += '\'';
				this.Advance();
				this.SkipSpaces();
            }
            
			return this.CurrentToken;
        }
        
		/// <summary>
		/// Gets the following string literal.
		/// </summary>
		/// <returns>The string literal.</returns>
        public string GetStringLiteral()
        {
            return GetLiteral( '"', '"' );
        }
        
		/// <summary>
		/// Gets any literal, given the opening and ending delimiters.
		/// </summary>
		/// <returns>The literal, as a string.</returns>
		/// <param name="openDelimiter">The opening delimiter, a char.</param>
		/// <param name="endDelimiter">The ending delimiter, a char.</param>
        public string GetLiteral(char openDelimiter, char endDelimiter)
        {
            bool escaped = false;
            
			this.CurrentToken = "";
            SkipSpaces();
            
            if ( GetCurrentChar() == openDelimiter ) {
				this.CurrentToken += openDelimiter;
                Advance();
                
                while( GetCurrentChar() != endDelimiter
                    || escaped )
                {
					this.CurrentToken += GetCurrentChar();
                    
                    if ( GetCurrentChar() == '\\' )
                            escaped = true;
                    else    escaped = false;
                    
                    Advance();
                }
                
                Advance();
				this.CurrentToken += endDelimiter;
            }
            
            SkipSpaces();
			return this.CurrentToken;
        }
        
		/// <summary>
		/// Gets the current char in the input.
		/// </summary>
		/// <returns>The current char.</returns>
        public char GetCurrentChar()
        {
            char toret = '\0';
            
            if ( !this.IsEOL() ) {
                toret = this.Line[ this.Pos ];
            }
            
            return toret;
        }
        
		/// <summary>
		/// Gets the first char of the input.
		/// </summary>
		/// <returns>The first char.</returns>
        public char GetFirstChar()
        {
            return Line[ 0 ];
        }
        
		/// <summary>
		/// Gets the last char of the input.
		/// </summary>
		/// <returns>The last char.</returns>
        public char GetLastChar()
        {
            return Line[ Length -1 ];
        }
        
		/// <summary>
		/// Gets the type of the current token.
		/// </summary>
		/// <returns>The current token type.</returns>
        public TokenType GetCurrentTokenType()
        {
            TokenType toret = TokenType.Invalid;
			string token = this.CurrentToken;
            
			if ( token.Length > 0 ) {
                char ch = token[ 0 ];
                
				if ( IsBool( token ) ) {
                    toret = TokenType.BooleanValue;
                }
                else
                if ( ch == '\''
				  && IsCharacter( token ) )
                {
                    toret = TokenType.Char;
                }
                else
                if ( ch == '"'
				  && IsStringLiteral( token ) )
                {
                    toret = TokenType.Text;
                }
				else
				if (  ch == '+'
				   || ch == '-'
				   || char.IsDigit( ch ) )
				{
					int pos = 0;
					
					if ( ch == '+'
					  || ch == '-' )
					{
						++pos;
					}

					if ( pos < token.Length
					  && token[ pos ] == '0'
					  && char.ToUpper( this.GetCurrentChar() ) == 'X' )
					{
						toret = TokenType.HexNumber;
					} else {
						toret = TokenType.RealNumber;

						if ( !IsRealNumber( token ) ) {
							toret = TokenType.Invalid;
						}

						if ( IsIntNumber( token ) ) {
							toret = TokenType.IntNumber;
						}
					}
				}
				else
                if ( Char.IsLetter( ch )
                    || ch == '_' )
                {
                    toret = TokenType.Id;
                }
                else
                if ( SpecialCharacter.IndexOf( ch ) > -1 ) {
                    toret = TokenType.SpecialCharacter;
                }
            }
            
            return toret;
        }
        
		/// <summary>
		/// Is the current token an rvalue?
		/// </summary>
		/// <returns><c>true</c>, if it is a RValue, <c>false</c> otherwise.</returns>
        public bool IsRValue()
        {
            TokenType type = GetCurrentTokenType();
            
            return ( type == TokenType.BooleanValue
                    || type == TokenType.IntNumber
					|| type == TokenType.RealNumber
                    || type == TokenType.Text
                    || type == TokenType.Char
                    || type == TokenType.Id )
                ;
        }
        
		/// <summary>
		/// Gets the type of the next token.
		/// It won't affect the next token lecture.
		/// </summary>
		/// <returns>The next <see cref="TokenType"/>.</returns>
        public TokenType GetNextTokenType()
        {
            SkipSpaces();
            int oldPos = Pos;
            
			this.GetToken();
            TokenType toret = GetCurrentTokenType();
            
            Pos = oldPos;
			this.CurrentToken = "";
            
            return toret;
        }
        
        /// <summary>
        /// Determines wether the token is a real number or not.
        /// </summary>
        /// <returns>
        /// true when number, false otherwise
        /// </returns>
        /// <param name='token'>
        /// A string to compare
        /// </param>
        public static bool IsRealNumber(string token) {
            double number;

            return double.TryParse( token, out number );
        }

		/// <summary>
		/// Determines wether the token is an integer number or not.
		/// </summary>
		/// <returns>
		/// true when number, false otherwise
		/// </returns>
		/// <param name='token'>
		/// A string to compare
		/// </param>
		public static bool IsIntNumber(string token) {
			int number;

			return int.TryParse( token, out number );
		}


        /// <summary>
        /// Determines whether the token holds a char of the form: 'c'
        /// </summary>
        /// <returns>
        /// true when it is a character conforming the format, false otherwise
        /// </returns>
        /// <param name='token'>
        /// The token to evaluate
        /// </param>
        public static bool IsCharacter(string token)
        {
            bool toret = false;
            
            if ( token[ 0 ] == '\''
              && token[ token.Length -1 ] == '\''  )
            {
                if ( token.Length != 3 ) {
                    toret = ( token[ 1 ] == '\\' && token.Length == 4 );
                }
                else toret = true;
            }
            
            return toret;
        }
        
        /// <summary>
        /// Decides whether the token holds a string literal or not
        /// </summary>
        /// <returns>
        /// <c>true</c>, if it holds text delimited by double-quotes, <c>false</c> otherwise.
        /// </returns>
        /// <param name='token'>
        /// The string to decide on the format: "text"
        /// </param>
        public static bool IsStringLiteral(string token)
        {
            return IsLiteral( '"', '"', token );
        }
        
        /// <summary>
        /// Decides whether the token holds a literal or not
        /// </summary>
        /// <returns>
        /// true if it holds text delimited by delimiter, false otherwise
        /// </returns>
        /// <param name='openDelimiter'>
        /// Open delimiter.
        /// </param>
        /// <param name='endDelimiter'>
        /// End delimiter.
        /// </param>
        /// <param name='token'>
        /// The string to decide on the format: "delimiter literal delimiter"
        /// </param>
        public static bool IsLiteral(char openDelimiter, char endDelimiter, string token)
        {
            return ( token[ 0 ] == openDelimiter
                  && token[ token.Length -1 ] == endDelimiter );
        }
        
        /// <summary>
        /// Decides whether the token is a boolean or not.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if it is "true" or "false", <c>false</c> otherwise.
        /// </returns>
        /// <param name='token'>
        /// A string containing "true", "false", or any other value.
        /// </param>
        public static bool IsBool(string token) {
			return ( BooleanLiterals.IndexOf( " " + token + " ", StringComparison.InvariantCulture ) > -1 );
        }

		/// <summary>
		/// Gets or sets the position in the input.
		/// </summary>
		/// <value>The position.</value>
        public int Pos {
            get; set;
        }

		/// <summary>
		/// Gets or sets the line input.
		/// </summary>
		/// <value>The line.</value>
        public string Line {
            get; set;
        }

		/// <summary>
		/// Gets the length of the input.
		/// </summary>
		/// <value>The length.</value>
        public int Length {
            get { return this.Line.Length; }
        }
        
        /// <summary>
        /// Gets the current token.
        /// </summary>
        /// <value>
        /// The current token, as a string.
        /// </value>
        public string CurrentToken {
			get; private set;
        }
    }
}

