
namespace CSim.Core.Opcodes {
	using CSim.Core;
	using CSim.Core.Types;
	using CSim.Core.Variables;
	using CSim.Core.Exceptions;
	using CSim.Core.Literals;

	/// <summary>
	/// Assign opcode.
	/// </summary>
    public class AssignOpcode: Opcode {
		/// <summary>The opcode id.</summary>
        public static char OpcodeValue = (char) 0xE1;
        
		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Opcodes.AssignOpcode"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/> this opcode will be executed in.</param>
        public AssignOpcode(Machine m)
            : base( m )
        {
			this.Value = null;
        }

		private void SetRef(RefVariable lvalue, Variable rvalue)
		{
			// Check rvalue
			if ( rvalue is NoPlaceTempVariable ) {
				throw new UnknownVbleException( string.Format(
							"[{0}]",
					new IntLiteral( this.Machine, rvalue.Address ).ToPrettyNumber() ) );
			}

			var rvalueTypeAsRef = rvalue.Type as Ref;

			// Assign to variable
			if ( rvalueTypeAsRef != null ) {
				long targetVbleAddress = rvalue.LiteralValue.GetValueAsInt();
				lvalue.PointedVble = this.Machine.TDS.LookForAddress( targetVbleAddress );

				if ( lvalue.PointedVble == null ) {
					throw new UnknownVbleException( string.Format(
						"[{0}]",
						new IntLiteral( this.Machine, targetVbleAddress ).ToPrettyNumber() ) );
				}
			} else {
				lvalue.PointedVble = rvalue;
			}

			// Chk
			this.Machine.Memory.CheckSizeFits( lvalue.PointedVble.Address, lvalue.AssociatedType.Size );

			if ( lvalue.PointedVble.Type != rvalue.Type ) {
				throw new TypeMismatchException(
					lvalue.PointedVble.Type
					+ " != "
					+ rvalue.Type
				);
			}

			return;
		}

		/// <summary>
		/// Execute the assignment.
		/// </summary>
        public override void Execute()
		{
			// Take value
			this.Value = this.Machine.ExecutionStack.Pop();
			Variable rvalue = this.Machine.TDS.SolveToVariable( this.Value );

			// Take variable
			this.Vble = this.Machine.TDS.SolveToVariable( this.Machine.ExecutionStack.Pop() );

			if ( !( rvalue is NoPlaceTempVariable )
			  && rvalue.Name.Name == this.Vble.Name.Name )
			{
				// The variable has just been created.
				this.Value = this.Machine.ExecutionStack.Pop();
			}

			// Prepare assign parts
			if ( this.Vble is NoPlaceTempVariable ) {
				throw new UnknownVbleException( "temp vble: " + this.Vble.Name.Name + "??" );
			}

			Variable toret = this.Vble;

			// Chk types
			if ( !toret.Type.IsCompatibleWith( rvalue.Type ) ) {
				throw new TypeMismatchException(
						rvalue.Name.Name + ": "
						+ rvalue.Type + " != "
						+ toret.Type
					);
			}

			// Is lvalue a ref?
			var r = toret as RefVariable;

			if ( r != null ) {
				if ( !r.IsSet() ) {
					this.SetRef( r, rvalue );
					toret = r.PointedVble;
				} else {
					toret = r.PointedVble;
					toret.LiteralValue = rvalue.LiteralValue;
				}
			} else {
				if ( this.Value is StrLiteral ) {
					var s = (string) rvalue.LiteralValue.Value;

					Variable mblock = this.Machine.TDS.AddArray(
						new Id( SymbolTable.GetNextMemoryBlockName() ),
						this.Machine.TypeSystem.GetCharType(),
						s.Length + 1
					);

					// Copy string contents
					for(int i = 0; i < s.Length; ++i) {
						this.Machine.Memory.Write( mblock.Address + i, new byte[]{ (byte) s[ i ] } );
					}

					// Set trailing zero
					this.Machine.Memory.Write( mblock.Address + s.Length, new byte[]{ 0 } );

					toret.LiteralValue = new IntLiteral( this.Machine, mblock.Address );
				}
				else
				if ( rvalue is ArrayVariable ) {
					toret.LiteralValue = new IntLiteral( this.Machine, rvalue.Address );
				} else {
					toret.LiteralValue = rvalue.LiteralValue;
				}
			}

			this.Machine.ExecutionStack.Push( toret );
        }
        
        private RValue Value {
            get; set;
        }

		private Variable Vble {
			get; set;
		}
    }
}
