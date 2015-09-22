using System;
using System.Collections;
using System.Collections.Generic;
using CSim.Core.Literals;

using CSim.Core.Opcodes;
using CSim.Core.Exceptions;
using CSim.Core.Types.Primitives;
using CSim.Core.FunctionLibrary;
using CSim.Core.Types;

namespace CSim.Core {
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
                        this.parseDelete();
                    } else {
						lexer.Pos = oldPos;
						this.ParseFunctionCall();
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
			Type toret;
			this.lexer.SkipSpaces();

			// Is it '&x'?
			if ( this.lexer.GetCurrentChar() == Reserved.OpAddressOf[ 0 ] ) {
				this.lexer.Advance();
				this.opcodes.Add( new AddressOf( this.Machine, new Id( this.lexer.GetToken() ) ) );
				toret = this.Machine.TypeSystem.GetPtrType( Any.Get() );
			} else {
				toret = this.Machine.TypeSystem.GetIntType();
			}

			return toret;
		}

		/// <summary>
		/// Parses expressions like 'delete ptr',
		/// where ptr must be a pointer variable.
		/// </summary>
        private void parseDelete()
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
					this.opcodes.Add( new Call( this.Machine, Free.Name, new RValue[]{ ParseFinal() } ) );
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

        private void ParseNew(string id)
		{
			lexer.SkipSpaces();
	
			if ( lexer.GetToken() == Reserved.OpNew ) {
				Type type = this.Machine.TypeSystem.GetPrimitiveType( lexer.GetToken() );
                Id memBlkId = new Id( SymbolTable.GetNextMemoryBlockName() );

                // Create variable (memory block)
				this.opcodes.Add( new Create( this.Machine, memBlkId, this.Machine.TypeSystem.GetPtrType( type ) ) );

                // Make the vble "id" point to it
                this.opcodes.Add(
                    new ModifyWithVbleAddress( this.Machine, id, memBlkId.Value, 
                        ModificationOpcode.NotLeftPtrAccess ) );
			} else {
                throw new EngineException(
                    L18n.Get( L18n.Id.ErrExpected )
                    + ": " + Reserved.OpNew );
			}

			return;
		}

		private RValue ParseFinal()
		{
			RValue toret;
            bool isHex = false;
			int oldPos = lexer.Pos;

			// Hex?
            if ( lexer.GetCurrentChar() == '0' ) {
                lexer.Advance();
                if ( lexer.GetCurrentChar() == 'x' ) {
                    isHex = true;
                    lexer.Advance();
                } else {
                    lexer.Pos = oldPos;
                }
            }

			// Get the final type
			Lexer.TokenType tokenType = this.lexer.GetNextTokenType();

			if ( isHex
	          || tokenType == Lexer.TokenType.Number )
	        {
				string number = lexer.GetHexNumber();

				try {
					toret = new IntLiteral( this.Machine, Convert.ToInt32( number, isHex ? 16 : 10 ) );
				} catch(FormatException)
				{
					try {
						toret = new DoubleLiteral( this.Machine, Convert.ToDouble( number, System.Globalization.CultureInfo.InvariantCulture ) );
					} catch(FormatException)
					{
						throw new TypeMismatchException( L18n.Get( L18n.Id.ErrExpectedLiteralNumber ) +  ": " + number );
					}
				}
			}
			else
			if ( tokenType == Lexer.TokenType.Id ) {
				string strValue = lexer.GetToken();

				if ( Reserved.IsNullId( strValue ) )
				{
					toret = new IntLiteral( this.Machine, 0 );
				} else {
					toret = new Id( strValue );
				}
			}
			else
			if ( tokenType == Lexer.TokenType.Char ) {
                toret = new CharLiteral( this.Machine, lexer.GetToken()[ 1 ] );
			} else  {
				throw new TypeMismatchException( L18n.Get( L18n.Id.ErrExpectedLiteralOrId ) );
			}

			return toret;
		}

		/// <summary>
		/// Parses the assignment of a variable.
		/// </summary>
		private void ParseAssign()
		{
			string id;
			string token;
			int oldPos;
			bool isPtrLeft = false;

            // Get the id
            if ( lexer.GetCurrentChar() == Reserved.OpAccess[ 0 ] ) {
                isPtrLeft = true;
                lexer.Advance();
                lexer.SkipSpaces();
            }

            id = lexer.GetToken().Trim();

            // Parse expr after '='
			lexer.SkipSpaces();
			if ( !lexer.IsEOL() ) {

                if ( lexer.GetCurrentChar() == Reserved.OpAssign[ 0 ] ) {
					lexer.Advance();
					lexer.SkipSpaces();
					oldPos = lexer.Pos;

					// Modifiers
					token = lexer.GetToken();
					lexer.Pos = oldPos;

					if ( token == Reserved.OpNew ) {
						if ( isPtrLeft ) {
                            throw new EngineException( L18n.Get( L18n.Id.ErrBadUseStar ) );
						}

						ParseNew( id );
					}
					else {
						ParseExpression( /* id, isPtrLeft*/ );
					}
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
            // Is it an ampersand there?
                if ( lexer.GetCurrentChar() == Reserved.OpAddressOf[ 0 ] ) {
                isRef = true;
                lexer.Advance();
            }

            // Get id
			lexer.SkipSpaces();
			oldPos = lexer.Pos;
            id = new Id( lexer.GetToken() );

            if ( isRef ) {
				this.opcodes.Add( new Create( this.Machine, id,
				                              this.Machine.TypeSystem.GetRefType(
												this.Machine.TypeSystem.GetPrimitiveType( typeId ) ) ) );
            } else {
                if ( isPtr ) {
					this.opcodes.Add( new Create( this.Machine, id,
					                             this.Machine.TypeSystem.GetPtrType(
													this.Machine.TypeSystem.GetPrimitiveType( typeId ) ) ) );
                } else {
					this.opcodes.Add( new Create( this.Machine, id,
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
            List<RValue> realArgs = new List<RValue>();

			lexer.SkipSpaces();
			id = lexer.GetToken().Trim();

			if ( id.Length > 0 ) {
				lexer.SkipSpaces();

				if ( lexer.GetCurrentChar() == '(' ) {
                    lexer.Advance();

                    if ( lexer.GetCurrentChar() != ')' ) {
                        do {
        					lexer.SkipSpaces();
                            realArgs.Add( this.ParseFinal() );
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
                    this.opcodes.Add( new Call( this.Machine, id, realArgs.ToArray() ) );
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

