//
// ByteConverter.cs
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
	using CSim.Core.Types;

	/// <summary>
	/// Byte converter.
	/// Reads or writes bytes for the primitive types.
	/// </summary>
	public class ByteConverter {
		/// <summary>
		/// Initializes a new instance of the <see cref="ByteConverter"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/> this converter will work for.</param>
		public ByteConverter(Machine m)
		{
			this.Machine = m;
		}

		/// <summary>
		/// Converts the given byte vector to a single char.
		/// </summary>
		/// <returns>The char, as a primitive char.</returns>
		/// <param name="data">The data, as a vector of bytes.</param>
		public char FromBytesToChar(byte[] data)
		{
			return (char) data[ 0 ];
		}

		/// <summary>
		/// Cnvts the char to a vector of bytes.
		/// Note that chars are guaranteed to be of size 1.
		/// </summary>
		/// <seealso cref="CSim.Core.Types.Primitives.Char"/>
		/// <returns>A vector of bytes, length 1</returns>
		/// <param name="c">The char to convert, as a primitive char.</param>
		public byte[] FromCharToBytes(char c)
		{
			var toret = new byte[ 1 ];

			toret[ 0 ] = (byte) c;
			return toret;
		}

		/// <summary>
		/// Converts the given byte vector to a single int.
		/// The byte sequence is expected to follow the machine's architecture.
		/// </summary>
		/// <returns>The int, as a primitive int.</returns>
		/// <param name="data">The data, as a vector of bytes.</param>
		public long FromBytesToInt(byte[] data)
		{
			int currentWordSize = this.Machine.WordSize;
			int destLength = data.Length < sizeof(long) ? sizeof(long) : data.Length;

			// Create dest array
			var bytes = new byte[ destLength ];

			// Create org vector, if needed
			if ( data.Length != currentWordSize ) {
				var newData = new byte[ currentWordSize ];

				Array.Copy(
					data, 0,
					newData, Math.Max( 0, currentWordSize - data.Length ),
					data.Length );

				data = newData;
			}

			// Reverse it if needed
			if ( BitConverter.IsLittleEndian != this.Machine.IsLittleEndian )
			{
				Array.Reverse( data );
			}

			// Copy the result and end
			Array.Copy(
				data, 0,
				bytes, 0,//Math.Max( 0, destLength - wordSize ),
				currentWordSize );
			return BitConverter.ToInt64( bytes, 0 );
		}

		/// <summary>
		/// Converts an int to an array of bytes.
		/// The byte sequence will be returned following the machine's architecture.
		/// </summary>
		/// <returns>The resulting array.</returns>
		/// <param name="value">Value to convert, as a primitive int.</param>
		public byte[] FromIntToBytes(long value)
		{
			int currentWordSize = this.Machine.WordSize;
			var toret = BitConverter.GetBytes( value );

			// Limit the byte array to wordsize
			if ( toret.Length > currentWordSize ) {
				var bytes = new byte[ this.Machine.WordSize ];

				if ( !BitConverter.IsLittleEndian ) {
					Array.Copy( toret, currentWordSize, bytes, 0, currentWordSize );
					toret = bytes;
					bytes = new byte[ this.Machine.WordSize ];
				}

				Array.Copy( toret, 0, bytes, 0, currentWordSize );
				toret = bytes;
			}

			if ( BitConverter.IsLittleEndian != this.Machine.IsLittleEndian )
			{
				Array.Reverse( toret );
			}

			return toret;
		}

		/// <summary>
		/// Converts the given byte vector to a single double.
		/// Should apply IEEE 754.
		/// </summary>
		/// <returns>The double, as a primitive double.</returns>
		/// <param name="data">The data, as a vector of bytes.</param>
		public double FromBytesToDouble(byte[] data)
		{
			if ( BitConverter.IsLittleEndian != this.Machine.IsLittleEndian )
			{
				var bytes = new byte[ data.Length ];
				Array.Copy( data, bytes, data.Length );
				Array.Reverse( bytes );
				data = bytes;
			}

			return BitConverter.ToDouble( data, 0 );
		}

		/// <summary>
		/// Converts a double to an array of bytes.
		/// </summary>
		/// <returns>The resulting array.</returns>
		/// <param name="value">Value to convert, as a primitive double.</param>
		public byte[] FromDoubleToBytes(double value)
		{
			var toret = BitConverter.GetBytes( value );

			if ( BitConverter.IsLittleEndian != this.Machine.IsLittleEndian )
			{
				Array.Reverse( toret );
			}

			return toret;
		}

		/// <summary>
		/// Converts a <see cref="AType"/> to its raw byte value.
		/// </summary>
		/// <returns>A byte representing the <see cref="AType"/>.</returns>
		/// <param name="t">An array of bytes, with the binary representation.</param>
		public byte[] FromTypeToBytes(AType t)
		{
			byte toret = 0;

			if ( t != Any.Get( t.Machine) ) {
				var ptrType = t as Ptr;
				var refType = t as Ref;

				// Modifiers
				if ( refType != null ) {
					t = refType.AssociatedType;
					toret += 20;
				}
				else
					if ( ptrType != null ) {
						t = ptrType.AssociatedType;
						toret += 10;

					}

				// Basic value
				if ( t is Types.Primitives.Char ) {
					toret += 1;
				}
				else
					if ( t is Types.Primitives.Int ) {
						toret += 2;
					}
					else
						if ( t is Types.Primitives.Double ) {
							toret += 3;
						}
			}

			return new []{ toret };
		}

		/// <summary>
		/// Converts a binary representation to its corresponding type.
		/// </summary>
		/// <returns>A <see cref="AType"/>.</returns>
		/// <param name="bytes">A raw binary representation.</param>
		public AType FromBytesToType(byte[] bytes)
		{
			AType toret = null;
			bool isPtr = false;
			bool isRef = false;
			byte b = bytes[ 0 ];

			if ( b >= 20 ) {
				isRef = true;
				b -= 20;
			}

			if ( b >= 10 ) {
				isPtr = true;
				b -= 10;
			}

			switch( b ) {
				case 0:
				toret = Any.Get( this.Machine );
				break;
				case 1:
				toret = this.Machine.TypeSystem.GetCharType();
				break;
				case 2:
				toret = this.Machine.TypeSystem.GetIntType();
				break;
				case 3:
				toret = this.Machine.TypeSystem.GetDoubleType();
				break;
			}

			if ( isRef ) {
				toret = this.Machine.TypeSystem.GetRefType( toret );
			}

			if ( isPtr ) {
				toret = this.Machine.TypeSystem.GetPtrType( toret );
			}

			// Chk
			if ( toret == null ) {
				throw new EngineException( "inconvertible type: " + b );
			}

			return toret;
		}

		/// <summary>
		/// Gets the machine for this byte converter.
		/// </summary>
		/// <value>The <see cref="Machine"/>.</value>
		public Machine Machine {
			get; private set;
		}
	}
}
