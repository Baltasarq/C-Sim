// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core {
	using System;
    using System.Numerics;
	using System.Collections.ObjectModel;
	using System.Collections.Generic;

	using Exceptions;
	using Literals;

    /// <summary>
    /// Represents system memory.
    /// </summary>
    public class MemoryManager
    {
		/// <summary>
		/// Supported ways of resetting the memory.
		/// <seealso cref="Reset()"/>
		/// </summary>
		public enum ResetType { 
			///<summary>Random values</summary>
			Random,
			///<summary>Zeroes</summary>
			Zero
		};

        /// <summary>
        /// The default max memory.
        /// Either this value or the provided one must > 16 and % 16 == 0
        /// </summary>
        public const int DefaultMaxMemory = 512;

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.MemoryManager"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/> this memory manager belongs to.</param>
		public MemoryManager(Machine m)
			:this( m, DefaultMaxMemory )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.MemoryManager"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/> this memory manager belongs to.</param>
		/// <param name="max">The maximum amount of memory, in bytes. It must be rounded to 2^4.</param>
		public MemoryManager(Machine m, int max)
		{
			if ( max < 16
		     || ( max % 16 ) != 0 )
			{
				throw new InvalidMaxMemoryException( max.ToString() );
			}

			this.Machine = m;
			this.raw = new byte[ max ];
		}

		/// <summary>
		/// Resets memory with random values.
		/// </summary>
		public void Reset()
		{
			this.Reset( ResetType.Random );
		}

		/// <summary>
		/// Resets memory with the specified reset type.
		/// </summary>
		/// <param name="rt">Rt.</param>
        public void Reset(ResetType rt)
        {
			var rand = new Random();

            for(int i = 0; i < this.raw.Length; ++i) {
				if ( rt == ResetType.Random ) {
                    this.raw[ i ] = (byte) ( rand.Next() % byte.MaxValue );
				} else {
					this.raw[ i ] = 0;
				}
            }

            return;
        }

		/// <summary>
		/// Checks a given size in bytes, counting from a given memory position, fits in memory.
		/// </summary>
		/// <param name="pos">The position to check.</param>
		/// <param name="size">The size in bytes to check.</param>
        public void CheckSizeFits(BigInteger pos, int size)
        {
            BigInteger maxMemory = this.Max;
            
            if ( pos < 0
			  || pos > maxMemory
              || ( pos + size ) > maxMemory )
            {
                throw new ExhaustedMemoryException(
                    L18n.Get( L18n.Id.ErrAccessingAt )
					+ " " + new IntLiteral( this.Machine, pos ).ToHex() );
            }
            
            return;
        }
        
        /// <summary>
        /// Checks whether 0 &lt; address &lt; <see cref="MemoryManager.Max"/>.
        /// </summary>
        /// <param name="pos">Position.</param>
        public void CheckAddressFits(BigInteger pos)
        {
            if ( pos < 0
              || pos >= this.Max )
            {
                throw new IncorrectAddressException(
                    L18n.Get( L18n.Id.ErrAccessingAt )
                    + " " + new IntLiteral( this.Machine, pos ).ToHex() );
            }
            
            return;
        }

		/// <summary>
		/// Read size bytes from position pos.
		/// </summary>
		/// <param name="pos">The position, as an int.</param>
		/// <param name="size">The size, as an int.</param>
        public byte[] Read(BigInteger pos, int size)
        {
            var toret = new byte[ size ];

			this.CheckSizeFits( pos, size );
			Array.Copy( this.raw, (int) pos, toret, 0, size );

            return  toret;
        }

		/// <summary>
		/// Writes a vector of bytes (in all its length), starting from position pos.
		/// </summary>
		/// <param name="pos">The position, as an int.</param>
		/// <param name="value">The vector of bytes.</param>
        public void Write(BigInteger pos, byte[] value)
        {
            int size = value.Length;
            this.CheckSizeFits( pos, size );
			Array.Copy( value, 0, this.raw, (int) pos, size );

            return;
        }

        /// <summary>
        /// Reads from memory the stored string.
        /// </summary>
        /// <param name="pos">The position to begin to read from.</param>
        /// <returns>The raw secquence of bytes.</returns>
        public byte[] ReadStringFromMemory(BigInteger pos)
        {
            var toret = new List<byte>();

            while( this.raw[ (int) pos ] != 0 ) {
                this.CheckSizeFits( pos, 1 );
                toret.Add( this.raw[ (int) pos ] );
                ++pos;
            }

            return toret.ToArray();
        }

		/// <summary>
		/// Creates a literal from the memory contents, given an address and type.
		/// </summary>
		/// <param name="address">The address to read from.</param>
		/// <param name="type">The type, reporting the size.</param>
		public Literal CreateLiteral(BigInteger address, AType type)
		{
			Literal toret = null;

			// Special case: char* is a StringLiteral
			if ( type == this.Machine.TypeSystem.GetPCharType() ) {
				byte[] str = this.ReadStringFromMemory( address );
				toret = new StrLiteral(
                            this.Machine,
                            new string( Machine.TextEncoding.GetChars( str ) ) );
			} else {
				toret = type.CreateLiteral( this.Read( address, type.Size ) );
			}

			if ( toret == null ) {
				throw new RuntimeException( L18n.Get( L18n.Id.ExcUnknownType ) );
			}

			return toret;
		}

		/// <summary>
		/// Extracts the values of an array from memory.
		/// </summary>
		public byte[][] ExtractArrayElementValues(AType type, BigInteger address, BigInteger count)
		{
			var toret = new byte[ (int) count ][];

			for (int i = 0; i < count; ++i) {
				toret[ i ] = this.Read( address, type.Size );
				address += type.Size;
			}

			return toret;
		}

        /// <summary>
        /// Reverse the endianness' value at the specified address and type.
        /// </summary>
        /// <param name="address">The address of the value to reverse endianness.</param>
        /// <param name="size">The number of bytes affected.</param>
        public void SwitchEndianness(BigInteger address, int size)
        {
            if ( size > 1 ) {
				byte[] rawValue = this.Read( address, size );
				this.Write( address, SwitchEndiannessInBytes( this.Machine.WordSize, rawValue ) );
            }

            return;
        }
        
        /// <summary>
        /// Switchs the endianness in bytes.
        /// </summary>
        /// <returns>The new sequence of bytes.</returns>
        /// <param name="data">A raw sequence of bytes.</param>
        /// <param name="wordSize">The size of the machine's word, in bytes.</param>
        public static byte[] SwitchEndiannessInBytes(int wordSize, byte[] data)
        {
            byte[] toret = data;
            int size = data.Length;

            if ( size > 1 ) {
                if ( data.Length < wordSize ) {
                    wordSize = data.Length;
                }

                int i = 0;
                do {
                    Array.Reverse( toret, i, wordSize );
                    i += wordSize;
                } while( i < toret.Length );
            }

            return toret;
        }
        
        /// <summary>
        /// Gets the whole memory.
        /// </summary>
        /// <value>
        /// The memory, as a vector of char.
        /// </value>
        public ReadOnlyCollection<byte> Raw {
            get {
                byte[] toret = new byte[ this.raw.Length ];

                Array.Copy( this.raw, toret, this.raw.Length );
                return new ReadOnlyCollection<byte>( toret );
            }
        }
        
        /// <summary>
        /// Gets the upper limit for memory.
        /// </summary>
        /// <value>The max number of bytes, as a <see cref="BigInteger"/>.</value>
        public BigInteger Max {
            get { return new BigInteger( this.raw.Length ); }
        }

		/// <summary>
		/// Gets the machine owning this memory.
		/// </summary>
		/// <value>The machine.</value>
		public Machine Machine {
			get; private set;
		}

        private byte[] raw;
    }
}

