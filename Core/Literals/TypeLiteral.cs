//
// TypeLiteral.cs
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
    /// <see cref="AType"/> <see cref="Literal"/>'s.
    /// </summary>
    public class TypeLiteral: Literal {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Literals.TypeLiteral"/> class.
		/// </summary>
		/// <param name="v">The value for the literal.</param>
		public TypeLiteral(object v)
			:this( (AType) v )
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CSim.Core.Literals.TypeLiteral"/> class.
        /// </summary>
        /// <param name="v">The value for the literal.</param>
        public TypeLiteral(AType v)
			:base( v.Machine, v.Machine.Bytes.FromTypeToBytes( v )[ 0 ] )
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:CSim.Core.Literals.TypeLiteral"/> class.
        /// </summary>
        /// <param name="m">The machine this literal will live in.</param>
        /// <param name="v">The representation of the type as a byte.</param>
        internal static TypeLiteral Create(Machine m, byte v)
        {
			return new TypeLiteral( m.Bytes.FromBytesToType( new []{ v } ) );
        }

        /// <summary>
        /// Gets the type of the <see cref="TypeLiteral"/>.
        /// The type is <see cref="Types.TypeType"/>.
        /// </summary>
        /// <value>The <see cref="AType"/>, which is <see cref="Types.TypeType"/>.</value>
        public override AType Type {
            get {
                return Types.TypeType.Get( this.Machine );
            }
        }

        /// <summary>
        /// The value stored, as a type.
        /// </summary>
        /// <value>The value.</value>
        public new AType Value {
            get {
				return this.Machine.Bytes.FromBytesToType(new [] { (byte) base.Value } );
            }
        }

        /// <summary>
        /// Gets the raw value of the literal, as sequence of bytes.
        /// </summary>
        /// <value>The raw value.</value>
        public override byte[] GetRawValue()
        {
            return new []{ (byte) base.Value };
        }
        
        /// <summary>
        /// Gets the value as an integer.
        /// </summary>
        /// <returns>The value as a <see cref="BigInteger"/>.</returns>
        public override BigInteger GetValueAsInteger()
        {
            return System.Convert.ToInt64( this.Value.Size );
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:TypeLiteral"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:TypeLiteral"/>.
        /// </returns>
        public override string ToString()
        {
            return this.Value.ToString();
        }

        /// <summary>
        /// Creates a <see cref="TypeLiteral"/> from a given string.
        /// For example, it creates the corresponding TypeLiteral from "int **"
        /// </summary>
        /// <returns>The corresponding <see cref="TypeLiteral"/>.</returns>
        /// <param name="m">The <see cref="Machine"/> this literal will live in.</param>
        /// <param name="strTypeLit">The type literal, as a string.</param>
        public static TypeLiteral CreateFromString(Machine m, string strTypeLit)
        {
            TypeLiteral toret = null;
            AType type = m.TypeSystem.FromStringToType( strTypeLit );
            
            if ( type != null ) {
                toret = new TypeLiteral( type );
            }
            
            return toret;
        }
    }
}
