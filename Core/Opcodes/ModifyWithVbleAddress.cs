using System;

using CSim.Core;
using CSim.Core.Exceptions;
using CSim.Core.Types;
using CSim.Core.Variables;
using CSim.Core.Literals;

namespace CSim.Core.Opcodes
{
	public class ModifyWithVbleAddress: ModificationOpcode
	{
		public static char OpcodeValue = (char) 0xE5;
        
        public ModifyWithVbleAddress(Machine machine, string leftId, string rightId, bool isLeftPtr)
            :base( machine, leftId, isLeftPtr )
        {
            this.RightId = rightId;
        }
        
        public override Variable Execute()
		{
            Variable toret = this.LookForLValue();
			Variable rightVble = this.Machine.TDS.LookUp( this.RightId );

			// Chk compatibility
            if ( toret.Type is Ptr
              && !( toret.Type is Ref ) )
            {
				var ptrType = (Ptr) toret.Type;

				if ( ptrType.AssociatedType != rightVble.GetTargetType() ) {
					throw new TypeMismatchException( ptrType.AssociatedType.ToString() );
				}
			} else {
                throw new TypeMismatchException( L18n.Get( L18n.Id.LblPointer ).ToLower() );
            }

			var refRightVble = rightVble as RefVariable;
			if ( refRightVble != null ) {
				toret.LiteralValue = new IntLiteral( this.Machine, refRightVble.PointedVble.Address );
			} else {
				toret.LiteralValue = new IntLiteral( this.Machine, rightVble.Address );
			}
            return toret;
        }
        
        public string RightId {
            get; set;
        }
	}
}

