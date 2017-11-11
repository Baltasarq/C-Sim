// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core {
    using System;
	using System.Numerics;
    
    /// <summary>Represents 64 bits integer values.</summary>
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

        /// <summary>Gets the value as a <see cref="BigInteger"/>.</summary>
		public BigInteger Value {
			get {
				return new BigInteger();
			}
		}

		/// <summary>Gets the representation in bytes.</summary>
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
