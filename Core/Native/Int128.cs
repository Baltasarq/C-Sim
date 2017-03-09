//
// Int128.cs
//
// Author:
//		Simon Mourier
// Adapted by:
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

namespace CSim.Core {
    using System;
	using System.Numerics;
    
    public struct Int128 {
		/// <summary>The length in bytes for this integer's
		///  binary representation.</summary>
		public const int LengthInBytes = 16;

		/// <summary>
		/// Initializes a new instance of the <see cref="Int128"/> struct.
		/// </summary>
		/// <param name="value">The value.</param>
		public Int128(byte value)
		{
			this.hi = 0;
			this.lo = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Int128"/> struct.
		/// </summary>
		/// <param name="value">The value.</param>
		public Int128(char value)
		{
			this.hi = 0;
			this.lo = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Int128"/> struct.
		/// </summary>
		/// <param name="value">The value.</param>
		public Int128(Int16 value)
			: this( (Int32) value)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Int128"/> struct.
		/// </summary>
		/// <param name="value">The value.</param>
		public Int128(Int32 value)
			: this( (Int64) value)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Int128"/> struct.
		/// </summary>
		/// <param name="value">The value.</param>
		public Int128(Int64 value)
		{
			if ( value < 0 ) {
				// long.MinValue = -long.MinValue
				if (value == long.MinValue) {
					this.hi = HiNeg;
					this.lo = HiNeg;
					return;
				}

		/*		Int128 n = -new Int128(-value);
				this.hi = n.hi;
				this.lo = n.lo;*/
				this.hi = this.lo = 0;
				return;
			}

			this.hi = 0;
			this.lo = (UInt64)value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Int128"/> struct.
		/// </summary>
		/// <param name="value">The value.</param>
		public Int128(byte[] value)
		{
			if (value == null)
				throw new ArgumentNullException( nameof( value ) );

			if (value.Length != 16)
				throw new ArgumentException( null, nameof( value ) );

			this.hi = BitConverter.ToUInt64( value, 8 );
			this.lo = BitConverter.ToUInt64( value, 0 );
		}

		private Int128(UInt64 hi, UInt64 lo)
		{
			this.hi = hi;
			this.lo = lo;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Int128"/> struct.
		/// </summary>
		/// <param name="value">The value, as a <see cref="BigInteger"/>.</param>
		public Int128(BigInteger value)
			:this( value.ToByteArray() )
		{
		}

		public BigInteger Value {
			get {
				return new BigInteger();
			}
		}

		public byte[] Bytes {
			get {
				return this.Value.ToByteArray();
			}
		}


		/// <summary>
		/// Gets a value that represents the number 0 (zero).
		/// </summary>
		public static Int128 Zero = GetZero();

		/// <summary>
		/// Represents the largest possible value of an Int128.
		/// </summary>
		public static Int128 MaxValue = GetMaxValue();

		/// <summary>
		/// Represents the smallest possible value of an Int128.
		/// </summary>
		public static Int128 MinValue = GetMinValue();

		private static Int128 GetMaxValue()
		{
			return new Int128( long.MaxValue, ulong.MaxValue );
		}

		private static Int128 GetMinValue()
		{
			return new Int128( HiNeg, 0 );
		}

		private static Int128 GetZero()
		{
			return new Int128();
		}

		/// <summary>
		/// Creates a new value from raw bytes.
		/// </summary>
		/// <returns>A new integral value.</returns>
		/// <param name="data">A raw byte array.</param>
		public static Int128 FromBytes(byte[] data)
		{
			var toret = new Int128();
			int max = Math.Min( Int128.LengthInBytes, data.Length );

			if ( BitConverter.IsLittleEndian ) {
				int pos = 0;
				for(int i = max; i >= 0; --i) {
				//	toret.bytes[ pos ] = data[ i ];
					++pos;
				}
			} else {
				for(int i = 0; i < max; ++i) {
				//	toret.bytes[ i ] = data[ i ];
				}
			}

			return toret;
		}
        
		private const UInt64 HiNeg = 0x8000000000000000;
		private UInt64 hi;
		private UInt64 lo;
    }
}
