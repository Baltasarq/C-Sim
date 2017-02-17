
namespace CSim.Core {
	using System;
	using System.Text;

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
			/// <summary>Big endianness.</summary>
			BigEndian,
			/// <summary>Little endianness.</summary>
			LittleEndian
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
		/// <param name="endianness">The endiannes of the machine.</param>
		public Machine(int wordSize, int maxMemory, Endianness endianness)
		{
			this.endianness = endianness;
			this.wordSize = CalculateWordSize( wordSize );

			this.TypeSystem = new TypeSystem( this );
            this.Memory = new MemoryManager( this, maxMemory );
			this.TDS = new SymbolTable( this ) { AlignVbles = true };
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
            return this.WordSizeInBits + @"bit";
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
        /// Switchs the endianness of this machine.
        /// </summary>
        public void SwitchEndianness()
        {
            var endianess = this.Endian;

            if ( endianness == Endianness.BigEndian ) {
                this.Endian = Endianness.LittleEndian;
            } else {
                this.Endian = Endianness.BigEndian;
            }

            return;
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
			return this.Execute( new Parser( input, this ) );
		}

		/// <summary>
		/// Executed the parser input.
		/// </summary>
		/// <param name="parser">A <see cref="Parser"/> to parse the input.</param>
		public Variable Execute(Parser parser)
		{
			var er = new SnapshotManager( this );
			Variable toret = null;

			this.TDS.CollectArrayElements();

			er.SaveSnapshot();
			this.ExecutionStack.Clear();

			try {
				// Execute opcodes
				foreach(Opcode opcode in parser.Parse()) {
					opcode.Execute();
				}

				toret = this.ExecutionStack.Pop().SolveToVariable();

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

		/// <summary>
		/// Gets the execution stack.
		/// This is the stack used while executing opcodes.
		/// </summary>
		/// <value>The execution stack.</value>
		public ExecutionStack ExecutionStack {
			get; private set;
		}

		/// <summary>
		/// Gets the random engine.
		/// </summary>
		/// <value>The random engine, as a <see cref="Random"/> instance.</value>
		public Random Random {
			get; private set;
		}

		/// <summary>
		/// Gets the snapshot manager.
		/// </summary>
		/// <value>The snapshot manager, as a <see cref="SnapshotManager"/> instance.</value>
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
                this.TDS.SwitchEndianness();
			}
		}

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:CSim.Core.Machine"/> aligns vbles.
        /// </summary>
        /// <value><c>true</c> if aligns vbles; otherwise, <c>false</c>.</value>
        public bool AlignVbles {
            get; set;
        }

		/// <summary>
		/// Gets the type system for this machine.
		/// </summary>
		/// <value>The type system, as a <see cref="TypeSystem"/> object.</value>
		public TypeSystem TypeSystem {
			get; private set;
		}

		/// <summary>
		/// Gets the byte converter engine.
		/// </summary>
		/// <value>The <see cref="ByteConverter"/>.</value>
		public ByteConverter Bytes {
			get; private set;
		}

		private int wordSize;
		private Endianness endianness;
    }
}
