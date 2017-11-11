// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.Native {
	using System;
	using System.Numerics;

	/// <summary>
	/// Represents unsigned integers of 128bits, big endian.
	/// </summary>
	public class UInt128 {
		/// <summary>The length in bytes for this integer's
		///  binary representation.</summary>
		public const int LengthInBytes = 16;
        
        /// <summary>Maximum number of different values in a single byte.</summary>
        public const int MaxByteValues = byte.MaxValue + 1;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Native.UInt128"/> class.
		/// </summary>
		/// <param name="value">The new value.</param>
		public UInt128(BigInteger value)
			:this()
		{
			for(int i = 0; i < ( LengthInBytes - 1 ); ++i) {
				var divisor = BigInteger.Pow( MaxByteValues, LengthInBytes - i - 1 );

				this.bytes[ i ] = (byte) ( value / divisor );
				value %= divisor;
			}

			this.bytes[ LengthInBytes - 1 ] = value.ToByte();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Native.UInt128"/> class.
		/// With a zero value.
		/// </summary>
		public UInt128()
		{
			this.bytes = new byte[ LengthInBytes ];
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <value>The value, as a <see cref="BigInteger"/>.</value>
		public BigInteger Value {
			get {
				BigInteger toret = BigInteger.Zero;

				for(int i = 0; i < LengthInBytes; ++i) {
					toret += this.bytes[ i ] * BigInteger.Pow( MaxByteValues, LengthInBytes - i - 1 );
				}

				return toret;
			}
		}

		/// <summary>
		/// Gets the binary representation.
		/// </summary>
		/// <value>A primitive byte array.</value>
		public byte[] Bytes {
			get {
				var toret = new byte[ LengthInBytes ];
				Array.Copy( this.bytes, toret, LengthInBytes );
				return toret;
			}
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Native.UInt128"/>.
		/// </summary>
		/// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Native.UInt128"/>.</returns>
		public override string ToString()
		{
			return string.Format( "{0}", this.Value );
		}
        
        /// <summary>
        /// Gets the minimum value.
        /// </summary>
        /// <value>The minimum value.</value>
        /// <seealso cref="Zero"/>
        public static UInt128 MinValue {
            get {
                return Zero;
            }
        }

		/// <summary>
		/// Gets the zero value for this type.
		/// </summary>
		/// <value>The zero.</value>
		public static UInt128 Zero {
			get {
				if ( zero == null ) {
					zero = new UInt128();
				}

				return zero;
			}
		}
        
        /// <summary>
        /// Gets the max value for this type.
        /// </summary>
        /// <value>The max value.</value>
        public static UInt128 MaxValue {
            get {
                if ( maxValue == null ) {
                    maxValue = new UInt128();
                    for(int i = 0; i < maxValue.bytes.Length; ++i) {
                        maxValue.bytes[ i ] = byte.MaxValue;
                    }
                }

                return maxValue;
            }
        }

        /// <summary>
        /// An implicit operator to be able to convert 64 bit
        /// integral numbers.
        /// </summary>
        /// <returns>A new <see cref="UInt128"/> value.</returns>
        /// <param name="value">The value to convert from.</param>
		public static implicit operator UInt128(UInt64 value)
		{
			return new UInt128( value );
		}
        
        /// <summary>
        /// An explicit operator to convert a <see cref="UInt128"/>
        /// to a <see cref="BigInteger"/>.
        /// </summary>
        /// <returns>A new <see cref="BigInteger"/> value.</returns>
        /// <param name="value">The value to convert from.</param>
        public static explicit operator BigInteger(UInt128 value)
        {
            return value.Value;
        }

		/// <summary>
		/// Creates a new value from raw bytes.
		/// </summary>
		/// <returns>A new integral value.</returns>
		/// <param name="data">A raw byte array.</param>
		public static UInt128 FromBytes(byte[] data)
		{
			var toret = new UInt128();
			int max = Math.Min( UInt128.LengthInBytes, data.Length );

			if ( BitConverter.IsLittleEndian ) {
				int pos = 0;
				for(int i = max; i >= 0; --i) {
					toret.bytes[ pos ] = data[ i ];
					++pos;
				}
			} else {
				for(int i = 0; i < max; ++i) {
					toret.bytes[ i ] = data[ i ];
				}
			}

			return toret;
		}

		private static UInt128 zero;
        private static UInt128 maxValue;
		private byte[] bytes;
	}
}
