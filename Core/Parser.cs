
namespace CSim.Core {
	using System;
	using System.Diagnostics;
	using System.Collections.Generic;

	using CSim.Core.Types;
	using CSim.Core.Literals;
	using CSim.Core.Opcodes;
	using CSim.Core.Exceptions;
	using CSim.Core.FunctionLibrary;

	/// <summary>
	/// Parses user input.
	/// </summary>
    public class Parser {
		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Parser"/> class.
		/// </summary>
		/// <param name="input">The user's input.</param>
		/// <param name="m">The machine the instruction will be parsed for.</param>
        public Parser(string input, Machine m)
		{
			// Remove trailing ';'
			input = input.Trim();
            while ( input.EndsWith( Reserved.LblEOL, StringComparison.InvariantCulture ) )
    		{
				input = input.Remove( input.Length -1 ); 
			}

			this.Lexer = new Lexer( input );
			this.Opcodes = new List<Opcode>();
            this.Machine = m;
		}

		/// <summary>
		/// Parsing starts here.
		/// </summary>
		public virtual Opcode[] Parse()
		{
			Lexer.TokenType tokenType;

			if ( this.Lexer.GetCurrentChar() == Reserved.OpAccess[ 0 ] ) {
	            this.ParseAssign();
            } else {
                tokenType = this.Lexer.GetNextTokenType();

    			if ( tokenType == Lexer.TokenType.Id ) {
					int oldPos = this.Lexer.Pos;
					string id = this.Lexer.GetToken();

                    if ( this.Machine.TypeSystem.IsPrimitiveType( id ) ) {
                        this.ParseCreation( id );
                    }
                    else
                    if ( this.Machine.TDS.IsIdOfExistingVariable( new Id( id ) ) )
					{
						this.Lexer.Pos = oldPos;
                        this.ParseAssign();
                    }
                    else
                    if ( id == Reserved.OpDelete ) {
						this.Lexer.Pos = oldPos;
                        this.ParseDelete();
                    }
					else
					if ( this.Machine.API.Match( id ) != null ) {
						this.Lexer.Pos = oldPos;
						this.ParseFunctionCall();
					} else {
						throw new ParsingException( id + "?" );
					}
    			}
            }

            return this.Opcodes.ToArray();
        }

		/// <summary>
		/// Parses a complex expression.
		/// i.e., a * 2
		/// </summary>
		protected void ParseExpression()
		{
			ParseIntermediateExpression();

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
				ParseIntermediateExpression();
				this.Opcodes.Add( opr );
			}

			end:
			return;
		}

		/// <summary>
		/// Parses an intermediate expression, as in "*x", "(y + 2)" or "print(x)"
		/// </summary>
		protected void ParseIntermediateExpression()
		{
			this.Lexer.SkipSpaces();
			int oldPos = this.Lexer.Pos;
			string token = this.Lexer.GetToken();
			this.Lexer.Pos = oldPos;
			char currentChar = this.Lexer.GetCurrentChar();

			// Is it 'print(x)'?
			if ( this.Machine.API.Match( token ) != null )
			{
				this.Lexer.Pos = oldPos;
				this.ParseFunctionCall();
			}
			else
			// Is it '*x'?
			if ( currentChar == Reserved.OpAccess[ 0 ] ) {
				parseAccessTo();
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
		/// Parses a terminal, as in "x", "5"...
		/// </summary>
		protected void ParseTerminal()
		{
			Lexer.TokenType tokenType = this.Lexer.GetNextTokenType();

			this.Lexer.SkipSpaces();
			int oldPos = this.Lexer.Pos;
			char currentChar = this.Lexer.GetCurrentChar();
			this.Lexer.Pos = oldPos;

			// Is it '&x'?
			if ( currentChar == Reserved.OpAddressOf[ 0 ] ) {
				parseAddressOf();
			}
			else
			// Is it 'x'?
			if ( tokenType == Lexer.TokenType.Id ) {
				string id = this.Lexer.GetToken();

				if ( id == Reserved.OpNew ) {
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
							this.Opcodes.Add( new StoreRValue( this.Machine, new Id( id ) ) );
							this.Opcodes.Add( new ArrayIndexAccessOpcode( this.Machine ) );
					} else {
						this.Opcodes.Add( new StoreRValue( this.Machine, new Id( id ) ) );
					}
				}
			}
			else
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
				this.Opcodes.Add( new StoreRValue( this.Machine,
							new IntLiteral( this.Machine,
								            int.Parse( this.Lexer.GetHexNumber(),
									                   System.Globalization.NumberStyles.HexNumber ) * negative ) ) );
			}
			else
			// Is it '5'?
			if ( tokenType == Lexer.TokenType.IntNumber ) {
				this.Opcodes.Add( new StoreRValue( this.Machine,
									new IntLiteral( this.Machine, int.Parse( this.Lexer.GetNumber() ) ) ) );
			}
			// Is it '5.0'?
			else
			if ( tokenType == Lexer.TokenType.RealNumber ) {
				double x = double.Parse( this.Lexer.GetToken(),
										 System.Globalization.NumberFormatInfo.InvariantInfo );
				this.Opcodes.Add( new StoreRValue( this.Machine,
						new DoubleLiteral( this.Machine, x ) ) );
			}
			// Is it 'a'?
			else
			if ( tokenType == Lexer.TokenType.Char ) {
				this.Lexer.Advance();
				this.Opcodes.Add( new StoreRValue( this.Machine,
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
				this.Opcodes.Add( new StoreRValue( this.Machine, new StrLiteral( this.Machine, s ) ) );
			}

			return;
		}

		/// <summary>
		/// Parses the 'address of' expression.
		/// It can be as simple as: &amp;s, &amp;x, &amp;ptr.
		/// It can also be complex: &amp;s[5*foo()]
		/// </summary>
		protected void parseAddressOf()
		{
			// Pass '&'
			Debug.Assert( this.Lexer.GetCurrentChar() == Reserved.OpAddressOf[ 0 ] ) ;
			this.Lexer.Advance();
			this.Lexer.SkipSpaces();

			// Pass the id
			var id = new Id( this.Lexer.GetToken() );
			this.Lexer.SkipSpaces();

			// Is there '['?
			if ( this.Lexer.GetCurrentChar() == '[' ) {
				this.Lexer.Advance();
				this.Lexer.SkipSpaces();

				this.ParseExpression();
				this.Opcodes.Add( new StoreRValue( this.Machine, id ) );
				this.Opcodes.Add( new ArrayIndexAccessOpcode( this.Machine ) );

				this.Lexer.SkipSpaces();
				if ( this.Lexer.GetCurrentChar() == ']' ) {
					this.Lexer.Advance();
					this.Lexer.SkipSpaces();
				} else {
					throw new ParsingException( "]??" );
				}
			} else {
				this.Opcodes.Add( new StoreRValue( this.Machine, id ) );
			}

			this.Opcodes.Add( new AddressOfOpcode( this.Machine ) );
		}

		/// <summary>
		/// Parses the 'access to' expression.
		/// It can be as simple as: *ptr, *s...
		/// But it can also be complex: *v[0], *v[sin()*2]
		/// </summary>
		protected void parseAccessTo()
		{
			char currentChar = this.Lexer.GetCurrentChar();

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

			this.ParseExpression();
			this.Opcodes.Add( new AccessOpcode( this.Machine, levels ) );
		}

		/// <summary>
		/// Parses expressions like 'delete ptr',
		/// where ptr must be a pointer variable.
		/// </summary>
		protected void ParseDelete()
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
                        throw new EngineException(
                            L18n.Get( L18n.Id.ErrExpected )
                            + ": " + "]" );
                    }
                }

				Lexer.TokenType tokenType = this.Lexer.GetNextTokenType();

                if ( tokenType == Lexer.TokenType.Id ) {
					this.ParseExpression();
					this.Opcodes.Add( new CallOpcode( this.Machine, Free.Name ) );
                } else {
                    throw new EngineException(
                        L18n.Get( L18n.Id.ErrExpected )
                        + ": id." );
                }
            }
            else {
                throw new EngineException(
                    L18n.Get( L18n.Id.ErrExpected )
                    + ": " + Reserved.OpDelete
                );
            }

            return;
        }

		/// <summary>
		/// Parses the 'new' operator, as in "new int;"
		/// </summary>
		protected void ParseNew()
		{
			Lexer.SkipSpaces();
	
			if ( Lexer.GetToken() == Reserved.OpNew ) {
				// Get the type to reserve
				string strType = this.Lexer.GetToken();

				char ch = this.Lexer.GetCurrentChar();
				while( ch == Ref.RefTypeNamePart[ 0 ]
				    || ch == Ptr.PtrTypeNamePart[ 0 ] )
				{
					strType += ch;
					this.Lexer.Advance();
					ch = this.Lexer.GetCurrentChar();
				}

				Type type = this.Machine.TypeSystem.FromStringToType( strType );

				// Match square brackets -- is it a vector?
				if ( Lexer.GetCurrentChar() == '[' ) {
					Lexer.Advance();
					Lexer.SkipSpaces();
					this.ParseExpression();
					this.Opcodes.Add( new StoreRValue( this.Machine, new IntLiteral( this.Machine, type.Size ) ) );
					this.Opcodes.Add( new MulOpcode( this.Machine ) );
					Lexer.SkipSpaces();

					if ( Lexer.GetCurrentChar() != ']' ) {
						throw new ParsingException( "]?" );
					}

					this.Opcodes.Add( new CallOpcode( this.Machine, Malloc.Name ) );
				} else {
					var memBlkId = new Id( SymbolTable.GetNextMemoryBlockName() );
					this.Opcodes.Add( new CreateOpcode( this.Machine, memBlkId, type ) );
                    this.Opcodes.Add( new AddressOfOpcode( this.Machine ) );
					
					// Match brackets -- init?
					if ( Lexer.GetCurrentChar () == '(' ) {
						Lexer.Advance();
						Lexer.SkipSpaces();
                        this.Opcodes.Add( new AssignOpcode( this.Machine ) );
                        this.Opcodes.Add( new StoreRValue( this.Machine, memBlkId ) );
						this.ParseExpression();
						Lexer.SkipSpaces();

						if ( Lexer.GetCurrentChar() != ')' ) {
							throw new ParsingException( ")?" );
						}
					}
				}
			} else {
                throw new EngineException(
                    L18n.Get( L18n.Id.ErrExpected )
                    + ": " + Reserved.OpNew );
			}

			return;
		}

		/// <summary>
		/// Parses variable assign.
		/// </summary>
		protected void ParseAssign()
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
                    throw new EngineException(
                        L18n.Get( L18n.Id.ErrExpected )
                        + ": '='" );
				}
			} else {
                throw new EngineException(
                    L18n.Get( L18n.Id.ErrUnexpected )
                    + " " + L18n.Get( L18n.Id.ErrEOL ) );
			}

			return;
		}

		/// <summary>
		/// Parses the creation of a variable.
		/// </summary>
		protected void ParseCreation(string typeId)
		{
			int oldPos;
			Id id;
            int ptrLevel = 0;
            bool isRef = false;

			Lexer.SkipSpaces();

            // Is it a star over there?
            while ( Lexer.GetCurrentChar() == Reserved.OpAccess[ 0 ] ) {
				++ptrLevel;
                Lexer.Advance();
				Lexer.SkipSpaces();
            }

            // Is there an ampersand there?
            if ( Lexer.GetCurrentChar() == Reserved.OpAddressOf[ 0 ] ) {
                isRef = true;
                Lexer.Advance();
            }

            // Get id
			Lexer.SkipSpaces();
			oldPos = Lexer.Pos;
            id = new Id( Lexer.GetToken() );

            if ( isRef ) {
				this.Opcodes.Add( new CreateOpcode( this.Machine, id,
	                              	this.Machine.TypeSystem.GetRefType(
										this.Machine.TypeSystem.GetPrimitiveType( typeId ) ) ) );
            } else {
				if ( ptrLevel > 0 ) {
					this.Opcodes.Add(
						new CreateOpcode( this.Machine, id,
					    	this.Machine.TypeSystem.GetPtrType(
								this.Machine.TypeSystem.GetPrimitiveType( typeId ),
								ptrLevel ) )
					);
                } else {
					this.Opcodes.Add( new CreateOpcode( this.Machine, id,
					                  	this.Machine.TypeSystem.GetPrimitiveType( typeId ) ) );
                }
            }

			// Check whether there is an assign on creation
			Lexer.SkipSpaces();
			if ( !Lexer.IsEOL() ) {
                if ( Lexer.GetCurrentChar() == Reserved.OpAssign[ 0 ] ) {
					Lexer.Pos = oldPos;
					this.ParseAssign();
				} else {
                        throw new EngineException(
                            L18n.Get( L18n.Id.ErrExpected )
                            + " " + L18n.Get( L18n.Id.ErrEOL ) );
                }
			} else {
                if ( isRef ) {
                    throw new EngineException( L18n.Get( L18n.Id.ErrRefInit ) );
                }
            }

			return;
		}

		/// <summary>
		/// Parses a function call in the input.
		/// </summary>
		protected void ParseFunctionCall()
		{
			string id;

			Lexer.SkipSpaces();
			id = Lexer.GetToken().Trim();

			if ( id.Length > 0 ) {
				Lexer.SkipSpaces();

				if ( Lexer.GetCurrentChar() == '(' ) {
                    Lexer.Advance();

                    if ( Lexer.GetCurrentChar() != ')' ) {
                        do {
        					Lexer.SkipSpaces();
							this.ParseExpression();
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
                    this.Opcodes.Add( new CallOpcode( this.Machine, id ) );
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
			get; protected set;
		}

		/// <summary>
		/// Gets or sets the opcodes resulting of the parsing.
		/// </summary>
		/// <value>The opcodes, as a <see cref="System.Collections.Generic.List{Opcode}"/>.</value>
		protected List<Opcode> Opcodes {
			get; set;
		}
	}
}
