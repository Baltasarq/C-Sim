using System;

using CSim.Core.Exceptions;
using CSim.Core.Variables;
using CSim.Core.Literals;

namespace CSim.Core.Opcodes
{
	public abstract class ModificationOpcode: Opcode
	{
        public const bool LeftPtrAccess = true;
        public const bool NotLeftPtrAccess = false;

        protected ModificationOpcode(Machine m, string lvalueId, bool isLeftPtr)
            : base( m )
        {
            this.LValueId = lvalueId;
            this.IsLeftPtrAccess = isLeftPtr;
        }

        public Variable LookForLValue()
        {
            Variable toret = null;

            if ( this.IsLeftPtrAccess ) {
                var ptrVble = this.Machine.TDS.LookUp( this.LValueId ) as PtrVariable;

                if ( ptrVble != null ) {
					toret = this.Machine.TDS.GetPointedValueAsVariable( ptrVble );
                } else {
                    throw new TypeMismatchException( 
                        L18n.Get( L18n.Id.LblPointer ).ToLower()
                        + " (" + L18n.Get( L18n.Id.ErrNotAPointer )
                        + ": " + this.LValueId + ") " );
                }
            }
            else {
				toret = this.Machine.TDS.LookUp( this.LValueId );
            }

            return toret;
        }

		public static ModificationOpcode Generate(Machine machine, string id, RValue value)
		{
			ModificationOpcode toret = null;
			var strValue = value as StrLiteral;
			bool isLeftPtr = ( id [0] == '*' );

			if ( isLeftPtr ) {
				id = id.Substring( 1 );
            }

            if ( strValue != null
              && strValue.Value[ 0 ] == '&' )
			{
                strValue = new StrLiteral( machine, strValue.Value.Substring( 1 ) );
                toret = new ModifyWithVbleAddress( machine, id, strValue.Value, isLeftPtr );
            } else {
				toret = new Assign( machine, id, value, isLeftPtr );
			}

			return toret;
		}

        public bool IsLeftPtrAccess {
            get; set;
        }

        public string LValueId {
            get; set;
        }
	}
}

