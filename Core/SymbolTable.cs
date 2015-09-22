using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using CSim.Core.Exceptions;
using CSim.Core.Types;
using CSim.Core.Variables;
using CSim.Core.Literals;

namespace CSim.Core
{
    public class SymbolTable {
        public const string MemBlockName = "_mblk#";

        public SymbolTable(Machine m)
        {
            this.tds = new Dictionary<string, Variable>();
			this.machine = m;
            this.addresses = new List<int>();
        }

		public void Reset()
		{
			this.tds.Clear();
			this.addresses.Clear();
		}

		public Variable Add(string id, CSim.Core.Type t)
		{
			return this.Add( new Id( id ), t );
		}

        public Variable Add(Id id, CSim.Core.Type t)
        {
            return this.Add( new Variable( id, t, this.Machine, -1 ) );
        }

		public Variable AddRef(string id, CSim.Core.Type t)
		{
			return this.AddRef( new Id( id ), t );
		}

        public Variable AddRef(Id id, CSim.Core.Type t)
        {
			return this.Add( new RefVariable( id, t, this.Machine, -1 ) );
        }

		public Variable AddPtr(string id, CSim.Core.Type t)
		{
			return this.AddPtr( new Id( id ), t );
		}

        public Variable AddPtr(Id id, CSim.Core.Type t)
        {
			return this.Add( new PtrVariable( id, t, this.Machine, -1 ) );
        }

        private Variable Add(Variable v)
		{
            // Chk
			if ( this.IsIdOfExistingVariable( v.Name ) ) {
				throw new AlreadyExistingVbleException( v.Name.Value );
			}

            // Store
            v.Address = this.Reserve( v );
            this.tds.Add( v.Name.Value, v );

            return v;
        }

		/// <summary>
		/// Remove the specified id from the variables list.
		/// </summary>
		/// <param name='id'>
		/// The identifier of the variable.
		/// </param>
		public void Remove(string id)
		{
			this.tds.Remove( id );
		}

		/// <summary>
		/// Cleans spurious references
		/// </summary>
		public void Collect()
		{
			this.RemoveDanglingReferences( );
		}

		/// <summary>
		/// Removes dangling references. i.e., references not set.
		/// References must point to some variable on initialization.
		/// It does not make sense to have an unset reference, while it
		/// is possible to have an unset pointer.
		/// After execution, this method should be called to remove
		/// all dangling references.
		/// </summary>
		private void RemoveDanglingReferences()
		{
			var refs = new List<RefVariable>();

			// Find
			foreach (Variable v in this.Variables) {
				RefVariable r = v as RefVariable;

				if ( r != null
				  && !( r.IsSet() ) )
				{
					refs.Add( r );
				}
			}

			// Remove
			foreach(RefVariable r in refs) {
				this.Remove( r.Name.Value );
			}

			return;
		}

        /// <summary>
        /// Reserve memory for the specified v.
        /// </summary>
        /// <param name='v'>
        /// The variable to reserve memory for.
        /// </param>
        public int Reserve(Variable v)
        {
            int tries = 100;
            Random randomEngine = new Random();
            int toret = -1;
            int[] addressesToFill = new int[ v.Type.Size ];

            while( toret < 0 ) {
                // Generate memory location
                do {
                    toret = randomEngine.Next( 0, this.Memory.Max );

                    if ( ( toret + v.Type.Size ) > this.Memory.Max ) {
                            toret = -1;
                    }
                } while( toret < 0 );

                // Create addresses to fill
                for (int i = 0; i < v.Type.Size; ++i) {
                    addressesToFill[ i ] = toret + i;
                }

                // Check against occupied addresses
                for (int i = 0; i < v.Type.Size; ++i) {
                    if ( this.addresses.Contains( addressesToFill[ i ] ) ) {
                        toret = -1;
                        break;
                    }
                }

                --tries;
                if ( tries < 0 ) {
                    break;
                }
            }

            // Re-check
            if ( toret < 0 ) {
                throw new ExhaustedMemoryException( 
                    L18n.Get( L18n.Id.ErrReserving ) + ": " + v.Name );
            }

            // Store the addresses to fill
            for (int i = 0; i < v.Type.Size; ++i) {
                this.addresses.Add( addressesToFill[ i ] );
            }

            return toret;
        }

		/// <summary>
		/// Gets the variables.
		/// </summary>
		/// <value>
		/// Returns pointers and variables
		/// </value>
        public ReadOnlyCollection<Variable> Variables {
            get {
                var toret = new Variable[ this.tds.Count ];

                this.tds.Values.CopyTo( toret, 0 );
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

            if ( id != null
              && id.Length > 0 )
            {
                // Look for vble
                this.tds.TryGetValue( id, out toret );
            }

			return toret;
        }

		/// <summary>
		/// Looks up a variable.
		/// </summary>
		/// <returns>
		/// The variable, if found. Core.Exceptions.UnknownVariableException otherwise.
		/// </returns>
		/// <param name='id'>
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
		public bool IsIdOfExistingVariable(Id id)
		{
			return ( this.LookForVariableOfId( id.Value ) != null );
		}

        /// <summary>
        /// Looks for address, given an int literal.
        /// </summary>
        /// <returns>The address to look for.</returns>
        /// <param name="intLit">The literal holding the integer.</param>
        public Variable LookForAddress(IntLiteral intLit)
        {
            return this.LookForAddress( intLit.Value );
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
		public Variable LookForAddress(int address)
		{
			Variable toret = null;

			foreach(Variable vble in this.Variables) {
				if ( vble.Address == address ) {
					toret = vble;
					break;
				}
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
			var toret = this.LookForAddress( ptrVble.LiteralValue );

			if ( toret == null
			  || toret.GetTargetType() != ptrVble.AssociatedType )
			{
				toret = new TempVariable( ptrVble.AssociatedType );
				toret.Address = ptrVble.LiteralValue.Value;
				toret.LiteralValue = new IntLiteral( this.machine, ptrVble.Access );
			}

			return toret;
		}

		/// <summary>
		/// Interprets the rvalue.
		/// </summary>
		/// <returns>
		/// Following the rvalue:
		/// 	- the variable hold by the rvalue
		/// 	- the value hold by the rvalue, as a TempVariable
		/// 	- the variable pointed by the pointer hold by the rvalue
		/// </returns>
		/// <param name='lvble'>
		/// The lvalue to be checked, must be a variable.
		/// It is checked to be of the same type of the rvalue, if not null.
		/// </param>
		/// <param name='value'>
		/// The value to interpret
		/// </param>
		public Variable LookForRValue(Variable lvble, RValue value)
		{
			Variable toret = null;
			var lit = value as Literal;

			if ( lit != null ) {
				// Plain value
				toret = new TempVariable( lit );
			}
			else
				if ( value is StrLiteral ) {
				string rightId = ((StrLiteral) value).Value;
				Variable rightVble = null;
				bool isRightPtrAccess = false;

				if ( rightId[ 0 ] == '*' ) {
					isRightPtrAccess = true;
					rightId = rightId.Substring( 1 );
				}

				rightVble = this.LookUp( rightId );
				var rightRefVble = rightVble as RefVariable;

				if ( isRightPtrAccess ) {
					var rightPtrVbleType = rightVble.Type as Ptr;
					if ( rightPtrVbleType != null ) {
						var rightPtrVble = rightVble as PtrVariable;

						if ( lvble != null ) {
							var lvalueType = lvble.GetTargetType();

							if ( rightPtrVbleType.AssociatedType != lvalueType ) {
								throw new TypeMismatchException( lvalueType.Name );
							}
						}

						if ( rightPtrVble == null ) {
							throw new TypeMismatchException(
								L18n.Get( L18n.Id.LblPointer ).ToLower()
								+ " (rvalue)" );
						}

						// Get the pointed variable, if possible, or access the memory
						toret = this.GetPointedValueAsVariable( rightPtrVble );
					} else {
						throw new TypeMismatchException(
							L18n.Get( L18n.Id.LblPointer ).ToLower()
							+ " (rvalue)" );
					}
				}
				else
					if ( rightRefVble != null ) {
					toret = rightRefVble.PointedVble;
				} else {
					// Plain variable
					toret = rightVble;
				}
			}

			return toret;
		}

		/// <summary>
		/// Deletes a memory block given a memory address.
		/// Looks for the pointed variable, which must be on heap.
		/// </summary>
		/// <param name="address">The address of the memory block, as an int.</param>
		/// <exception cref="IncorrectAddressException">when the memory pointed is not on heap.</exception>
		public void DeleteBlk(int address)
		{
			Variable pointedVble = this.Machine.TDS.LookForAddress( address );

			if ( pointedVble != null ) {
				if ( pointedVble.IsInHeap ) {
					this.Machine.TDS.Remove( pointedVble.Name.Value );
				} else {
					throw new IncorrectAddressException(
						L18n.Get( L18n.Id.ErrMemoryNotInHeap )
						+ ": " + Literal.ToPrettyNumber( address )
						);
				}
			}

			return;
		}

        public static string GetNextMemoryBlockName()
        {
            ++numMemBlock;
            return ( MemBlockName + numMemBlock.ToString() );
        }

		public MemoryManager Memory {
			get {
				return this.machine.Memory;
			}
		}

		public Machine Machine {
			get {
				return this.machine;
			}
		}

		private Dictionary<string, Variable> tds;
		private List<int> addresses;

        private Machine machine;
        private static int numMemBlock = 0;
    }
}

