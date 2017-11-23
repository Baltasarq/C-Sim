// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core {
	using System;
	using System.Diagnostics;
	using System.Collections.Generic;

	using Types;
	using Literals;
	using Opcodes;
	using Exceptions;
	using FunctionLibrary;

	/// <summary>
	/// Parses user input.
	/// </summary>
    public sealed class Parser {
		/// <summary>
		/// Initializes a new <see cref="Parser"/>.
		/// </summary>
		/// <param name="input">The user's input.</param>
		/// <param name="m">The machine the instruction will be parsed for.</param>
        public Parser(string input, Machine m)
		{
            var compareMode = StringComparison.InvariantCulture;
            
			// Remove trailing ';'
			input = input.Trim();
            while ( input.EndsWith( Reserved.LblEOL, compareMode ) )
    		{
				input = input.Remove( input.Length - 1 ); 
			}
            
            // Remove comments, if present
            int commentPos = input.IndexOf( Reserved.LblComment, compareMode );
            if ( commentPos > -1 ) {
                input = input.Substring( 0, commentPos );
            }

			this.Lexer = new Lexer( input );
			this.Opcodes = new List<Opcode>();
            this.Machine = m;
		}
        
        /// <summary>
        /// Gets the opcodes that resulted on parsing.
        /// </summary>
        /// <returns>The opcodes, as an array.</returns>
        public Opcode[] GetOpcodes()
        {
            return this.Opcodes.ToArray();
        }

		/// <summary>
		/// Parsing starts here.
		/// </summary>
		public Opcode[] Parse()
		{
			Lexer.TokenType tokenType;
            
            this.Lexer.SkipSpaces();
            
            if ( this.Lexer.IsEOL() ) {
                goto End;
            }
            
			if ( this.Lexer.GetCurrentChar() == Reserved.OpAccess[ 0 ] ) {
                this.ParseAccessTo( considerAssign: true );
            } else {
				this.Lexer.SkipSpaces();
                tokenType = this.Lexer.GetNextTokenType();

    			if ( tokenType == Lexer.TokenType.Id ) {
					int oldPos = this.Lexer.Pos;
					string id = this.Lexer.GetToken();

                    this.Lexer.SkipSpaces();
                    bool isEOL = this.Lexer.IsEOL();
                    bool isLParen = ( this.Lexer.GetCurrentChar() == '(' );
                    this.Lexer.Pos = oldPos;
                    
                    if ( this.Machine.TypeSystem.IsBasicType( id )
                      && !isLParen )
                    {
                        this.ParseCreation();
                    }
                    else
                    if ( this.Machine.TDS.IsIdOfExistingVariable( id ) )
					{
                        if ( isEOL ) {
                            this.ParseIntermediateExpression();
                        } else {
                            this.ParseAssign();
                        }
                    }
                    else
                    if ( id == Reserved.OpDelete ) {
                        this.ParseDelete();
                    }
					else
					if ( this.Machine.API.Match( id ) != null ) {
						this.ParseFunctionCall();
					} else {
						throw new ParsingException( id + "?" );
					}
    			}
                else
                if ( Lexer.GetCurrentChar() == '('
                  || ( tokenType != Lexer.TokenType.Invalid
                    && tokenType != Lexer.TokenType.SpecialCharacter ) )
                {
                    this.ParseExpression();
                }
                else {
                    char currentChar = Lexer.GetCurrentChar();
                    string msg = L18n.Get( L18n.Id.ErrUnexpected ) + ": '";
                    
                    if ( currentChar == 0 ) {
                        msg += "'";
                    } else {
                        msg += ": '" + currentChar + "'";
                    }
                    
                    throw new ParsingException( msg );
                }
            }

            End:
            return this.Opcodes.ToArray();
        }        

		/// <summary>
		/// Parses a complex expression.
		/// i.e., (a + 4) * 2
		/// </summary>
		private void ParseExpression()
		{
			this.ParseIntermediateExpression();

			this.Lexer.SkipSpaces();
			if ( !this.Lexer.IsEOL() ) {
				Opcode opr = null;

				switch( this.Lexer.GetCurrentChar() ) {
				case '+':
					opr = new AddOpcode( this.Machine );
					break;
				case '-':
					opr = new SubOpcode( this.Machine );
					break;
				case '*':
					opr = new MulOpcode( this.Machine );
					break;
				case '/':
					opr = new DivOpcode( this.Machine );
					break;
				case '%':
					opr = new ModOpcode( this.Machine );
					break;
				default:
					// It is not a expression of this kind
					goto end;
				}

				this.Lexer.Advance();
				this.Lexer.SkipSpaces();
				this.ParseIntermediateExpression();
				this.Opcodes.Add( opr );
			}

			end:
			return;
		}
        
        /// <summary>
        /// Matches a type cast.
        /// </summary>
        /// <returns><c>true</c>, if a cast was matched next, <c>false</c> otherwise.</returns>
        private bool MatchTypeCast()
        {
            int posOrg = this.Lexer.Pos;
            bool toret = Primitive.IsPrimitive( this.Lexer.GetToken() );
            
            if ( toret ) {
                this.Lexer.SkipSpaces();
                toret = ( this.Lexer.GetCurrentChar() == '(' );
            }
            
            this.Lexer.Pos = posOrg;
            return toret;
        }

		/// <summary>
		/// Parses an intermediate expression, as in "*x", "(y + 2)" or "print(x)"
		/// </summary>
		private void ParseIntermediateExpression()
		{
			this.Lexer.SkipSpaces();
			int oldPos = this.Lexer.Pos;
			string token = this.Lexer.GetToken();
			this.Lexer.Pos = oldPos;
			char currentChar = this.Lexer.GetCurrentChar();

            // Is it 'int' (type literal) or 'int(x)' (type cast [function call]) ?
            if ( Primitive.IsPrimitive( token ) ) {
                if ( this.MatchTypeCast() ) {
                    this.Lexer.Pos = oldPos;
                    this.ParseFunctionCall();
                } else {
                    this.Lexer.Pos = oldPos;
                    this.ParseTerminal();
                }
            }
            else
			// Is it 'print(x)'?
			if ( this.Machine.API.Match( token ) != null )
			{
				this.Lexer.Pos = oldPos;
				this.ParseFunctionCall();
			}
			else
			// Is it '*x'?
			if ( currentChar == Reserved.OpAccess[ 0 ] ) {
				ParseAccessTo( considerAssign: false );
			}
			else
			// Is it '('?
			if ( currentChar == '(' ) {
				this.Lexer.Advance();
				this.ParseExpression();
				
				this.Lexer.SkipSpaces();
				if ( this.Lexer.GetCurrentChar() != ')' ) {
					throw new ParsingException( ")?" );
				}
				
				this.Lexer.Advance();
				this.Lexer.SkipSpaces();
			}
            else {
				ParseTerminal();
			}

			return;
		}
        
        /// <summary>
        /// Parses a primitive literal.
        /// </summary>
        private void ParsePrimitiveLiteral()
        {
            Lexer.TokenType tokenType = this.Lexer.GetNextTokenType();
            
            // Is it '0x5'?
            if ( tokenType == Lexer.TokenType.HexNumber ) {
                int negative = 1;
                char ch = this.Lexer.GetCurrentChar();

                if ( ch == '+'
                  || ch == '-' )
                {
                    if ( ch == '-' ) {
                        negative = -1;
                    }

                    this.Lexer.Advance();
                }

                this.Lexer.Advance(); // pass '0'
                this.Lexer.Advance(); // pass 'x'
                this.Opcodes.Add( new StoreRValue(
                            new IntLiteral( this.Machine,
                                            int.Parse( this.Lexer.GetHexNumber(),
                                                       System.Globalization.NumberStyles.HexNumber ) * negative ) ) );
            }
            else
            // Is it '5'?
            if ( tokenType == Lexer.TokenType.IntNumber ) {
                this.Opcodes.Add( new StoreRValue(
                                    new IntLiteral( this.Machine, int.Parse( this.Lexer.GetNumber() ) ) ) );
            }
            // Is it '5.0'?
            else
            if ( tokenType == Lexer.TokenType.RealNumber ) {
                double x = double.Parse( this.Lexer.GetToken(),
                                         System.Globalization.NumberFormatInfo.InvariantInfo );
                this.Opcodes.Add( new StoreRValue(
                        new DoubleLiteral( this.Machine, x ) ) );
            }
            // Is it 'a'?
            else
            if ( tokenType == Lexer.TokenType.Char ) {
                this.Lexer.Advance();
                this.Opcodes.Add( new StoreRValue(
                            new CharLiteral( this.Machine, this.Lexer.GetCurrentChar() ) ) );
                this.Lexer.Advance( 2 );
            }
            // Is it "hola"?
            else
            if ( tokenType == Lexer.TokenType.Text ) {
                string s = this.Lexer.GetStringLiteral();

                // Remove quotes
                if ( s[ 0 ] == '"' ) {
                    s = s.Substring( 1 );
                }

                if ( s[ s.Length - 1 ] == '"' )
                {
                    s = s.Substring( 0, s.Length - 1 );
                }
                
                // Store string literal
                this.Opcodes.Add( new StoreRValue( new StrLiteral( this.Machine, s ) ) );
            }
        }

		/// <summary>
		/// Parses a terminal, as in "x", "5"...
		/// </summary>
		private void ParseTerminal()
		{
			Lexer.TokenType tokenType = this.Lexer.GetNextTokenType();

			this.Lexer.SkipSpaces();
			int oldPos = this.Lexer.Pos;
			char currentChar = this.Lexer.GetCurrentChar();
			this.Lexer.Pos = oldPos;

			// Is it '&x'?
			if ( currentChar == Reserved.OpAddressOf[ 0 ] ) {
				ParseAddressOf();
			}
			else
            // Is it 'x' or 'int' (type_t)?
			if ( tokenType == Lexer.TokenType.Id ) {
                this.Lexer.SkipSpaces();
                int typeLitPos = this.Lexer.Pos;
				string strId = this.Lexer.GetToken();

                if ( this.Machine.TypeSystem.IsBasicType( strId ) ) {
                    this.Lexer.Pos = typeLitPos;
                    string strType = this.Lexer.GetTypeLiteral();
                    var typeLit = TypeLiteral.CreateFromString( this.Machine, strType );
                    this.Opcodes.Add( new StoreRValue( typeLit ) );
                }
                else
	            // Is it null?
	            if ( Reserved.IsNullId( strId ) ) {
	                this.Opcodes.Add(
                                new StoreRValue(
                                    new IntLiteral( this.Machine, 0 ) ) );
	            }
                else
				if ( strId == Reserved.OpNew ) {
					this.Lexer.Pos = oldPos;
					this.ParseNew();
				} else {
					this.Lexer.SkipSpaces();
					if ( this.Lexer.GetCurrentChar() == '[' ) {
							this.Lexer.Advance();
							ParseExpression();
							this.Lexer.SkipSpaces();

							if ( this.Lexer.GetCurrentChar() != ']' ) {
								throw new ParsingException( "]?" );
							}
							this.Lexer.Advance();
							this.Opcodes.Add( new StoreRValue( new Id( this.Machine, strId ) ) );
							this.Opcodes.Add( new ArrayIndexAccessOpcode( this.Machine ) );
					} else {
						this.Opcodes.Add( new StoreRValue( new Id( this.Machine, strId ) ) );
					}
				}
			} else {
                this.ParsePrimitiveLiteral();
            }

			return;
		}

		/// <summary>
		/// Parses the 'address of' expression.
		/// It can be as simple as: &amp;s, &amp;x, &amp;ptr.
		/// It can also be complex: &amp;s[5*foo()]
		/// </summary>
		private void ParseAddressOf()
		{
			// Pass '&'
			Debug.Assert( this.Lexer.GetCurrentChar() == Reserved.OpAddressOf[ 0 ] ) ;
			this.Lexer.Advance();
			this.Lexer.SkipSpaces();

			// Pass the id
			var id = new Id( this.Machine, this.Lexer.GetToken() );
			this.Lexer.SkipSpaces();

			// Is there '['?
			if ( this.Lexer.GetCurrentChar() == '[' ) {
				this.Lexer.Advance();
				this.Lexer.SkipSpaces();

				this.ParseExpression();
				this.Opcodes.Add( new StoreRValue( id ) );
				this.Opcodes.Add( new ArrayIndexAccessOpcode( this.Machine ) );

				this.Lexer.SkipSpaces();
				if ( this.Lexer.GetCurrentChar() == ']' ) {
					this.Lexer.Advance();
					this.Lexer.SkipSpaces();
				} else {
					throw new ParsingException( "]??" );
				}
			} else {
				this.Opcodes.Add( new StoreRValue( id ) );
			}

			this.Opcodes.Add( new AddressOfOpcode( this.Machine ) );
		}

		/// <summary>
		/// Parses the 'access to' expression.
		/// It can be as simple as: *ptr, *s...
		/// But it can also be complex: *v[0], *v[sin()*2]
        /// <param name="considerAssign">Maybe it is an expression...</param>
		/// </summary>
		private void ParseAccessTo(bool considerAssign)
		{
			char currentChar = this.Lexer.GetCurrentChar();
            int orgNumOpcodes = this.Opcodes.Count;
            int orgPos = this.Lexer.Pos;

			Debug.Assert( currentChar == Reserved.OpAccess[ 0 ] );
            
			this.Lexer.Advance();
			int levels = 1;

			this.Lexer.SkipSpaces();
			currentChar = this.Lexer.GetCurrentChar();

			while ( currentChar == Reserved.OpAccess[ 0 ] ) {
				this.Lexer.SkipSpaces();
				this.Lexer.Advance();
				++levels;
				currentChar = this.Lexer.GetCurrentChar();
			}
            
			this.ParseTerminal();
			this.Opcodes.Add( new AccessOpcode( this.Machine, levels ) );

            this.Lexer.SkipSpaces();
            bool isAssign =
                      ( this.Lexer.GetCurrentChar() == Reserved.OpAssign[ 0 ] );
            if ( isAssign
              && considerAssign )
            {
                this.Lexer.Pos = orgPos;
                this.Opcodes.RemoveRange( orgNumOpcodes,
                                            this.Opcodes.Count - orgNumOpcodes );
                this.ParseAssign();
            }
            
            return;
		}

		/// <summary>
		/// Parses expressions like 'delete ptr',
		/// where ptr must be a pointer variable.
		/// </summary>
		private void ParseDelete()
        {
			string token = this.Lexer.GetToken();

            if ( token == Reserved.OpDelete ) {
				this.Lexer.SkipSpaces();

				if ( this.Lexer.GetCurrentChar() == '[' ) {
					this.Lexer.Advance();
					this.Lexer.SkipSpaces();

					if ( this.Lexer.GetCurrentChar() == ']' ) {
						this.Lexer.Advance();
						this.Lexer.SkipSpaces();
                    }
                    else {
                        throw new ParsingException(
                            L18n.Get( L18n.Id.ErrExpected ) + ": " + "]" );
                    }
                }

				Lexer.TokenType tokenType = this.Lexer.GetNextTokenType();

                if ( tokenType == Lexer.TokenType.Id ) {
					this.ParseExpression();
					this.Opcodes.Add( new CallOpcode( this.Machine, Free.Name, 1 ) );
                } else {
                    throw new ParsingException(
                        L18n.Get( L18n.Id.ErrExpected ) + ": id." );
                }
            }
            else {
                throw new ParsingException(
                    L18n.Get( L18n.Id.ErrExpected ) + ": " + Reserved.OpDelete
                );
            }

            return;
        }

		/// <summary>
		/// Parses the 'new' operator, as in "new int;"
		/// </summary>
		private void ParseNew()
		{
			Lexer.SkipSpaces();
	
			if ( Lexer.GetToken() == Reserved.OpNew ) {
                var typeLit = TypeLiteral.CreateFromString( this.Machine, this.Lexer.GetTypeLiteral() );

				// Match square brackets -- is it a vector?
				if ( Lexer.GetCurrentChar() == '[' ) {
					Lexer.Advance();
					Lexer.SkipSpaces();
					this.ParseExpression();

					if ( Lexer.GetCurrentChar() != ']' ) {
						throw new ParsingException( "]?" );
					}

                    this.Opcodes.Add( new StoreRValue( typeLit ) );
					this.Opcodes.Add( new CallOpcode( this.Machine, TypedMalloc.Name, 2 ) );
				} else {
					var memBlkId = new Id( this.Machine, SymbolTable.GetNextMemoryBlockName() );
					this.Opcodes.Add( new CreateOpcode( this.Machine, memBlkId, typeLit.Value ) );
                    this.Opcodes.Add( new AddressOfOpcode( this.Machine ) );
					
					// Match brackets -- init?
					if ( Lexer.GetCurrentChar () == '(' ) {
						Lexer.Advance();
						Lexer.SkipSpaces();
                        this.Opcodes.Add( new AssignOpcode( this.Machine ) );
                        this.Opcodes.Add( new StoreRValue( memBlkId ) );
						this.ParseExpression();
						Lexer.SkipSpaces();

						if ( Lexer.GetCurrentChar() != ')' ) {
							throw new ParsingException( ")?" );
						}
					}
				}
			} else {
                throw new ParsingException(
                    L18n.Get( L18n.Id.ErrExpected ) + ": " + Reserved.OpNew );
			}

			return;
		}

		/// <summary>
		/// Parses variable assign.
		/// </summary>
		private void ParseAssign()
		{
            // Get the id
			this.ParseIntermediateExpression();

			// Parse expr after '='
			Lexer.SkipSpaces();
			if ( !Lexer.IsEOL() ) {
                if ( Lexer.GetCurrentChar() == Reserved.OpAssign[ 0 ] ) {
					Lexer.Advance();
					Lexer.SkipSpaces();

					ParseExpression();

					// Create assign opcode
					this.Opcodes.Add( new AssignOpcode( this.Machine ) );
				} else {
                    throw new RuntimeException(
                        L18n.Get( L18n.Id.ErrExpected ) + ": '='" );
				}
			} else {
                throw new ParsingException(
                    L18n.Get( L18n.Id.ErrUnexpected )
                                    + " " + L18n.Get( L18n.Id.ErrEOL ) );
			}

			return;
		}

		/// <summary>
		/// Parses the creation of a variable.
		/// </summary>
		private void ParseCreation()
		{
			int oldPos;
			Id id;
			string strId;
			AType t;

			// Get the type literal
			this.Lexer.SkipSpaces();
			strId = this.Lexer.GetTypeLiteral();
			t = this.Machine.TypeSystem.FromStringToType( strId );           

            // Get id
			Lexer.SkipSpaces();
			oldPos = Lexer.Pos;
            id = new Id( this.Machine, Lexer.GetToken() );

			// Create the creation opcode
			this.Opcodes.Add( new CreateOpcode( this.Machine, id, t ) );

			// Check whether there is an assign on creation
			Lexer.SkipSpaces();
			if ( !Lexer.IsEOL() ) {
                if ( Lexer.GetCurrentChar() == Reserved.OpAssign[ 0 ] ) {
					Lexer.Pos = oldPos;
					this.ParseAssign();
				} else {
                        throw new ParsingException(
                            L18n.Get( L18n.Id.ErrExpected )
                            + " " + L18n.Get( L18n.Id.ErrEOL ) );
                }
			} else {
                if ( t is Ref ) {
                    throw new ParsingException( L18n.Get( L18n.Id.ErrRefInit ) );
                }
            }

			return;
		}

		/// <summary>
		/// Parses a function call in the input.
		/// </summary>
		private void ParseFunctionCall()
		{
			string id;
            int numArgs = 0;

			Lexer.SkipSpaces();
			id = Lexer.GetToken().Trim();

			if ( id.Length > 0 ) {
				Lexer.SkipSpaces();

				if ( Lexer.GetCurrentChar() == '(' ) {
                    Lexer.Advance();
                    Lexer.SkipSpaces();

                    if ( Lexer.GetCurrentChar() != ')' ) {
                        do {
        					Lexer.SkipSpaces();
							this.ParseExpression();
                            ++numArgs;
                            Lexer.SkipSpaces();

                            if ( Lexer.GetCurrentChar() == ',' ) {
                                Lexer.Advance();
                                Lexer.SkipSpaces();
                            }
                            else
                            if ( Lexer.GetCurrentChar() != ')' ) {
                                throw new ParsingException(
                                    L18n.Get( L18n.Id.ErrExpected )
                                        + ": ',', ')'" 
                                );
                            }
                        } while( Lexer.GetCurrentChar() != ')'
                              && !Lexer.IsEOL() );
                    }

                    if ( Lexer.GetCurrentChar() != ')' ) {
                        throw new ParsingException( L18n.Get( L18n.Id.ErrExpectedParametersEnd ) );
                    }
                     
                    Lexer.Advance();
                    Lexer.SkipSpaces();
                    this.Opcodes.Add( new CallOpcode( this.Machine, id, numArgs ) );
				} else {
					throw new ParsingException(L18n.Get(  L18n.Id.ErrExpectedParametersBegin ) );
				}
			}

			return;
		}

		/// <summary>
		/// Gets the input this parser was created for.
		/// </summary>
		/// <value>The input, as a string.</value>
		public string Input {
			get { return this.Lexer.Line; }
		}

		/// <summary>
		/// Gets or sets the machine this parser was created for.
		/// </summary>
		/// <value>The <see cref="Machine"/> object.</value>
        public Machine Machine {
            get; set;
        }

		/// <summary>
		/// Gets the lexer associated to the parser.
		/// It is created by the parser itself.
		/// </summary>
		/// <value>The <see cref="Lexer"/>.</value>
		public Lexer Lexer {
			get; private set;
		}

		/// <summary>
		/// Gets or sets the opcodes resulting of the parsing.
		/// </summary>
		/// <value>The opcodes, as a <see cref="List{Opcode}"/>.</value>
		private List<Opcode> Opcodes {
			get; set;
		}
	}
}
