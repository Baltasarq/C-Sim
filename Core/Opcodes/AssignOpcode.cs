
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
				var targetVbleAddress = rvalue.LiteralValue.GetValueAsInteger();
				lvalue.PointedVble = this.Machine.TDS.LookForAddress( targetVbleAddress, lvalue.AssociatedType );

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
			var rvalueVble = this.Machine.ExecutionStack.Pop().SolveToVariable();

			// Take variable
			var lvalueVble = this.Machine.ExecutionStack.Pop().SolveToVariable();

			// Prepare assign parts
			if ( lvalueVble is NoPlaceTempVariable ) {
				throw new UnknownVbleException( "temp vble: " + lvalueVble.Name.Name + "??" );
			}

			Variable toret = lvalueVble;

			// Chk types
			if ( !toret.Type.IsCompatibleWith( rvalueVble.Type ) ) {
				throw new TypeMismatchException(
						rvalueVble.Name.Name + "? : "
						+ rvalueVble.Type + " != "
						+ toret.Type
					);
			}

			// Is lvalue a ref?
			var r = toret as RefVariable;

			if ( r != null ) {
				if ( !r.IsSet() ) {
					this.SetRef( r, rvalueVble );
					toret = r.PointedVble;
				} else {
					toret = r.PointedVble;
				}
			}
            
            // Assign rvalue
            var strRValue = rvalueVble.LiteralValue as StrLiteral;
            
			if ( strRValue != null ) {
                string s = strRValue.Value;
				Variable mblock = new ArrayVariable(
					    new Id( this.Machine, SymbolTable.GetNextMemoryBlockName() ),
					    this.Machine.TypeSystem.GetCharType(),
                        s.Length + 1
                );
				
                this.Machine.TDS.Add( mblock );

				// Copy string contents
				for(int i = 0; i < s.Length; ++i) {
					this.Machine.Memory.Write( mblock.Address + i, new byte[]{ (byte) s[ i ] } );
				}

				// Set trailing zero
				this.Machine.Memory.Write( mblock.Address + s.Length, new byte[]{ 0 } );

				toret.LiteralValue = new IntLiteral( this.Machine, mblock.Address );
			}
			else
			if ( rvalueVble is ArrayVariable ) {
				toret.LiteralValue = new IntLiteral( this.Machine, rvalueVble.Address );
			} else {
				toret.LiteralValue = rvalueVble.LiteralValue;
			}

			this.Machine.ExecutionStack.Push( toret );
        }
    }
}
