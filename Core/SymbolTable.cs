
namespace CSim.Core {
	using System;
	using System.Collections.ObjectModel;
	using System.Collections.Generic;

	using CSim.Core.Exceptions;
	using CSim.Core.Variables;
	using CSim.Core.Types;

	/// <summary>All the variables in the <see cref="Machine"/> reside here.</summary>
    public class SymbolTable {
		/// <summary>Prefix to use for memory blocks (heap)</summary>
        public const string MemBlockName = "_mblk#";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.SymbolTable"/> class,
		/// for a given <see cref="Machine"/>.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/> this symbol table will be used in.</param>
        public SymbolTable(Machine m)
        {
            this.Machine = m;
            this.tdsIds = new Dictionary<string, Variable>();
            this.tdsAddresses = new Dictionary<long, Variable>();
            this.addresses = new List<long>();
        }

		/// <summary>
		/// Resets the symbol table: no registered variables.
		/// Of course, this does not affect memory.
		/// <seealso cref="MemoryManager"/>
		/// </summary>
		public void Reset()
		{
			this.tdsIds.Clear();
            this.tdsAddresses.Clear();
			this.addresses.Clear();
		}

        /// <summary>
        /// Creates the addresses to fill array,
        /// containing the total of needed addresses for
        /// a given <see cref="Variable"/>.
        /// For example, an int variable starting at 100, with
        /// wordSize == 4 (32bit), will be: [100, 101, 102, 103].
        /// </summary>
        /// <returns>The addresses to fill, as an array.</returns>
        /// <param name="address">The address in which the variable could sit.</param>
        /// <param name="size">The size needed by the <see cref="Variable"/>.</param>
        private long[] CreateAddressesToFill(long address, int size)
        {
            long[] toret = new long[ size ];

            // Create addresses to fill
            for (int i = 0; i < size; ++i) {
                toret[ i ] = address + i;
            }

            return toret;
        }

        /// <summary>
        /// Stores the addresses occupied by the variable
        /// in the list of occupied addresses.
        /// </summary>
        /// <param name="v">V.</param>
        private void StoreAddressesToFill(Variable v)
        {
            long[] addressesToOccupy = this.CreateAddressesToFill( v.Address, v.Size );

            // Store the addresses to fill
            for (int i = 0; i < addressesToOccupy.Length; ++i) {
                this.addresses.Add( addressesToOccupy[ i ] );
            }
        }

        /// <summary>
        /// Registers the specified variable.
        /// </summary>
        /// <param name="v">The <see cref="Variable"/> to register.</param>
        private void Register(Variable v)
        {
            this.tdsIds.Add( v.Name.Name, v );
            this.tdsAddresses.Add( v.Address, v );
            this.StoreAddressesToFill( v );
        }

        /// <summary>
        /// Adds the variable, honoring the address registered inside it.
        /// </summary>
        /// <param name="v">The <see cref="Variable"/> to add</param>
        public void AddVariableInPlace(Variable v)
        {
            this.Add( v, true );
        }

		/// <summary>
		/// Adds the specified <see cref="Variable"/>, already created.
		/// </summary>
		/// <param name="v">The <see cref="Variable"/> to add.</param>
        /// <param name="inPlace">Determines whether to honor the address stored in
        /// the given <see cref="Variable"/>.</param>
        public void Add(Variable v, bool inPlace = false)
		{
            // Chk
			if ( this.IsIdOfExistingVariable( v.Name ) ) {
				throw new AlreadyExistingVbleException( v.Name.Name );
			}

            // Store
            if ( !inPlace ) {
                v.Address = this.Reserve( v );
            }

            this.Register( v );
        }

		/// <summary>
		/// Remove the specified id from the variables list.
		/// </summary>
		/// <param name='id'>
		/// The identifier of the variable.
		/// </param>
		public void Remove(string id)
		{
            Variable vbleToRemove;

            if ( this.tdsIds.TryGetValue( id, out vbleToRemove ) ) {
			    this.tdsIds.Remove( id );
                this.tdsAddresses.Remove( vbleToRemove.Address );

                // Remove affected addresses
                int i = 0;
                while( i < this.addresses.Count ) {
                    if ( vbleToRemove.IsStoredIn( this.addresses[ i ] ) ) {
                        this.addresses.RemoveAt( i );
                    } else {
                        ++i;
                    }
                }
            } else {
                throw new UnknownVbleException( id );
            }

            return;
		}

		/// <summary>
		/// Removes all the temp variables.
		/// Removes dangling references. i.e., references not set.
		/// References must point to some variable on initialization.
		/// It does not make sense to have an unset reference, while it
		/// is possible to have an unset pointer.
		/// After execution, this method should be called to remove
		/// all dangling references.
		/// </summary>
		public void Collect()
		{
			var spurious = new List<Variable>();

			// Find
			foreach (Variable v in this.Variables) {
				var tempVble = v as TempVariable;
				var refVble = v as RefVariable;

				if ( tempVble != null ) {
					spurious.Add( tempVble );
				}
				else
				if ( refVble != null
				  && !( refVble.IsSet() ) )
				{
					spurious.Add( refVble );
				}
			}

			// Remove
			foreach(Variable r in spurious) {
				this.Remove( r.Name.Name );
			}

			return;
		}

		/// <summary>
		/// Collects the array elements.
		/// It is not part of garbage collection, since array elements
		/// can be used to draw the diagram.
		/// </summary>
		public void CollectArrayElements()
		{
			var arrayElements = new List<ArrayElement>();

			// Find
			foreach (Variable v in this.Variables) {
				var r = v as ArrayElement;

				if ( r != null ) {
					arrayElements.Add( r );
				}
			}

			// Remove
			foreach(ArrayElement r in arrayElements) {
				this.Remove( r.Name.Name );
			}

			return;
		}

        /// <summary>
        /// Determines whether the given location in memory is free or not,
        /// considering the initial position and the needed size.
        /// </summary>
        /// <returns><c>true</c>, if location is free, <c>false</c> otherwise.</returns>
        /// <param name="address">The starting address.</param>
        /// <param name="size">The needed size.</param>
        private bool IsLocationFree(long address, int size)
        {
            long[] addressesToOccupy = this.CreateAddressesToFill( address, size );
            int i = 0;

            for (; i < addressesToOccupy.Length; ++i) {
                if ( this.addresses.Contains( addressesToOccupy[ i ] ) ) {
                    break;
                }
            }

            return ( i >= addressesToOccupy.Length );
        }

        /// <summary>
		/// Reserve memory for the specified <see cref="Variable"/> v.
        /// </summary>
        /// <param name='v'>
		/// The <see cref="Variable"/> to reserve memory for.
        /// </param>
        public long Reserve(Variable v)
        {
            long toret;

            if ( this.AlignVbles ) {
                toret = this.AlignedReserve( v );
            } else {
                toret = this.AleaReserve( v );
            }

            return toret;
        }

        private long AlignedReserve(Variable v)
        {
            int toret = -1;
            int address = this.Machine.WordSize;

            while( address < this.Memory.Max ) {
                if ( this.LookForAddress( address ) == null
                  && this.IsLocationFree( address, v.Size ) )
                {
                    toret = address;
                    break;
                }

                address += v.Size;
            }

            // Re-check
            if ( toret < 0 ) {
                throw new ExhaustedMemoryException(
                    L18n.Get( L18n.Id.ErrReserving ) + ": " + v.Name );
            }

            return toret;
        }

        private long AleaReserve(Variable v)
        {
            int tries = 100;
            var randomEngine = new Random();
            int toret = -1;

            // Generate memory location
            do {
				toret = randomEngine.Next( 0, this.Memory.Max - this.Machine.WordSize );

                if ( ( toret + v.Size ) >= this.Memory.Max
                  || !this.IsLocationFree( toret, v.Size ) )
                {   
                    toret = -1;
                    --tries;
                    if ( tries < 0 ) {
                        break;
                    }
                }
            } while( toret < 0 );

            // Re-check
            if ( toret < 0 ) {
                throw new ExhaustedMemoryException( 
                    L18n.Get( L18n.Id.ErrReserving ) + ": " + v.Name );
            }

            return toret;
        }

		/// <summary>
		/// Gets all variables.
		/// </summary>
		/// <value>
		/// Returns pointers and variables
		/// </value>
        public ReadOnlyCollection<Variable> Variables {
            get {
                var toret = new Variable[ this.tdsIds.Count ];

                this.tdsIds.Values.CopyTo( toret, 0 );
                return new ReadOnlyCollection<Variable>( toret );
            }
        }

        /// <summary>
        /// Looks for a variable, given its id.
        /// </summary>
        /// <returns>
        /// The variable corresponding to that id, null otherwise.
        /// </returns>
        /// <param name='id'>
        /// The identifier of the variable to look for.
        /// </param>
        private Variable LookForVariableOfId(string id)
		{
			Variable toret = null;

			if ( !string.IsNullOrEmpty( id ) ) {
                // Look for vble
                this.tdsIds.TryGetValue( id, out toret );
            }

			return toret;
        }

		/// <summary>
		/// Looks up a variable.
		/// </summary>
		/// <returns>
		/// The variable, if found. Core.Exceptions.UnknownVariableException otherwise.
		/// </returns>
		/// <param name='idVble'>
		/// The identifier, as a string.
		/// </param>
		/// <exception cref="UnknownVbleException">if the id does not correspond to any variable</exception>
		public Variable LookUp(string idVble)
		{
			Variable toret = null;

			// Look for variable
			toret = this.LookForVariableOfId( idVble );

			if ( toret == null ) {
				throw new UnknownVbleException( idVble );
			}

			return toret;
		}

		/// <summary>
		/// Determines whether there is a variable with an identifier like the specified one.
		/// </summary>
		/// <returns><c>true</c> if there is a variable with that identifier; otherwise, <c>false</c>.</returns>
		/// <param name="idVble">The identifier in question, as a string.</param>
		public bool IsIdOfExistingVariable(Id idVble)
		{
			return ( this.LookForVariableOfId( idVble.Name ) != null );
		}

		/// <summary>
		/// Looks for a variable, given its address.
		/// </summary>
		/// <returns>
		/// The variable, as a Variable object.
		/// </returns>
		/// <param name='address'>
		/// The address to look for.
		/// </param>
		public Variable LookForAddress(long address)
		{
            Variable toret;

            if ( !this.tdsAddresses.TryGetValue( address, out toret ) )
            {
                toret = null;
            }

			return toret;
		}

		/// <summary>
		/// Returns the variable pointed by the pointer variable, or a
		/// temporary variable for that address.
		/// </summary>
		/// <returns>A temporary or a true variable</returns>
		/// <param name="ptrVble">A PtrVariable object.</param>
		public Variable GetPointedValueAsVariable(PtrVariable ptrVble)
		{
            long address = ptrVble.IntValue.GetValueAsLongInt();
            AType requiredType = ptrVble.AssociatedType;
            Variable toret = this.LookForAddress( address );

            if ( toret != null ) {
                // Determine whether the required type is the same one
                AType targetType = toret.Type;
                var ptrType = targetType as Ptr;

                if ( ptrType != null ) {
                    targetType = ptrType.AssociatedType;
                }

                if ( requiredType != targetType ) {
                    toret = null;
                }
            }

            if ( toret == null ) {
                toret = new InPlaceTempVariable( requiredType );
				toret.Address = address;
				this.Machine.TDS.AddVariableInPlace( toret );
            }

			return toret;
		}

		/// <summary>
		/// Deletes a memory block given a memory address.
		/// Looks for the pointed variable, which must be on heap.
		/// </summary>
		/// <param name="address">The address of the memory block, as an int.</param>
		/// <exception cref="IncorrectAddressException">when the memory pointed is not on heap.</exception>
		public void DeleteBlk(long address)
		{
			Variable pointedVble = this.Machine.TDS.LookForAddress( address );

			if ( pointedVble != null ) {
				if ( pointedVble.IsInHeap ) {
					this.Machine.TDS.Remove( pointedVble.Name.Name );
				} else {
					throw new IncorrectAddressException(
						L18n.Get( L18n.Id.ErrMemoryNotInHeap )
						+ ": " + pointedVble.LiteralValue.ToPrettyNumber()
						);
				}
			}

			return;
		}

		/// <summary>
		/// Gets the name of the next memory block,
		/// honoring the number of already created ones.
		/// </summary>
		/// <returns>The next memory block name, as a string.</returns>
        public static string GetNextMemoryBlockName()
        {
            ++numMemBlock;
            return ( MemBlockName + numMemBlock );
        }

        /// <summary>
        /// Switchs the endianess of all variables in this machine.
        /// The machine's endianness (<see cref="Machine.Endian"/>
        /// is assumed to have already changed.
        /// </summary>
        public void SwitchEndianness()
        {
            foreach (Variable vble in this.Variables) {
                var arrayVble = vble as ArrayVariable;

                if ( arrayVble != null ) {
                    long address = arrayVble.Address;
                    var elementSize = arrayVble.Type.Size;
                    var endAddress = address + arrayVble.Size;

                    // Reverse each element
                    for(; address < endAddress; address += elementSize) {
                        this.Memory.SwitchEndianness( address, elementSize );
                    }
                } else {
                    this.Memory.SwitchEndianness( vble.Address, vble.Type.Size );
                }
            }

            return;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:CSim.Core.Machine"/> aligns vbles.
        /// </summary>
        /// <value><c>true</c> if aligns vbles; otherwise, <c>false</c>.</value>
        public bool AlignVbles {
            get {
                return this.Machine.AlignVbles;
            }
            set {
                this.Machine.AlignVbles = value;
            }
        }

		/// <summary>
		/// A convenience method to get the associated <see cref="MemoryManager"/>.
		/// <seealso cref="SymbolTable.Machine"/>
		/// </summary>
		/// <value>The <see cref="MemoryManager"/>.</value>
		public MemoryManager Memory {
			get {
				return this.Machine.Memory;
			}
		}

		/// <summary>
		/// A convenience method to get the associated <see cref="Machine"/>.
		/// <seealso cref="SymbolTable.Memory"/>
		/// </summary>
		/// <value>The <see cref="Machine"/>.</value>
		public Machine Machine {
			get; private set;
		}

		private Dictionary<string, Variable> tdsIds;
        private Dictionary<long, Variable> tdsAddresses;
		private List<long> addresses;

        private static int numMemBlock = 0;
    }
}

