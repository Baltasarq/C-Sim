// MemoryManager.cs
namespace CSim.Core {
	using System;
	using System.Collections.ObjectModel;
	using System.Collections.Generic;

	using CSim.Core.Exceptions;
	using CSim.Core.Types;
	using CSim.Core.Literals;

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
        public void CheckSizeFits(long pos, int size)
        {
            int maxMemory = this.Max;
            
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
		/// Read size bytes from position pos.
		/// </summary>
		/// <param name="pos">The position, as an int.</param>
		/// <param name="size">The size, as an int.</param>
        public byte[] Read(long pos, int size)
        {
            var toret = new byte[ size ];

			this.CheckSizeFits( pos, size );
			Array.Copy( this.raw, pos, toret, 0, size );

            return  toret;
        }

		/// <summary>
		/// Writes a vector of bytes (in all its length), starting from position pos.
		/// </summary>
		/// <param name="pos">The position, as an int.</param>
		/// <param name="value">The vector of bytes.</param>
        public void Write(long pos, byte[] value)
        {
            int size = value.Length;
            this.CheckSizeFits( pos, size );
			Array.Copy( value, 0, this.raw, pos, size );

            return;
        }

		/// <summary>
		/// Gets the upper limit for memory.
		/// </summary>
		/// <value>The max number of bytes, as an int.</value>
        public int Max {
            get { return this.raw.Length; }
        }

        /// <summary>
        /// Reads from memory the stored string.
        /// </summary>
        /// <param name="pos">The position to begin to read from.</param>
        /// <returns>The raw secquence of bytes.</returns>
        public byte[] ReadStringFromMemory(long pos)
        {
            var toret = new List<byte>();

            while( this.raw[ pos ] != 0 ) {
                this.CheckSizeFits( pos, 1 );
                toret.Add( this.raw[ pos ] );
                ++pos;
            }

            return toret.ToArray();
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
		/// Creates a literal from the memory contents, given an address and type.
		/// </summary>
		/// <param name="address">The address to read from.</param>
		/// <param name="type">The type, reporting the size.</param>
		public Literal CreateLiteral(long address, Type type)
		{
			Literal toret = null;
			var ptrType = type as Ptr;

			// Special case: char* is a StringLiteral
			if ( ptrType != null
			  && ptrType.AssociatedType == this.Machine.TypeSystem.GetCharType() )
			{
				byte[] str = this.ReadStringFromMemory( address );
				toret = new StrLiteral( this.Machine, new string( Machine.TextEncoding.GetChars( str ) ) );
			} else {
				toret = type.CreateLiteral( this.Machine, this.Read( address, type.Size ) );
			}

			if ( toret == null ) {
				throw new EngineException( L18n.Get( L18n.Id.ExcUnknownType ) );
			}

			return toret;
		}

		/// <summary>
		/// Extracts the values of an array from memory.
		/// </summary>
		public byte[][] ExtractArrayElementValues(Core.Type type, long address, long count)
		{
			var toret = new byte[ count ][];

			for (int i = 0; i < count; ++i) {
				toret[ i ] = this.Read( address, type.Size );
				address += type.Size;
			}

			return toret;
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

