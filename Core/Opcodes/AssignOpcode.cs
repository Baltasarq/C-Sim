
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
        public static byte OpcodeValue = 0xE1;
        
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
			if ( rvalue.IsTemp() ) {
				throw new UnknownVbleException(
                    string.Format( "{0} ({1}) == {2}",
                    rvalue.Name.Name,
					new IntLiteral( this.Machine, rvalue.Address )
                                                            .ToPrettyNumber(),
                    rvalue.LiteralValue ) );
			}

			lvalue.PointedVble = rvalue;

			// Chk
            AType lType = lvalue.AssociatedType;
            AType rType = lvalue.PointedVble.Type;
			this.Machine.Memory.CheckSizeFits(
                                            lvalue.PointedVble.Address,
                                            lvalue.AssociatedType.Size );

			if ( lType != rType ) {
				throw new TypeMismatchException( lType + " != " + rType );
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
			if ( lvalueVble.IsTemp() ) {
				throw new UnknownVbleException( "temp vble: " + lvalueVble.Name.Name + "??" );
			}

            // Chk types
            if ( !lvalueVble.Type.IsCompatibleWith( rvalueVble.Type ) ) {
                throw new TypeMismatchException(
                        rvalueVble.Name.Name + "? : "
                        + rvalueVble.Type + " != "
                        + lvalueVble.Type
                    );
            }            

            Variable toret = lvalueVble;

            // Is lvalue a ref?            
			if ( toret is RefVariable r ) {
				if ( !r.IsSet() ) {
					this.SetRef( r, rvalueVble );
                    goto End;
				}
                
                toret = r.PointedVble;
			}
                        
			if ( !( rvalueVble is ArrayVariable )
              && rvalueVble.LiteralValue is StrLiteral strRValue )
            {
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
                if ( toret != rvalueVble ) {
				    toret.LiteralValue = rvalueVble.LiteralValue;
                }
			}

            End:
			this.Machine.ExecutionStack.Push( toret );
        }
        
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Opcodes.AssignOpcode"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Opcodes.AssignOpcode"/>.</returns>
        public override string ToString()
        {
            return string.Format(
                            "[AssignOpcode ({0}) lvalue(POP) <- rvalue(POP)]",
                            OpcodeValue
            );
        }
    }
}
