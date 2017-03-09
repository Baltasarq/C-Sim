//
// UIntLiteral.cs
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

namespace CSim.Core.Literals {
    using System.Numerics;
    
    /// <summary>
    /// Literals of type Int.
    /// </summary>
    public class ULongLiteral: Literal {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:CSim.Core.Literals.IntLiteral"/> class.
        /// </summary>
        /// <param name="m">The <see cref="Machine"/>.</param>
        /// <param name="x">A given long.</param>
        public ULongLiteral(Machine m, object x)
            :this( m, GetValueAsULongFromUnsigned( m, x ) )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CSim.Core.Literals.IntLiteral"/> class.
        /// </summary>
        /// <param name="m">The <see cref="Machine"/>.</param>
        /// <param name="x">A given unsigned integer.</param>
        public ULongLiteral(Machine m, BigInteger x)
            :base( m, x )
        {
        }

        /// <summary>
        /// Gets the type of the int literal.
        /// The type is <see cref="CSim.Core.Types.Primitives.Int"/>.
        /// </summary>
        /// <value>The <see cref="AType"/>.</value>
        public override AType Type {
            get {
                return this.Machine.TypeSystem.GetULongType();
            }
        }

        /// <summary>
        /// The value stored, as an integer value.
        /// </summary>
        /// <value>The value.</value>
        public new BigInteger Value {
            get {
                return (BigInteger) base.Value;
            }
        }

        /// <summary>
        /// Gets the raw value of the literal, as sequence of bytes.
        /// </summary>
        /// <value>The raw value.</value>
        public override byte[] GetRawValue()
        {
            return this.Machine.Bytes.FromULongToBytes( this.Value );
        }
        
        /// <summary>
        /// Gets the value as an integer.
        /// </summary>
        /// <returns>The value as <see cref="BigInteger"/>.</returns>
        public override BigInteger GetValueAsInteger()
        {
            return GetValueAsULongFromUnsigned( this.Machine, this.Value );
        }
        
        /// <summary>
        /// Returns a string appropriately formatted with the value of the literal.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="CSim.Core.Literals.IntLiteral"/>.</returns>
        public override string ToString()
        {
            return this.ToPrettyNumber();
        }
        
        /// <summary>
        /// Gets the value as an integer from an unsigned value.
        /// It uses the byte representation from the machine to do the conversion,
        /// since the standard library would throw an exception.
        /// </summary>
        /// <returns>The value as a <see cref="BigInteger"/>.</returns>
        /// <param name="m">The machine this value will be converted for.</param>
        /// <param name="x">The value itself.</param>
        public static BigInteger GetValueAsULongFromUnsigned(Machine m, object x)
        {
            return m.Bytes.FromBytesToULong( m.Bytes.FromLongToBytes( x.ToBigInteger() ) );
        }
    }
}
