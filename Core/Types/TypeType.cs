//
// TypeType.cs
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
using CSim.Core.Variables;

namespace CSim.Core.Types {
    /// <summary>
    /// A type for a <see cref="Core.Variables.TypeVariable"/>'s.
    /// </summary>
    public class TypeType: AType {
        /// <summary>The name of the type.</summary>
        public const string TypeName = "type_t";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CSim.Core.Types.TypeType"/> class.
        /// <param name="m">The <see cref="Machine"/> this type will live in.</param>
        /// </summary>
        public TypeType(Machine m)
            : base( m, TypeName, 1 )
        {
        }

		/// <summary>
		/// Creates a corresponding variable.
		/// </summary>
		/// <returns>A <see cref="Variable"/> honoring this <see cref="AType"/>.</returns>
		/// <param name="strId">The identifier for the <see cref="Variable"/>.</param>
		public override Variable CreateVariable(string strId)
		{
			return new TypeVariable( new Id( this.Machine, strId ) );
		}

        /// <summary>
        /// Creates a new literal for this type.
        /// </summary>
        /// <returns>The literal, as a <see cref="Literals.TypeLiteral"/>.</returns>
        /// <param name="raw">A raw representation of the value in memory.</param>
        public override Literal CreateLiteral(byte[] raw)
        {
			return new Literals.TypeLiteral( this.Machine, raw[ 0 ] );
        }

        /// <summary>
        /// Returns the only instance for this <see cref="AType"/>.
        /// </summary>
        /// <param name="m">The <see cref="Machine"/> this type will live in.</param>
        /// <returns>The <see cref="TypeType"/> <see cref="AType"/>.</returns>
        public static TypeType Get(Machine m)
        {
            if ( typeTypeInstance == null ) {
                typeTypeInstance = new TypeType( m );
            }

            return typeTypeInstance;
        }

        private static TypeType typeTypeInstance;
    }
}
