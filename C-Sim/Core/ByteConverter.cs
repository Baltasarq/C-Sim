// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core {
    using Convert = System.Convert;
    using Array = System.Array;
    using BitConverter = System.BitConverter;
    using System.Numerics;
    
	using Types;
    using Native;
    using Exceptions;

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
			this.Reset( m );
		}
        
        /// <summary>
        /// Prepares the converter to run appropriately
        /// </summary>
        /// <param name="m">The <see cref="Machine"/> the converter will work for.</param>
        public void Reset(Machine m)
        {
            this.Machine = m;
            
            this.CharSize = this.Machine.TypeSystem.GetCharType().Size;
            this.ShortSize = this.Machine.TypeSystem.GetShortType().Size;
            this.IntSize = this.Machine.TypeSystem.GetIntType().Size;
            this.LongSize = this.Machine.TypeSystem.GetLongType().Size;
            this.DoubleSize = this.Machine.TypeSystem.GetDoubleType().Size;
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
			return new []{ (byte) c };
		}

        /// <summary>
        /// Converts the given byte vector to a single short int.
        /// The byte sequence is expected to follow the machine's architecture.
        /// </summary>
        /// <returns>The short int, as a <see cref="BigInteger"/>.</returns>
        /// <param name="data">The data, as a vector of bytes.</param>
        public BigInteger FromBytesToShort(byte[] data)
        {
            return this.FromBytesToIntegral( data, this.ShortSize );
        }
        
        /// <summary>
        /// Converts the given byte vector to a single unsigned short int.
        /// The byte sequence is expected to follow the machine's architecture.
        /// </summary>
        /// <returns>The unsigned short int, as a <see cref="BigInteger"/>.</returns>
        /// <param name="data">The data, as a vector of bytes.</param>
        public BigInteger FromBytesToUShort(byte[] data)
        {
            return this.FromBytesToUnsignedIntegral( data, this.ShortSize );
        }
        
        /// <summary>
        /// Converts the given byte vector to a single int.
        /// The byte sequence is expected to follow the machine's architecture.
        /// </summary>
        /// <returns>The int, as a primitive int.</returns>
        /// <param name="data">The data, as a vector of bytes.</param>
        public BigInteger FromBytesToInt(byte[] data)
        {
            return this.FromBytesToIntegral( data, this.IntSize );
        }
        
        /// <summary>
        /// Converts the given byte vector to a single unsigned int.
        /// The byte sequence is expected to follow the machine's architecture.
        /// </summary>
        /// <returns>The int, as a primitive int.</returns>
        /// <param name="data">The data, as a vector of bytes.</param>
        public BigInteger FromBytesToUInt(byte[] data)
        {
            return this.FromBytesToUnsignedIntegral( data, this.IntSize );
        }
        
        /// <summary>
        /// Converts the given byte vector to a single <see cref="BigInteger"/>.
        /// The byte sequence is expected to follow the machine's architecture.
        /// </summary>
        /// <returns>The short int, as a <see cref="BigInteger"/>.</returns>
        /// <param name="data">The data, as a vector of bytes.</param>
        public BigInteger FromBytesToLong(byte[] data)
        {
            return this.FromBytesToIntegral( data, this.LongSize );
        }
        
        /// <summary>
        /// Converts the given byte vector to a single <see cref="BigInteger"/>.
        /// The byte sequence is expected to follow the machine's architecture.
        /// </summary>
        /// <returns>The short int, as a <see cref="BigInteger"/>.</returns>
        /// <param name="data">The data, as a vector of bytes.</param>
        public BigInteger FromBytesToULong(byte[] data)
        {
            return this.FromBytesToUnsignedIntegral( data, this.LongSize );
        }
        
		/// <summary>
		/// Converts the given byte vector to a single integral.
		/// The byte sequence is expected to follow the machine's architecture.
		/// </summary>
		/// <returns>The int, as a <see cref="BigInteger"/>.</returns>
		/// <param name="data">The data, as a vector of bytes.</param>
        /// <param name="length">The number of bytes to consider.</param>
		public BigInteger FromBytesToIntegral(byte[] data, int length)
        {
            BigInteger toret = 0;
            
            if ( data.Length != length ) {
				throw new RuntimeException(
                            "incompatible raw bytes length with passed length" );
            }
            
            // Reverse the data, if needed
            if ( BitConverter.IsLittleEndian != this.Machine.IsLittleEndian )
            {
				MemoryManager.SwitchEndiannessInBytes( this.Machine.WordSize, data );
            }
            
            // Convert by data size
            switch ( length ) {
                case 1:
                    toret = Convert.ToInt16( data[ 0 ] );
                    break;
                case 2:
                    toret = BitConverter.ToInt16( data, 0 );
                    break;
                case 4:
                    toret = BitConverter.ToInt32( data, 0 );
                    break;
                case 8:
                    toret = BitConverter.ToInt64( data, 0 );
                    break;
                case 16:
                    toret = Int128.FromBytes( data ).Value;
                    break;
             }
            
            return toret;
		}
        
        /// <summary>
        /// Converts the given byte vector to a single integral.
        /// The byte sequence is expected to follow the machine's architecture.
        /// </summary>
        /// <returns>The int, as a <see cref="BigInteger"/>.</returns>
        /// <param name="data">The data, as a vector of bytes.</param>
        /// <param name="length">The number of bytes to consider.</param>
        public BigInteger FromBytesToUnsignedIntegral(byte[] data, int length)
        {
            BigInteger toret = 0;
            
            if ( data.Length != length ) {
                throw new RuntimeException(
                            "incompatible raw bytes length with passed length" );
            }
            
            // Reverse the data, if needed
            if ( BitConverter.IsLittleEndian != this.Machine.IsLittleEndian )
            {
                MemoryManager.SwitchEndiannessInBytes( this.Machine.WordSize, data );
            }
            
            // Convert by data size
            switch ( length ) {
                case 1:
                    toret = Convert.ToUInt16( data[ 0 ] );
                    break;
                case 2:
                    toret = BitConverter.ToUInt16( data, 0 );
                    break;
                case 4:
                    toret = BitConverter.ToUInt32( data, 0 );
                    break;
                case 8:
                    toret = BitConverter.ToUInt64( data, 0 );
                    break;
                case 16:
					toret = UInt128.FromBytes( data ).Value;
                    break;
             }
            
            return toret;
        }
        
        /// <summary>
        /// Converts a <see cref="CSim.Core.Types.Primitives.Short"/> to an array of bytes.
        /// The byte sequence will be returned following the machine's architecture.
        /// </summary>
        /// <returns>The resulting array.</returns>
        /// <param name="value">Value to convert, as a <see cref="BigInteger"/>.</param>
        public byte[] FromShortToBytes(BigInteger value)
        {
            return this.FromIntegralToBytes( value, this.ShortSize );
        }
        
        /// <summary>
        /// Converts an <see cref="CSim.Core.Types.Primitives.UShort"/> to an array of bytes.
        /// The byte sequence will be returned following the machine's architecture.
        /// </summary>
        /// <returns>The resulting array.</returns>
        /// <param name="value">Value to convert, as a <see cref="BigInteger"/>.</param>
        public byte[] FromUShortToBytes(BigInteger value)
        {
            return this.FromUnsignedIntegralToBytes( value, this.ShortSize );
        }
        
        /// <summary>
        /// Converts an <see cref="CSim.Core.Types.Primitives.UInt"/> to an array of bytes.
        /// The byte sequence will be returned following the machine's architecture.
        /// </summary>
        /// <returns>The resulting array.</returns>
        /// <param name="value">Value to convert, as a <see cref="BigInteger"/>.</param>
        public byte[] FromUIntToBytes(BigInteger value)
        {
            return this.FromUnsignedIntegralToBytes( value, this.IntSize );
        }

		/// <summary>
		/// Converts an <see cref="CSim.Core.Types.Primitives.Int"/> to an array of bytes.
		/// The byte sequence will be returned following the machine's architecture.
		/// </summary>
		/// <returns>The resulting array.</returns>
		/// <param name="value">Value to convert, as a <see cref="BigInteger"/>.</param>
		public byte[] FromIntToBytes(BigInteger value)
		{
            return this.FromIntegralToBytes( value, this.IntSize );
        }
        
        /// <summary>
        /// Converts a <see cref="CSim.Core.Types.Primitives.Long"/> to an array of bytes.
        /// The byte sequence will be returned following the machine's architecture.
        /// </summary>
        /// <returns>The resulting array.</returns>
        /// <param name="value">Value to convert, as a <see cref="BigInteger"/>.</param>
        public byte[] FromLongToBytes(BigInteger value)
        {
            return this.FromIntegralToBytes( value, this.LongSize );
        }
        
        /// <summary>
        /// Converts a <see cref="CSim.Core.Types.Primitives.ULong"/> to an array of bytes.
        /// The byte sequence will be returned following the machine's architecture.
        /// </summary>
        /// <returns>The resulting array.</returns>
        /// <param name="value">Value to convert, as a <see cref="BigInteger"/>.</param>
        public byte[] FromULongToBytes(BigInteger value)
        {
            return this.FromUnsignedIntegralToBytes( value, this.LongSize );
        }
        
        /// <summary>
        /// Converts an integer to an array of bytes.
        /// The byte sequence will be returned following the machine's architecture.
        /// </summary>
        /// <returns>The resulting array.</returns>
        /// <param name="value">Value to convert, as a <see cref="BigInteger"/>.</param>
        /// <param name="length">The length of the integral type.</param>
        public byte[] FromIntegralToBytes(BigInteger value, int length)
        {
			byte[] toret = null;
            
            switch( length ) {
                case 1:
                    toret = new byte[]{ value.ToByte() };
                    break;
                case 2:
                    toret = BitConverter.GetBytes( value.ToInt16() );
                    break;
                case 4:
                    toret = BitConverter.GetBytes( value.ToInt32() );
                    break;
                case 8:
                    toret = BitConverter.GetBytes( value.ToInt64() );
                    break;
                case 16:
                    toret = value.ToInt128().Bytes;
                    break;
            }
            
            if ( toret == null ) {
                throw new RuntimeException(
                                "asked for missing conversion, int"
                                + length * 8 );
            }
            
			if ( BitConverter.IsLittleEndian != this.Machine.IsLittleEndian )
			{
				MemoryManager.SwitchEndiannessInBytes( this.Machine.WordSize, toret );
			}

            Array.Resize( ref toret, length );
			return toret;
		}
        
        /// <summary>
        /// Converts an unsigned int to an array of bytes.
        /// The byte sequence will be returned following the machine's architecture.
        /// </summary>
        /// <returns>The resulting array.</returns>
        /// <param name="value">Value to convert, as a <see cref="BigInteger"/>.</param>
        /// <param name="length">The length of the integral type.</param>
        public byte[] FromUnsignedIntegralToBytes(BigInteger value, int length)
        {
            value = BigInteger.Abs( value );
            byte[] toret = null;
            
            switch( length ) {
                case 1:
                    toret = new byte[]{ value.ToByte() };
                    break;
                case 2:
                    toret = BitConverter.GetBytes( value.ToUInt16() );
                    break;
                case 4:
                    toret = BitConverter.GetBytes( value.ToUInt32() );
                    break;
                case 8:
                    toret = BitConverter.GetBytes( value.ToUInt64() );
                    break;
                case 16:
                    toret = value.ToUInt128().Bytes;
                    break;
            }
            
            if ( toret == null ) {
                throw new RuntimeException(
                            "asked for missing conversion, int"
                            + length * 8 );
            }
            
            if ( BitConverter.IsLittleEndian != this.Machine.IsLittleEndian )
            {
                MemoryManager.SwitchEndiannessInBytes( this.Machine.WordSize, toret );
            }

            Array.Resize( ref toret, length );
            return toret;
        }
        
        /// <summary>
        /// Converts the given byte vector to a single float.
        /// Should apply IEEE 754.
        /// </summary>
        /// <returns>The float, as a primitive double.</returns>
        /// <param name="data">The data, as a vector of bytes.</param>
        public double FromBytesToFloat(byte[] data)
        {
            double toret;

			// Reverse, if needed
			if ( BitConverter.IsLittleEndian != this.Machine.IsLittleEndian )
			{
				MemoryManager.SwitchEndiannessInBytes( this.Machine.WordSize, data );
			}
            
            if ( data.Length < 4 ) {
                toret = FromBytesToFloat16( data );
            }
            else {
                toret = FromBytesToFloat32( data );
            }
            
            return toret;
        }
        
        private double FromBytesToFloat32(byte[] data)
        {
            return BitConverter.ToSingle( data, 0 );
        }
        
        private double FromBytesToFloat16(byte[] data)
        {
            var dataAsInt16 = new byte[] { data[ 0 ], data[ 1 ], 0, 0 };            
            var intVal = BitConverter.ToInt32( dataAsInt16, 0 );
        
            int mantissa = intVal & 0x03ff;
            int exponent = intVal & 0x7c00;
            
            if ( exponent == 0x7c00 ) {
                exponent = 0x3fc00;
            }
            else
            if ( exponent != 0 ) {
                exponent += 0x1c000;
                
                if ( mantissa == 0
                  && exponent > 0x1c400 )
                {
                    return BitConverter.ToSingle(
                                BitConverter.GetBytes( ( intVal & 0x8000) << 16 | exponent << 13 | 0x3ff ), 0 );
                }
            }
            else
            if ( mantissa != 0 )
            {
                exponent = 0x1c400;
                do {
                    mantissa <<= 1;
                    exponent -= 0x400;
                } while ((mantissa & 0x400) == 0);
                mantissa &= 0x3ff;
            }
            
            return BitConverter.ToSingle(
                                BitConverter.GetBytes( ( intVal & 0x8000) << 16 | (exponent | mantissa) << 13 ), 0 );
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
				MemoryManager.SwitchEndiannessInBytes( this.Machine.WordSize, data );
			}

			return BitConverter.ToDouble( data, 0 );
		}

		/// <summary>
		/// Converts a float to an array of bytes.
		/// </summary>
		/// <returns>The resulting array.</returns>
		/// <param name="value">Value to convert, as a primitive double.</param>
		public byte[] FromFloatToBytes(double value)
		{
			var toret = BitConverter.GetBytes( (float) value );

			if ( BitConverter.IsLittleEndian != this.Machine.IsLittleEndian )
			{
				MemoryManager.SwitchEndiannessInBytes( this.Machine.WordSize, toret );
			}

			return toret;
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
				MemoryManager.SwitchEndiannessInBytes( this.Machine.WordSize, toret );
            }

            return toret;
        }
        
		/// <summary>
		/// Converts a <see cref="AType"/> to its raw byte value.
		/// </summary>
		/// <returns>A byte representing the <see cref="AType"/>.</returns>
		/// <param name="t">An array of bytes (length == 1), with the binary representation.</param>
		public byte[] FromTypeToBytes(AType t)
		{
			byte toret;

            if ( t is TypeType ) {
                toret = byte.MaxValue;
            }
            else
            if ( t == Any.Get( t.Machine ) ) {
                toret = 0;
            } else {
                byte references = 0;
                byte indirections = 0;
                toret = 0;

                // Is it a reference?
                if ( t is Ref refType ) {
                    t = refType.AssociatedType;
                    references = 1;
                }

                // How many indirections?
                if ( t is Ptr ptrType )
                {
                    t = ptrType.AssociatedType;
                    indirections = (byte) ptrType.IndirectionLevel;
                }

                // Basic value
                if ( t != Any.Get( t.Machine ) ) {
                    toret += (byte) ( Primitive.GetPrimitiveNameIndex( t.Name ) + 1 );
                }
                           
                // Build the final type's value    
                references <<= 7;
                indirections <<= 5;
                toret += (byte) ( references + indirections );
            }

            return new []{ toret }; 
		}

		/// <summary>
		/// Converts a binary representation to its corresponding type.
		/// </summary>
		/// <returns>A <see cref="AType"/>.</returns>
		/// <param name="bytes">A raw binary representation (length == 1).</param>
		public AType FromBytesToType(byte[] bytes)
		{
			AType toret = null;
			int indirections = 0;
			int references = 0;
			byte x = bytes[ 0 ];
            
            // TypeType?
            if ( ( x & byte.MaxValue ) == byte.MaxValue ) {
                toret = this.Machine.TypeSystem.GetTypeType();
            } else {            
                // Decompose
                indirections = x & 96;   // 01100000
                indirections >>= 5;
                references = x & 128;    // 10000000
                references >>= 7;
                x = (byte) ( x & 31 );   // 00011111
                
                // Retrieve the appropiate basic type
                if ( x == 0 ) {
                    toret = Any.Get( this.Machine );
                } else {
                    toret = this.Machine.TypeSystem.GetBasicType(
                                Primitive.GetPrimitiveNameAt( x - 1 ) );
                }
    
                // Indirections
    			if ( indirections > 0 ) {
    				toret = this.Machine.TypeSystem.GetPtrType( toret, indirections );
    			}
    
                // References
    			if ( references > 0 ) {
    				toret = this.Machine.TypeSystem.GetRefType( toret );
    			}
    
    			// Chk
    			if ( toret == null ) {
    				throw new RuntimeException( "inconvertible type: " + x );
    			}
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
        
        /// <summary>
        /// Gets the size of a char.
        /// </summary>
        /// <value>The size of a char.</value>
        public int CharSize {
            get; private set;
        }
        
        /// <summary>
        /// Gets the size of a short int.
        /// </summary>
        /// <value>The size of a short int.</value>
        public int ShortSize {
            get; private set;
        }
        
        /// <summary>
        /// Gets the size of an int.
        /// </summary>
        /// <value>The size of a int.</value>
        public int IntSize {
            get; private set;
        }
        
        /// <summary>
        /// Gets the size of a long.
        /// </summary>
        /// <value>The size of a long.</value>
        public int LongSize {
            get; private set;
        }
        
        /// <summary>
        /// Gets the size of a double.
        /// </summary>
        /// <value>The size of a double.</value>
        public int DoubleSize {
            get; private set;
        }
	}
}
