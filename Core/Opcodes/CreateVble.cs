using System;

using CSim.Core.Types;
using CSim.Core.Exceptions;

namespace CSim.Core.Opcodes
{
    public class CreateVble: CreationOpcode
    {
        public const char OpcodeValue = (char) 0xE1;

        public CreateVble(Machine m, string n, Type t)
            : base( m, n, t )
        {
        }

        public override Variable Execute()
        {
            return Machine.TDS.Add( this.Name, this.Type );
        }
    }
}

