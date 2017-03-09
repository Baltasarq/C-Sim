﻿//
// ExtensionMethods.cs
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

namespace CSim.Core {
    using System;
    using System.Numerics;

	using Core.Native;
    
    /// <summary>
    /// Contains all needed extension methods.
    /// </summary>
    public static class ExtensionMethods {
        /// <summary>
        /// Converts any object to a BigInteger, provided it
        /// holds a numeric value.
        /// </summary>
        /// <returns>A <see cref="BigInteger"/>.</returns>
        /// <param name="value">Any value.</param>
        /// <exception cref="ArgumentException">When the object does not hold
        /// a numeric value.</exception>
        public static BigInteger ToBigInteger(this object value)
        {
            BigInteger toret = BigInteger.Zero;

            if ( value == null ) {
                throw new ArgumentException( "trying to convert null to BigInteger" );
            }

            if ( value is BigInteger ) {
                toret = (BigInteger) value;
            }
            else
            if ( value is sbyte
              || value is byte
              || value is short
              || value is ushort
              || value is int
              || value is uint
              || value is long )
            {
                toret = new BigInteger( Convert.ToInt64( value ) );
            }
            else
            if ( value is ulong ) {
                toret = new BigInteger( Convert.ToUInt64( value ) );
            }
            else
            if ( value is float
              || value is double )
            {
                toret = new BigInteger( Convert.ToDouble( value ) );
            } else {
                throw new ArgumentException( "cannot convert to BigInteger from: " + value.GetType() );
            }
            
            return toret;
        }
        
        /// <summary>
        /// Converts a <see cref="BigInteger"/> to a byte.
        /// Overflow is allowed, so for example from a 128bit value
        /// only the first 8bits will be returned.
        /// </summary>
        /// <returns>An <see cref="System.Byte"/> value.</returns>
        /// <param name="x">A <see cref="BigInteger"/>.</param>
        public static byte ToByte(this BigInteger x)
        {
            return x.ToByteArray()[ 0 ];
        }
        
        /// <summary>
        /// Converts a <see cref="BigInteger"/> to an int of 16 bits.
        /// Overflow is allowed, so for example from a 128bit value
        /// only the first 16bits will be returned.
        /// </summary>
        /// <returns>An <see cref="System.Int16"/> value.</returns>
        /// <param name="x">A <see cref="BigInteger"/>.</param>
        public static Int16 ToInt16(this BigInteger x)
        {
			int numBytes = sizeof(Int16);
            Int16 toret;
			byte[] bits = x.ToByteArray();
            
			if ( bits.Length > numBytes ) {
                Array.Resize( ref bits, numBytes );
                toret = BitConverter.ToInt16( bits, 0 );
            } else {
                toret = (Int16) x;
            }
            
            return toret;
        }
        
        /// <summary>
        /// Converts a <see cref="BigInteger"/> to an unsigned int of 16 bits.
        /// Overflow is allowed, so for example from a 128bit value
        /// only the first 16bits will be returned.
        /// </summary>
        /// <returns>An <see cref="System.UInt16"/> value.</returns>
        /// <param name="x">A <see cref="BigInteger"/>.</param>
        public static UInt16 ToUInt16(this BigInteger x)
        {
            int numBytes = sizeof(UInt16);
            UInt16 toret;
            byte[] bits = x.ToByteArray();
            
            if ( bits.Length > numBytes ) {
                Array.Resize( ref bits, numBytes );
                toret = BitConverter.ToUInt16( bits, 0 );
            } else {
                toret = (UInt16) x;
            }
            
            return toret;
        }
        
        /// <summary>
        /// Converts a <see cref="BigInteger"/> to an int of 32 bits.
        /// Overflow is allowed, so for example from a 128bit value
        /// only the first 32bits will be returned.
        /// </summary>
        /// <returns>An <see cref="System.Int32"/> value.</returns>
        /// <param name="x">A <see cref="BigInteger"/>.</param>
        public static Int32 ToInt32(this BigInteger x)
        {
			int numBytes = sizeof(Int32);
			Int32 toret;
			byte[] bits = x.ToByteArray();

			if ( bits.Length > numBytes ) {
				Array.Resize( ref bits, numBytes );
				toret = BitConverter.ToInt32( bits, 0 );
			} else {
				toret = (Int32) x;
			}

			return toret;
        }
        
        /// <summary>
        /// Converts a <see cref="BigInteger"/> to an unsigned int of 32 bits.
        /// Overflow is allowed, so for example from a 128bit value
        /// only the first 32bits will be returned.
        /// </summary>
        /// <returns>An <see cref="System.UInt32"/> value.</returns>
        /// <param name="x">A <see cref="BigInteger"/>.</param>
        public static UInt32 ToUInt32(this BigInteger x)
        {
            int numBytes = sizeof(UInt32);
            UInt32 toret;
            byte[] bits = x.ToByteArray();
            
            if ( bits.Length > numBytes ) {
                Array.Resize( ref bits, numBytes );
                toret = BitConverter.ToUInt32( bits, 0 );
            } else {
                toret = (UInt32) x;
            }
            
            return toret;
        }
        
        /// <summary>
        /// Converts a <see cref="BigInteger"/> to an int of 64 bits.
        /// Overflow is allowed, so for example from a 128bit value
        /// only the first 64bits will be returned.
        /// </summary>
        /// <returns>An <see cref="System.Int64"/> value.</returns>
        /// <param name="x">A <see cref="BigInteger"/>.</param>
        public static Int64 ToInt64(this BigInteger x)
        {
			int numBytes = sizeof(Int64);
			Int64 toret;
			byte[] bits = x.ToByteArray();

			if ( bits.Length > numBytes ) {
				Array.Resize( ref bits, numBytes );
				toret = BitConverter.ToInt64( bits, 0 );
			} else {
				toret = (Int64) x;
			}

			return toret;
        }
        
        /// <summary>
        /// Converts a <see cref="BigInteger"/> to an int of 64 bits.
        /// Overflow is allowed, so for example from a 128bit value
        /// only the first 64bits will be returned.
        /// </summary>
        /// <returns>An <see cref="System.UInt64"/> value.</returns>
        /// <param name="x">A <see cref="BigInteger"/>.</param>
        public static UInt64 ToUInt64(this BigInteger x)
        {
            int numBytes = sizeof(UInt64);
            UInt64 toret;
            byte[] bits = x.ToByteArray();

            if ( bits.Length > numBytes ) {
                Array.Resize( ref bits, numBytes );
                toret = BitConverter.ToUInt64( bits, 0 );
            } else {
                toret = (UInt64) x;
            }

            return toret;
        }

		/// <summary>
		/// Converts a <see cref="BigInteger"/> to an unsigned int of 128 bits.
		/// Overflow is allowed, so for example from a 256bit value
		/// only the first 128bits will be returned.
		/// </summary>
		/// <returns>An <see cref="UInt128"/> value.</returns>
		/// <param name="x">A <see cref="BigInteger"/>.</param>
		public static UInt128 ToUInt128(this BigInteger x)
		{
			return new UInt128( x );
		}

		/// <summary>
		/// Converts a <see cref="BigInteger"/> to an int of 128 bits.
		/// Overflow is allowed, so for example from a 256bit value
		/// only the first 128bits will be returned.
		/// </summary>
		/// <returns>An <see cref="Int128"/> value.</returns>
		/// <param name="x">A <see cref="BigInteger"/>.</param>
		public static Int128 ToInt128(this BigInteger x)
		{
			return new Int128( x );
		}
    }
}