using System;
using System.Text;

namespace CSim.Core
{
    /// <summary>
    /// Represents the target machine emulated
    /// </summary>
    public class Machine {
        /// <summary>
        /// The default width of the system, in bytes.
        /// </summary>
		public static int DefaultWordSize = 4;

		///<summary>
		/// The Endianness of this machine.
		/// </summary>
		public enum Endianness {
			BigEndian, LittleEndian
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Machine"/> class.
		/// By default, 32 bit machine with 512 bytes RAM.
		/// </summary>
		public Machine()
			:this( DefaultWordSize, MemoryManager.DefaultMaxMemory, Endianness.LittleEndian )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Machine"/> class.
		/// </summary>
		/// <param name="wordSize">The word size, in bytes.</param>
		/// <param name="maxMemory">Max memory.</param>
		public Machine(int wordSize, int maxMemory, Endianness endianness)
		{
			// Prepare type system
			this.endianness = endianness;
			this.wordSize = this.CalculateWordSize( wordSize );
			this.typeSystem = new TypeSystem( this );

			// Init subsystems
			this.memory = new MemoryManager( this, maxMemory );
			this.tds = new SymbolTable( this );
			this.stdLib = new StdLib( this );
		}

		/// <summary>
		/// Calculates the size of the word, in bytes, given the proposed size.
		/// The result will be equal or greater than the proposed size,
		/// in case that the proposed size is not a multiple of 4.
        /// The minimum value is 2 (16 bits).
		/// </summary>
		/// <returns>The word size, in bytes, as an int.</returns>
		/// <param name="ws">The proposed wordsize, in bytes, as int.</param>
		public int CalculateWordSize(int ws)
		{
            int toret = Math.Max( 2, ws );

            // Assure it is multiple of four, or exactly two (16bits)
            while ( toret != 2
               && ( toret % 4 ) != 0 )
            {
                toret <<= 1;
            }

			return toret;
		}

		/// <summary>
		/// Gets the memory width in this system.
		/// </summary>
		/// <returns>
		/// The memory width, as a string with the format:
		/// W[bits]bit
		/// For example: W32bit
		/// </returns>
		public string GetMemoryWidth()
		{
            return "w" + this.WordSizeInBits.ToString() + @"bit";
		}

        /// <summary>
        /// Gets or sets the size of a word (in bytes) for this system.
		/// Setting the word size implies a full reset of the machine.
        /// </summary>
        /// <value>The size of the word in bytes, as an int.</value>
        public int WordSize {
            get {
                return this.wordSize;
            }
			set {
				this.wordSize = this.CalculateWordSize( value );
				this.TypeSystem.Reset();
				this.Reset();
			}
        }

		/// <summary>
		/// Gets the word size in bits.
		/// </summary>
		/// <value>The word size in bits, as an int.</value>
		public int WordSizeInBits {
			get {
				return ( this.WordSize * 8 );
			}
		}

		/// <summary>
		/// Gets a value indicating whether this machine is little endian.
		/// </summary>
		/// <value><c>true</c> if this instance is little endian; otherwise, <c>false</c>.</value>
		public bool IsLittleEndian {
			get {
				return ( this.endianness == Endianness.LittleEndian );
			}
		}

		/// <summary>
		/// Gets a value indicating whether this machine is big endian.
		/// </summary>
		/// <value><c>true</c> if this instance is little endian; otherwise, <c>false</c>.</value>
		public bool IsBigEndian {
			get {
				return ( this.endianness == Endianness.BigEndian );
			}
		}

		/// <summary>
		/// Gets the endianness of this machine.
		/// </summary>
		/// <value>The endianness, as a value of the <see cref="Endianness"/> enum.</value>
		public Endianness Endian {
			get {
				return this.endianness;
			}
		}

        /// <summary>
        /// Gets the symbol table, for all variables in the system.
        /// </summary>
        /// <value>The TDS, as a <see cref="SymbolTable"/> object.</value>
        public SymbolTable TDS {
            get {
                return this.tds;
            }
        }

        /// <summary>
        /// Gets the memory of the system.
        /// </summary>
        /// <value>The memory, as a <see cref="MemoryManager"/> object.</value>
        public MemoryManager Memory {
            get {
                return this.memory;
            }
        }

        /// <summary>
        /// Gets the API of the system.
        /// </summary>
        /// <value>The API, as a <see cref="StdLib"/> object.</value>
		public StdLib Api {
			get {
                return this.stdLib;
            }
		}

		/// <summary>
		/// Gets the type system for this machine.
		/// </summary>
		/// <value>The type system, as a <see cref="TypeSystem"/> object.</value>
		public TypeSystem TypeSystem {
			get {
				return this.typeSystem;
			}
		}

		/// <summary>
		/// Prepares the machine for execution
		/// from a new, clear status.
		/// </summary>
		public void Reset()
		{
			this.TDS.Reset();
		}

        /// <summary>
        /// Gets the system's text encoding.
        /// </summary>
        /// <value>The text encoding, as an <see cref="Encoding"/> object.</value>
        public static Encoding TextEncoding {
            get {
                return Encoding.GetEncoding( "ISO-8859-1" );
            }
        }

		/// <summary>
		/// Converts the given byte vector to a single char.
		/// </summary>
		/// <returns>The char, as a primitive char.</returns>
		/// <param name="data">The data, as a vector of bytes.</param>
		public char CnvtBytesToChar(byte[] data)
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
		public byte[] CnvtCharToBytes(char c)
		{
			var toret = new byte[ 1 ];

			toret[ 0 ] = (byte) c;
			return toret;
		}

		/// <summary>
		/// Converts the given byte vector to a single int.
		/// The byte sequence is expected to be big endian (net ordering).
		/// </summary>
		/// <returns>The int, as a primitive int.</returns>
		/// <param name="data">The data, as a vector of bytes following little endianness.</param>
		public int CnvtBytesToInt(byte[] data)
		{
			if ( BitConverter.IsLittleEndian != this.IsLittleEndian )
			{
				var bytes = new byte[ data.Length ];
				Array.Copy( data, bytes, data.Length );
				data = bytes;
			}

			return BitConverter.ToInt32( data, 0 );
		}

		/// <summary>
		/// Converts an int to an array of bytes.
		/// The byte sequence will be returned as big endian (net ordering).
		/// </summary>
		/// <returns>The resulting array.</returns>
		/// <param name="value">Value to convert, as a primitive int.</param>
		public byte[] CnvtIntToBytes(int value)
		{
			var toret = BitConverter.GetBytes( value );

			if ( BitConverter.IsLittleEndian != this.IsLittleEndian )
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
		public double CnvtBytesToDouble(byte[] data)
		{
			if ( BitConverter.IsLittleEndian != this.IsLittleEndian )
			{
				var bytes = new byte[ data.Length ];
				Array.Copy( data, bytes, data.Length );
				data = bytes;
			}

			return BitConverter.ToDouble( data, 0 );
		}

		/// <summary>
		/// Converts a double to an array of bytes.
		/// </summary>
		/// <returns>The resulting array.</returns>
		/// <param name="value">Value to convert, as a primitive double.</param>
		public  byte[] CnvtDoubleToBytes(double value)
		{
			var toret = BitConverter.GetBytes( value );

			if ( this.IsLittleEndian != BitConverter.IsLittleEndian )
			{
				Array.Reverse( toret );
			}

			return toret;
		}

		private int wordSize;
		private MemoryManager memory;
		private SymbolTable tds;
		private StdLib stdLib;
		private Endianness endianness;
		private TypeSystem typeSystem;
    }
}
