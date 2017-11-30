// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core {
	using System;
    using System.Numerics;
	using System.Collections.ObjectModel;
	using System.Collections.Generic;
    using System.Linq;

	using Exceptions;
	using Variables;
    using Literals;
	using Types;

	/// <summary>All the variables in the <see cref="Machine"/> reside here.</summary>
    public class SymbolTable {
		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.SymbolTable"/> class,
		/// for a given <see cref="Machine"/>.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/> this symbol table will be used in.</param>
        public SymbolTable(Machine m)
        {
            this.Machine = m;
            this.tdsIds = new Dictionary<string, Variable>();
            this.tdsAddresses = new Dictionary<BigInteger, List<Variable>>();
            this.addresses = new List<BigInteger>();
            
            this.CreateSysVariables();
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
            
            this.CreateSysVariables();
		}
        
        /// <summary>Creates system variables.</summary>
        private void CreateSysVariables()
        {
            var int_t = this.Machine.TypeSystem.GetIntType();
            
            var vblStdIn = new Variable( this.Machine, "stdin", int_t ) {
                LiteralValue = new IntLiteral( this.Machine, 0 )
            };
            var vblStdOut = new Variable( this.Machine, "stdout", int_t ) {
                LiteralValue = new IntLiteral( this.Machine, 1 )
            };
            var vblStdErr = new Variable( this.Machine, "stderr", int_t ) {
                LiteralValue = new IntLiteral( this.Machine, 2 )
            };
            
            this.tdsIds.Add( vblStdIn.Name.Text, vblStdIn );
            this.tdsIds.Add( vblStdOut.Name.Text, vblStdOut );
            this.tdsIds.Add( vblStdErr.Name.Text, vblStdErr );
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
        private BigInteger[] CreateAddressesToFill(BigInteger address, int size)
        {
            BigInteger[] toret = new BigInteger[ size ];

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
            BigInteger[] addressesToOccupy = this.CreateAddressesToFill( v.Address, v.Size );

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
            List<Variable> vbleList;
            
            // Register by id
            this.tdsIds.Add( v.Name.Text, v );
                        
            // Register by variable address
            if ( !this.tdsAddresses.TryGetValue( v.Address, out vbleList ) )
            {
                vbleList = new List<Variable>();
                this.tdsAddresses.Add( v.Address, vbleList );
            }

            vbleList.Add( v );
            
            // Register memory addresses
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
				throw new AlreadyExistingVbleException( v.Name.Text );
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
            if ( this.tdsIds.TryGetValue( id, out Variable vbleToRemove ) ) {
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
			foreach (Variable v in this.Variables) {
				if ( v is RefVariable refVble
				  && !( refVble.IsSet() ) )
				{
                    this.Remove( v.Name.Text );
				}
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
        private bool IsLocationFree(BigInteger address, int size)
        {
            BigInteger[] addressesToOccupy = this.CreateAddressesToFill( address, size );
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
        public BigInteger Reserve(Variable v)
        {
            BigInteger toret;

            if ( this.AlignVbles ) {
                toret = this.AlignedReserve( v );
            } else {
                toret = this.AleaReserve( v );
            }

            return toret;
        }

        private BigInteger AlignedReserve(Variable v)
        {
            int toret = -1;
            int address = this.Machine.WordSize;

            while( address < this.Memory.Max ) {
                IEnumerable<Variable> lVbles =
                                    this.LookForAllVblesInAddress( address );
                
                if ( !lVbles.Any()
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
                    L10n.Get( L10n.Id.ErrReserving ) + ": " + v.Name );
            }

            return toret;
        }

        private BigInteger AleaReserve(Variable v)
        {
            int tries = 100;
            var randomEngine = new Random();
            int toret = -1;

            // Generate memory location
            do {
                int max = (int) ( ( (long) this.Memory.Max ) - this.Machine.WordSize );
				toret = randomEngine.Next( max );

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
                    L10n.Get( L10n.Id.ErrReserving ) + ": " + v.Name );
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
		/// <param name="idVble">The identifier in question, as an Id.</param>
		public bool IsIdOfExistingVariable(Id idVble)
		{
            return this.IsIdOfExistingVariable( idVble.Text );
        }
        
        /// <summary>
        /// Determines whether there is a variable with an identifier like the specified one.
        /// </summary>
        /// <returns><c>true</c> if there is a variable with that identifier; otherwise, <c>false</c>.</returns>
        /// <param name="id">The identifier in question, as a string.</param>
        public bool IsIdOfExistingVariable(string id)
        {
			return ( this.LookForVariableOfId( id ) != null );
		}

		/// <summary>
		/// Looks for all variables in a given address.
		/// </summary>
		/// <returns>
		/// The variables, as a <see cref="IEnumerable{Variable}"/>.
		/// </returns>
		/// <param name='address'>
		/// The address to look for.
		/// </param>
		public IEnumerable<Variable> LookForAllVblesInAddress(BigInteger address)
		{
            List<Variable> toret;

            if ( !this.tdsAddresses.TryGetValue( address, out toret ) )
            {
                toret = new List<Variable>();
            }

			return toret;
		}
        
        /// <summary>Returns the variable in that address</summary>
        /// <param name='t'>
        /// An optional type to match the variable.
        /// </param>
        /// <returns>
        /// The variable, as a Variable object.
        /// </returns>
        /// <param name='address'>
        /// The address to look for.
        /// </param>
        public Variable LookForAddress(BigInteger address, AType t = null)
        {
            Variable toret = null;
            List<Variable> vbleList = null;
            
            // Fix optional type
            if ( t == null ) {
                t = Any.Get( this.Machine );
            }
            
            // Examine all variables
            if ( this.tdsAddresses.TryGetValue( address, out vbleList ) )
            {
                if ( vbleList.Count > 1 ) {
                    foreach(Variable vble in vbleList) {
                        if ( t is Any
                          || vble.Type == t )
                        {
                            toret = vble;
                            break;
                        }
                    }
                 } else {
                    toret = vbleList[ 0 ];
                 }
            }

            return toret;
        }

		/// <summary>
		/// Deletes a memory block given a memory address.
		/// Looks for the pointed variable, which must be on heap.
		/// </summary>
		/// <param name="address">The address of the memory block, as a <see cref="BigInteger"/> .</param>
		/// <exception cref="IncorrectAddressException">when the memory pointed is not on heap.</exception>
		public void DeleteBlk(BigInteger address)
		{
            bool removed = false;
			IEnumerable<Variable> pointedVbles =
                                    this.LookForAllVblesInAddress( address );

            // Look for variable in heap
            foreach (Variable pointedVble in pointedVbles) {
    			if ( pointedVble != null ) {
    				if ( pointedVble.IsInHeap ) {
                        removed = true;
    					this.Remove( pointedVble.Name.Text );
                        break;
    				}
    			}
            }
            
            // Vble found?
            if ( !removed ) {
                throw new IncorrectAddressException(
                    L10n.Get( L10n.Id.ErrMemoryNotInHeap )
                    + ": " + new IntLiteral( this.Machine, address ).ToPrettyNumber()
                    );
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
            return ( Reserved.PrefixMemBlockName + numMemBlock );
        }

        /// <summary>
        /// Switchs the endianess of all variables in this machine.
        /// The machine's endianness (<see cref="Machine.Endian"/>
        /// is assumed to have already changed.
        /// </summary>
        public void SwitchEndianness()
        {
            foreach (Variable vble in this.Variables) {
                if ( vble is ArrayVariable arrayVble ) {
                    BigInteger address = arrayVble.Address;
					var elementSize = ( (Ptr) arrayVble.Type ).DerreferencedType.Size;
                    var endAddress = address + arrayVble.Size;

                    // Reverse each element
                    for(; address < endAddress; address += elementSize) {
                        this.Memory.SwitchEndianness( address, elementSize );
                    }
                } else {
                    this.Memory.SwitchEndianness( vble.Address, vble.Size );
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
        private Dictionary<BigInteger, List<Variable>> tdsAddresses;
		private List<BigInteger> addresses;

        private static int numMemBlock;
    }
}

