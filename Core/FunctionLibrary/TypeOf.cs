//
// typeof.cs
//
// Author:
//       baltasarq <baltasarq@gmail.com>
//
// Copyright (c) 2017 baltasarq
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace CSim.Core.FunctionLibrary {
    using CSim.Core.Functions;
    using CSim.Core.Variables;
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

