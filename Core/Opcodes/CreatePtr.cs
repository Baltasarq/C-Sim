using System;

using CSim.Core;
using CSim.Core.Types;
using CSim.Core.Variables;
using CSim.Core.Exceptions;

namespace CSim.Core.Opcodes
{
	public class CreatePtr: CreationOpcode
	{
        public const char OpcodeValue = (char) 0xE2;

        public CreatePtr(Machine m, string n, Type t)
            : base( m, n, t )
        {
        }

        public override Variable Execute()
        {
            return  this.Machine.TDS.AddPtr( this.Name, this.Type );
        }
	}
}

