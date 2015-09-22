using System;

using CSim.Core;
using CSim.Core.Types;
using CSim.Core.Variables;
using CSim.Core.Exceptions;

namespace CSim.Core.Opcodes {
    public class Assign: ModificationOpcode {
        public static char OpcodeValue = (char) 0xE3;
        
        public Assign(Machine m, string id, RValue value, bool isLeftPtr)
            : base( m, id, isLeftPtr )
        {
            this.Value = value;
        }

		private void SetRef(RefVariable lvalue, Variable rvalue)
		{
			// Check rvalue
			if ( rvalue is TempVariable ) {
				int address = rvalue.Address;

				rvalue = Machine.TDS.LookForAddress( rvalue.Address );

				if ( rvalue == null ) {
					throw new UnknownVbleException( string.Format(
								"[{0}]",
					            Literal.ToPrettyNumber( address ) ) );
				}

				if ( lvalue.AssociatedType != rvalue.GetTargetType() ) {
					throw new TypeMismatchException( lvalue.Type.Name );
				}
			}

			var rvalueRef = rvalue as RefVariable;
			var rvaluePtr = rvalue as PtrVariable;

			// Assign to variable
			if ( rvalueRef != null ) {
				lvalue.PointedVble = rvalueRef.PointedVble;
			}
			else
			if ( rvaluePtr != null ) {
				lvalue.PointedVble = Machine.TDS.LookForAddress( rvaluePtr.LiteralValue );
			}
			else {
				lvalue.PointedVble = rvalue;
			}

			// Chk
			if ( lvalue.PointedVble.Address < 0 ) {
				throw new UnknownVbleException( rvalue.Name.Value );
			}

			if ( lvalue.PointedVble.Type != rvalue.Type ) {
				throw new TypeMismatchException(
					lvalue.PointedVble.Type.ToString()
					+ " is not "
					+ rvalue.Type.ToString()
				);
			}

			return;
		}

        public override Variable Execute()
		{
            Variable toret = this.LookForLValue();
			Variable rvalue = this.Machine.TDS.LookForRValue( toret, this.Value );

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
				toret.LiteralValue = rvalue.LiteralValue;
			}

			return toret;
        }
        
        public RValue Value {
            get; set;
        }
    }
}
