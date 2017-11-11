// Prys CSim - Copyright (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.FunctionLibrary {
    using CSim.Core.Functions;
    using CSim.Core.Literals;
    using CSim.Core.Types;

    /// <summary>
    /// This is the typeof function.
    /// Signature: type_t typeof(x); // x can be anything
    /// </summary>
    public sealed class TypeOf: EmbeddedFunction {
        /// <summary>
        /// The identifier for the function.
        /// </summary>
        public const string Name = "typeof";

        /// <summary>
        /// Initializes a new instance of the <see cref="CSim.Core.Functions.EmbeddedFunction"/> class.
        /// This is not intended to be used directly.
        /// <param name="m">The <see cref="Machine"/> this function will be executed in.</param>
        /// </summary>
        private TypeOf(Machine m)
            : base( m, Name, TypeType.Get( m ), sizeofFormalParams )
        {
        }

        /// <summary>
        /// Returns the only instance of this function.
        /// </summary>
        public static TypeOf Get(Machine m)
        {
            if ( instance == null ) {
                sizeofFormalParams = new Variable[] {
                    new Variable( new Id( m, @"x" ), Any.Get( m ) )
                };

                instance = new TypeOf( m );
            }

            return instance;
        }

        /// <summary>
        /// Execute this <see cref="Function"/> with
        /// the specified parameters (<see cref="RValue"/>'s).
        /// </summary>
        /// <param name="realParams">The parameters.</param>
        public override void Execute(RValue[] realParams)
        {
            var argVble = realParams[ 0 ].SolveToVariable();
            var typeLit = new TypeLiteral( argVble.Type );
            
            if ( argVble.Type is TypeType ) {
                typeLit = (TypeLiteral) argVble.LiteralValue;
            }
            
            this.Machine.ExecutionStack.Push(
                                        Variable.CreateTempVariable( typeLit ) );
        }

        private static TypeOf instance = null;
        private static Variable[] sizeofFormalParams;
    }
}

