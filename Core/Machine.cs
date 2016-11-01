
namespace CSim.Core {
	using System;
	using System.Text;
	using System.Collections.Generic;

    /// <summary>
    /// Represents the target machine emulated
    /// </summary>
    public class Machine {
		public class ByteConverter {
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
				int wordSize = this.Machine.WordSize;
				int destLength = data.Length < sizeof(long) ? sizeof(long) : data.Length;

				// Create dest array
				var bytes = new byte[ destLength ];

				// Create org vector, if needed
				if ( data.Length != wordSize ) {
					var newData = new byte[ wordSize ];

					Array.Copy(
						data, 0,
						newData, Math.Max( 0, wordSize - data.Length ),
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
					wordSize );
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
				int wordSize = this.Machine.WordSize;
				var toret = BitConverter.GetBytes( value );

				// Limit the byte array to wordsize
				if ( toret.Length > wordSize ) {
					var bytes = new byte[ this.Machine.WordSize ];

					if ( !BitConverter.IsLittleEndian ) {
						Array.Copy( toret, wordSize, bytes, 0, wordSize );
						toret = bytes;
						bytes = new byte[ this.Machine.WordSize ];
					}

					Array.Copy( toret, 0, bytes, 0, wordSize );
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

			public Machine Machine {
				get; private set;
			}
		}


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
			this.endianness = endianness;
			this.wordSize = CalculateWordSize( wordSize );

			this.TypeSystem = new TypeSystem( this );
			this.Memory = new MemoryManager( this, maxMemory );
			this.TDS = new SymbolTable( this );
			this.API = new StdLib( this );
			this.SetRandomEngine();
			this.ExecutionStack = new ExecutionStack();
			this.SnapshotManager = new SnapshotManager( this );
			this.Bytes = new ByteConverter( this );
		}

		/// <summary>
		/// Prepares the machine for execution
		/// from a new, clear status.
		/// </summary>
		public void Reset(MemoryManager.ResetType rt)
		{
			this.Memory.Reset( rt );
			this.SnapshotManager.Reset();
			this.TDS.Reset();
			this.TypeSystem.Reset();
		}

		/// <summary>
		/// Sets the random engine so it generates a predictable sequence.
		/// </summary>
		public void SetRandomEngine() {
			this.Random = new Random();
		}

		/// <summary>
		/// Sets the random engine with a given seed.
		/// </summary>
		/// <param name="seed">The seed to initialize the random generator, as an int.</param>
		public void SetRandomEngine(long seed) {
			this.Random = new Random( (int) seed );
		}

		/// <summary>
		/// Calculates the size of the word, in bytes, given the proposed size.
		/// The result will be equal or greater than the proposed size,
		/// in case that the proposed size is not a multiple of 4.
        /// The minimum value is 2 (16 bits).
		/// </summary>
		/// <returns>The word size, in bytes, as an int.</returns>
		/// <param name="ws">The proposed wordsize, in bytes, as int.</param>
		public static int CalculateWordSize(int ws)
		{
            int toret = Math.Max( 2, ws );

            // Assure it is multiple of four, or exactly two (16bits)
            while ( toret != 2
               && ( toret % 2 ) != 0 )
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
            return this.WordSizeInBits.ToString() + @"bit";
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
				this.wordSize = CalculateWordSize( value );
				this.Reset( MemoryManager.ResetType.Zero );
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
				return ( this.Endian == Endianness.LittleEndian );
			}
		}

		/// <summary>
		/// Gets a value indicating whether this machine is big endian.
		/// </summary>
		/// <value><c>true</c> if this instance is little endian; otherwise, <c>false</c>.</value>
		public bool IsBigEndian {
			get {
				return ( this.Endian == Endianness.BigEndian );
			}
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
		/// Parses given input and executes its opcodes.
		/// </summary>
		/// <param name="input">The user's input, as a string.</param>
		public Variable Execute(string input)
		{
			var er = new SnapshotManager( this );
			Variable toret = null;
			var parser = new Parser( input, this );

			er.SaveSnapshot();
			this.ExecutionStack.Clear();

			try {
				// Execute opcodes
				foreach(Opcode opcode in parser.Parse()) {
					opcode.Execute();
				}

				toret = (Variable) this.ExecutionStack.Pop();

				// Create snapshot
				this.SnapshotManager.SaveSnapshot();
			}
			catch(EngineException) {
				er.ApplySnapshot( 0 );
				throw;
			}
			finally {
				this.TDS.Collect();
			}

			return toret;
		}

		public ExecutionStack ExecutionStack {
			get; private set;
		}

		public Random Random {
			get; private set;
		}

		public SnapshotManager SnapshotManager {
			get; private set;
		}

		/// <summary>
		/// Gets the memory of the system.
		/// </summary>
		/// <value>The memory, as a <see cref="MemoryManager"/> object.</value>
		public MemoryManager Memory {
			get; private set;
		}

		/// <summary>
		/// Gets the symbol table, for all variables in the system.
		/// </summary>
		/// <value>The TDS, as a <see cref="SymbolTable"/> object.</value>
		public SymbolTable TDS {
			get; private set;
		}

		/// <summary>
		/// Gets the API of the system.
		/// </summary>
		/// <value>The API, as a <see cref="StdLib"/> object.</value>
		public StdLib API {
			get; private set;
		}

		/// <summary>
		/// Gets the endianness of this machine.
		/// </summary>
		/// <value>The endianness, as a value of the <see cref="Endianness"/> enum.</value>
		public Endianness Endian {
			get {
				return this.endianness;
			}
			set {
				this.endianness = value;
				this.Reset( MemoryManager.ResetType.Zero );
			}
		}

		/// <summary>
		/// Gets the type system for this machine.
		/// </summary>
		/// <value>The type system, as a <see cref="TypeSystem"/> object.</value>
		public TypeSystem TypeSystem {
			get; private set;
		}

		public ByteConverter Bytes {
			get; private set;
		}

		private int wordSize;
		private Endianness endianness;
    }
}
