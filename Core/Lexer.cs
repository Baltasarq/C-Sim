using System;

namespace CSim.Core
{
    public class Lexer
    {
        public const string SpecialCharacter = ";.,=(){}";
        public const string BooleanValues = " true false ";
		public const string HexDigits = "0123456789abcdef";
        public const char EOS = ';';
        public enum TokenType { Id, BooleanValue, Number, Char, Text, SpecialCharacter, Invalid };
        
        public Lexer(string ln)
        {
            this.Line = ln.Trim();
            this.Pos = 0;
        }
        
        public void Advance()
        {
            Advance( 1 );
        }
        
        public void Advance(int value)
        {
            this.Pos += value;
        }
        
        public void SkipSpaces()
        {
            while( !this.IsEOL()
                && char.IsWhiteSpace( Line[ this.Pos ] ) )
            {
                this.Advance();
            }
            
            return;
        }
        
        public bool IsEOL()
        {
            return ( this.Pos >= Length );
        }
        
        public string GetToken()
        {
            token = "";
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
                    token += ch;
                    this.Advance();
                    ch = this.GetCurrentChar();
                }
            }
            
            this.SkipSpaces();
            return token;
        }

        /// <summary>
        /// Reads a number in the source, in the format: (9)+[.(9)+]
        /// </summary>
        /// <returns>
        /// The number, as a string
        /// </returns>
        public string GetNumber()
        {
            token = "";
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
                token += ch;
                this.Advance();
                ch = GetCurrentChar();
            }
            
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

            token = "";
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
            
            return token;
        }
        
        /// <summary>
        /// Gets the next character in the source code.
        /// </summary>
        /// <returns>
        /// A string formed by a ', char, and '
        /// </returns>
        public string GetCharacter()
        {
            this.SkipSpaces();
            int oldPos = Pos;
            token = "";
            
            if ( GetCurrentChar() == '\'' ) {
                this.Advance();
                
                token += '\'';
                token += GetCurrentChar();
                
                if ( GetCurrentChar() == '\\' ) {
                    Advance();
                    token += GetCurrentChar();
                }
                
				this.Advance();
                if ( GetCurrentChar() != '\'' ) {
                    this.Pos = oldPos;
                }
                
                token += '\'';
                Advance();
                SkipSpaces();
            }
            
            return token;
        }
        
        public string GetStringLiteral()
        {
            return GetLiteral( '"', '"' );
        }
        
        public string GetLiteral(char openDelimiter, char endDelimiter)
        {
            bool escaped = false;
            
            token = "";
            SkipSpaces();
            
            if ( GetCurrentChar() == openDelimiter ) {
                token += openDelimiter;
                Advance();
                
                while( GetCurrentChar() != endDelimiter
                    || escaped )
                {
                    token += GetCurrentChar();
                    
                    if ( GetCurrentChar() == '\\' )
                            escaped = true;
                    else    escaped = false;
                    
                    Advance();
                }
                
                Advance();
                token += endDelimiter;
            }
            
            SkipSpaces();
            return token;
        }
        
        public char GetCurrentChar()
        {
            char toret = '\0';
            
            if ( !this.IsEOL() ) {
                toret = Line[ Pos ];
            }
            
            return toret;
        }
        
        public char GetFirstChar()
        {
            return Line[ 0 ];
        }
        
        public char GetLastChar()
        {
            return Line[ Length -1 ];
        }
        
        public TokenType GetCurrentTokenType()
        {
            TokenType toret = TokenType.Invalid;
            
            if ( GetCurrentToken().Length > 0 ) {
                char ch = GetCurrentToken()[ 0 ];
                
                if ( IsBool( GetCurrentToken() ) ) {
                    toret = TokenType.BooleanValue;
                }
                else
                if ( ch == '\''
                  && IsCharacter( GetCurrentToken() ) )
                {
                    toret = TokenType.Char;
                }
                else
                if ( ch == '"'
                  && IsStringLiteral( GetCurrentToken() ) )
                {
                    toret = TokenType.Text;
                }
                else
                if ( ( Char.IsDigit( ch )
                      || ch == '+'
                      || ch == '-' )
                    && IsNumber( GetCurrentToken() ) )
                {
                    toret = TokenType.Number;
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
        
        public bool IsRValue()
        {
            TokenType type = GetCurrentTokenType();
            
            return ( type == TokenType.BooleanValue
                    || type == TokenType.Number
                    || type == TokenType.Text
                    || type == TokenType.Char
                    || type == TokenType.Id )
                ;
        }
        
        public TokenType GetNextTokenType()
        {
            SkipSpaces();
            int oldPos = Pos;
            GetToken();
            TokenType toret = GetCurrentTokenType();
            
            Pos = oldPos;
            token = "";
            
            return toret;
        }
        
        /// <summary>
        /// Determines wether the token is a number or not.
        /// </summary>
        /// <returns>
        /// true when number, false otherwise
        /// </returns>
        /// <param name='token'>
        /// A string to compare
        /// </param>
        public static bool IsNumber(string token) {
            double number;

            return Double.TryParse( token, out number );
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
        /// The string to decide on the format: <delimiter>literal<delimiter>
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
            return ( BooleanValues.IndexOf( " " + token + " " ) > -1 );
        }

        public int Pos {
            get; set;
        }

        public string Line {
            get; set;
        }

        public int Length {
            get { return this.Line.Length; }
        }
        
        /// <summary>
        /// Gets the current token.
        /// </summary>
        /// <returns>
        /// The current token, as a string.
        /// </returns>
        public string GetCurrentToken() {
            return token;
        }

        protected string token;
    }
}

