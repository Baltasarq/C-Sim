
namespace CSim.Core {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using CSim.Core.Literals;

	using CSim.Core.Opcodes;
	using CSim.Core.Exceptions;
	using CSim.Core.Types.Primitives;
	using CSim.Core.FunctionLibrary;
	using CSim.Core.Types;

	/// <summary>
	/// Parses user input.
	/// </summary>
    public class Parser {
        public Parser(string input, Machine m)
		{
			// Remove trailing ';'
			input = input.Trim();
            while ( input.EndsWith( Reserved.LblEOL, StringComparison.InvariantCulture ) )
    		{
				input = input.Remove( input.Length -1 ); 
			}

			this.lexer = new Lexer( input );
			this.opcodes = new List<Opcode>();
            this.Machine = m;
			this.tds = this.Machine.TDS;
		}

		/// <summary>
		/// Parsing starts here.
		/// </summary>
		public Opcode[] Parse()
		{
			Lexer.TokenType tokenType;

            if ( lexer.GetCurrentChar() == Reserved.OpAccess[ 0 ] ) {
	            this.ParseAssign();
            } else {
                tokenType = this.lexer.GetNextTokenType();

    			if ( tokenType == Lexer.TokenType.Id ) {
    				int oldPos = lexer.Pos;
    				string id = lexer.GetToken();

                    if ( this.Machine.TypeSystem.IsPrimitiveType( id ) ) {
                        this.ParseCreation( id );
                    }
                    else
                    if ( this.tds.IsIdOfExistingVariable( new Id( id ) ) ) {
                        lexer.Pos = oldPos;
                        this.ParseAssign();
                    }
                    else
                    if ( id == Reserved.OpDelete ) {
                        lexer.Pos = oldPos;
                        this.ParseDelete();
                    }
					else
					if ( this.Machine.Api.Match( id ) != null ) {
						lexer.Pos = oldPos;
						this.ParseFunctionCall();
					} else {
						throw new ParsingException( id + "?" );
					}
    			}
            }

            return this.opcodes.ToArray();
        }

		/// <summary>
		/// Parses a complex expression.
		/// i.e., a * 2
		/// </summary>
		private Type ParseExpression()
		{
			Type toret = Any.Get();

			this.lexer.SkipSpaces();
			int oldPos = lexer.Pos;
			string token = this.lexer.GetToken();
			lexer.Pos = oldPos;
			char currentChar = this.lexer.GetCurrentChar();

			// Is it 'print(x)'?
			if ( this.Machine.Api.Match( token ) != null )
			{
				lexer.Pos = oldPos;
				this.ParseFunctionCall();
				toret = this.Machine.TypeSystem.GetPtrType( Any.Get() );
			}
			else
			// Is it '*x'?
			if ( currentChar == Reserved.OpAccess[ 0 ] ) {
				this.lexer.Advance();
				int levels = 1;
				
				currentChar = this.lexer.GetCurrentChar();

				while ( currentChar == Reserved.OpAccess[ 0 ] ) {
					this.lexer.SkipSpaces();
					this.lexer.Advance();
					++levels;
					currentChar = this.lexer.GetCurrentChar();
				}
				this.ParseExpression();
				this.opcodes.Add( new AccessOpcode( this.Machine, levels ) );
				toret = this.Machine.TypeSystem.GetPtrType( Any.Get() );
			}
			else {
				ParseTerminal();
			}

			return toret;
		}

		private Type ParseTerminal()
		{
			Type toret = Any.Get();
			Lexer.TokenType tokenType = this.lexer.GetNextTokenType();

			this.lexer.SkipSpaces();
			int oldPos = lexer.Pos;
			char currentChar = this.lexer.GetCurrentChar();
			lexer.Pos = oldPos;

			// Is it '&x'?
			if ( currentChar == Reserved.OpAddressOf[ 0 ] ) {
				this.lexer.Advance();
				this.opcodes.Add( new AddressOpcode( this.Machine, new Id( this.lexer.GetToken() ) ) );
				toret = this.Machine.TypeSystem.GetPtrType( Any.Get() );
			}
			else
			// Is it 'x'?
			if ( tokenType == Lexer.TokenType.Id ) {
				string id = this.lexer.GetToken();

				if ( id == Reserved.OpNew ) {
					this.lexer.Pos = oldPos;
					this.ParseNew();
				} else {
					this.lexer.SkipSpaces();
					if ( this.lexer.GetCurrentChar() == '[' ) {
							this.lexer.Advance();
							ParseExpression();
							this.lexer.SkipSpaces();

							if ( this.lexer.GetCurrentChar() != ']' ) {
								throw new ParsingException( "]?" );
							}
							this.lexer.Advance();
							this.opcodes.Add( new StoreRValue( this.Machine, new Id( id ) ) );
							this.opcodes.Add( new ArrayIndexAccessOpcode( this.Machine ) );
					} else {
						this.opcodes.Add( new StoreRValue( this.Machine, new Id( id ) ) );
					}
				}

				try {
					Variable vble = this.Machine.TDS.LookUp( id );
					toret = vble.Type;
				} catch(UnknownVbleException) { /* ignored */ }
			}
			else
			// Is it '0x5'?
			if ( tokenType == Lexer.TokenType.HexNumber ) {
				int negative = 1;
				char ch = this.lexer.GetCurrentChar();

				if ( ch == '+'
				  || ch == '-' )
				{
					if ( ch == '-' ) {
						negative = -1;
					}

					this.lexer.Advance();
				}

				this.lexer.Advance(); // pass '0'
				this.lexer.Advance(); // pass 'x'
				this.opcodes.Add( new StoreRValue( this.Machine,
							new IntLiteral( this.Machine,
								            int.Parse( this.lexer.GetHexNumber(),
									                   System.Globalization.NumberStyles.HexNumber ) * negative ) ) );
				toret = this.Machine.TypeSystem.GetIntType();
			}
			else
			// Is it '5'?
			if ( tokenType == Lexer.TokenType.IntNumber ) {
				this.opcodes.Add( new StoreRValue( this.Machine,
									new IntLiteral( this.Machine, int.Parse( this.lexer.GetNumber() ) ) ) );
				toret = this.Machine.TypeSystem.GetIntType();
			}
			// Is it '5.0'?
			else
			if ( tokenType == Lexer.TokenType.RealNumber ) {
				toret = this.Machine.TypeSystem.GetDoubleType();
				double x = double.Parse( this.lexer.GetToken(),
										 System.Globalization.NumberFormatInfo.InvariantInfo );
				this.opcodes.Add( new StoreRValue( this.Machine,
						new DoubleLiteral( this.Machine, x ) ) );
			}
			// Is it 'a'?
			else
			if ( tokenType == Lexer.TokenType.Char ) {
				toret = this.Machine.TypeSystem.GetCharType();
				this.lexer.Advance();
				this.opcodes.Add( new StoreRValue( this.Machine,
							new CharLiteral( this.Machine, this.lexer.GetCurrentChar() ) ) );
				this.lexer.Advance( 2 );
			}
			// Is it "hola"?
			else
			if ( tokenType == Lexer.TokenType.Text ) {
				toret = this.Machine.TypeSystem.GetPtrType( this.Machine.TypeSystem.GetCharType() );
				string s = this.lexer.GetStringLiteral();

				// Remove quotes
				if ( s[ 0 ] == '"' ) {
					s = s.Substring( 1 );
				}

				if ( s[ s.Length - 1 ] == '"' )
				{
					s = s.Substring( 0, s.Length - 1 );
				}
				
				// Store string literal
				this.opcodes.Add( new StoreRValue( this.Machine, new StrLiteral( this.Machine, s ) ) );
			}

			return toret;
		}

		/// <summary>
		/// Parses expressions like 'delete ptr',
		/// where ptr must be a pointer variable.
		/// </summary>
        private void ParseDelete()
        {
            string token = lexer.GetToken();

            if ( token == Reserved.OpDelete ) {
                lexer.SkipSpaces();

                if ( lexer.GetCurrentChar() == '[' ) {
                    lexer.Advance();
                    lexer.SkipSpaces();

                    if ( lexer.GetCurrentChar() == ']' ) {
                        lexer.Advance();
                        lexer.SkipSpaces();
                    }
                    else {
                        throw new EngineException(
                            L18n.Get( L18n.Id.ErrExpected )
                            + ": " + "]" );
                    }
                }

                Lexer.TokenType tokenType = lexer.GetNextTokenType();

                if ( tokenType == Lexer.TokenType.Id ) {
                    token = lexer.GetToken();
					this.ParseExpression();
					this.opcodes.Add( new CallOpcode( this.Machine, Free.Name ) );
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

        private void ParseNew()
		{
			bool isVector = false;
			lexer.SkipSpaces();
	
			if ( lexer.GetToken() == Reserved.OpNew ) {
				Type type = this.Machine.TypeSystem.GetPrimitiveType( lexer.GetToken() );
                Id memBlkId = new Id( SymbolTable.GetNextMemoryBlockName() );

				// Match square brackets
				if ( lexer.GetCurrentChar() == '[' ) {
					isVector = true;
					lexer.Advance();
					lexer.SkipSpaces();
					this.ParseExpression();
					lexer.SkipSpaces();

					if ( lexer.GetCurrentChar() != ']' ) {
						throw new ParsingException( "]?" );
					}
				}

                // Create variable (memory block)
				if ( isVector ) {
					this.opcodes.Add( new CallOpcode( this.Machine, Malloc.Name ) );
				} else {
					this.opcodes.Add( new CreateOpcode( this.Machine, memBlkId, type ) );
				}

                // Make the vble "id" point to it
				this.opcodes.Add( new AddressOpcode( this.Machine, memBlkId ) );
				this.opcodes.Add( new AssignOpcode( this.Machine ) );
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
		private void ParseAssign()
		{
            // Get the id
			ParseExpression();

			// Parse expr after '='
			lexer.SkipSpaces();
			if ( !lexer.IsEOL() ) {
                if ( lexer.GetCurrentChar() == Reserved.OpAssign[ 0 ] ) {
					lexer.Advance();
					lexer.SkipSpaces();

					ParseExpression();

					// Create assign opcode
					this.opcodes.Add( new AssignOpcode( this.Machine ) );
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
		private void ParseCreation(string typeId)
		{
			int oldPos;
			Id id;
            bool isPtr = false;
            bool isRef = false;

			lexer.SkipSpaces();

            // Is it a star over there?
            if ( lexer.GetCurrentChar() == Reserved.OpAccess[ 0 ] ) {
                isPtr = true;
                lexer.Advance();
            }
			else
            // Is there an ampersand there?
                if ( lexer.GetCurrentChar() == Reserved.OpAddressOf[ 0 ] ) {
                isRef = true;
                lexer.Advance();
            }

            // Get id
			lexer.SkipSpaces();
			oldPos = lexer.Pos;
            id = new Id( lexer.GetToken() );

            if ( isRef ) {
				this.opcodes.Add( new CreateOpcode( this.Machine, id,
				                              this.Machine.TypeSystem.GetRefType(
												this.Machine.TypeSystem.GetPrimitiveType( typeId ) ) ) );
            } else {
                if ( isPtr ) {
					this.opcodes.Add( new CreateOpcode( this.Machine, id,
					                             this.Machine.TypeSystem.GetPtrType(
													this.Machine.TypeSystem.GetPrimitiveType( typeId ) ) ) );
                } else {
					this.opcodes.Add( new CreateOpcode( this.Machine, id,
					                              this.Machine.TypeSystem.GetPrimitiveType( typeId ) ) );
                }
            }

			// Check whether there is an assign on creation
			lexer.SkipSpaces();
			if ( !lexer.IsEOL() ) {
                if ( lexer.GetCurrentChar() == Reserved.OpAssign[ 0 ] ) {
					lexer.Pos = oldPos;
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
		public void ParseFunctionCall()
		{
			string id;

			lexer.SkipSpaces();
			id = lexer.GetToken().Trim();

			if ( id.Length > 0 ) {
				lexer.SkipSpaces();

				if ( lexer.GetCurrentChar() == '(' ) {
                    lexer.Advance();

                    if ( lexer.GetCurrentChar() != ')' ) {
                        do {
        					lexer.SkipSpaces();
							this.ParseExpression();
                            lexer.SkipSpaces();

                            if ( lexer.GetCurrentChar() == ',' ) {
                                lexer.Advance();
                                lexer.SkipSpaces();
                            }
                            else
                            if ( lexer.GetCurrentChar() != ')' ) {
                                throw new ParsingException(
                                    L18n.Get( L18n.Id.ErrExpected )
                                        + ": ',', ')'" 
                                );
                            }
                        } while( lexer.GetCurrentChar() != ')'
                              && !lexer.IsEOL() );
                    }

                    if ( lexer.GetCurrentChar() != ')' ) {
                        throw new ParsingException( L18n.Get( L18n.Id.ErrExpectedParametersEnd ) );
                    }
                     
                    lexer.Advance();
                    lexer.SkipSpaces();
                    this.opcodes.Add( new CallOpcode( this.Machine, id ) );
				} else {
					throw new ParsingException(L18n.Get(  L18n.Id.ErrExpectedParametersBegin ) );
				}
			}

			return;
		}

		public string Input {
			get { return this.lexer.Line; }
		}

        public Machine Machine {
            get; set;
        }

		private Lexer lexer = null;
		private List<Opcode> opcodes = null;
		private SymbolTable tds = null;
	}
}
