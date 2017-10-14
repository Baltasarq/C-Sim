
namespace CSim.Core {
    using System.Numerics;
    
	/// <summary>
	/// A variable living in the <see cref="Machine"/>.
	/// </summary>
    public class Variable: RValue {
		/// <summary>
		/// Auxiliar constructor
		/// Initializes a new instance of the <see cref="CSim.Core.Variable"/> class.
		/// </summary>
		/// <param name="m">The machine the variable will live in.</param>
		protected Variable(Machine m)
            :base( m )
		{
			this.Address = -1;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Variable"/> class.
		/// </summary>
		/// <param name="id">An <see cref="Id"/> for the variable.</param>
		/// <param name="t">A <see cref="AType"/> for the variable.</param>
		public Variable(Id id, AType t)
			: this( id.Machine )
		{
			this.Name = id;
			this.type = t;
			this.Address = -1;
			this.Size = this.type.Size;
		}

		/// <summary>
		/// Gets or sets the name of the variable.
		/// </summary>
		/// <value>The name, as a string.</value>
        public Id Name {
			get; set;
        }

		/// <summary>
		/// Sets the name without checking.
		/// This is needed internally, not for users.
		/// </summary>
		/// <param name="n">The new name, as a string.</param>
		protected void SetNameWithoutChecking(string n)
		{
            this.Name.SetIdWithoutChecks( n );
		}

        /// <summary>
        /// Determines whether this variable is stored in that address
        /// </summary>
        /// <returns><c>true</c>, if it is partly stored here, <c>false</c> otherwise.</returns>
        /// <param name="address">A position in memory.</param>
        public bool IsStoredIn(BigInteger address)
        {
            return ( address >= this.Address
                  && address < this.Address + this.Size );
        }

        /// <summary>
        /// Solves the type to a variable with the <see cref="Literals.TypeLiteral"/> as value.
        /// </summary>
        /// <returns>A suitable <see cref="Variable"/>.</returns>
        public override Variable SolveToVariable()
        {
            return this;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="CSim.Core.Variable"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="CSim.Core.Variable"/>.</returns>
        public override string ToString()
        {
            return this.Type + " " + this.Name.Name;
        }        

		/// <summary>
		/// Gets or sets the address this variable exists in.
		/// </summary>
		/// <value>The address, as an int.</value>
        public BigInteger Address {
            get; set;
        }

        /// <summary>
        /// Gets a value indicating whether
        /// this <see cref="T:CSim.Core.Variable"/> is a pointer.
        /// </summary>
        /// <value><c>true</c> if is a pointer; otherwise, <c>false</c>.</value>
        public bool IsPtr {
            get {
                return this.Type is Types.Ptr;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is in heap.
        /// </summary>
        /// <value><c>true</c> if this instance is in heap; otherwise, <c>false</c>.</value>
		public bool IsInHeap {
			get { return this.Name.IsHeapId(); }
		}

        /// <summary>
        /// Gets or sets the value associated to this variable,
        /// reading or writing in memory.
        /// </summary>
        /// <value>The value, as a literal.</value>
        public virtual Literal LiteralValue {
            get {
				return this.Memory.CreateLiteral( this.Address, this.Type );
            }
            set {
				var lit = this.type.CreateLiteral( value.Value );
				this.Memory.Write( this.Address, lit.GetRawValue() );
            }
        }

		/// <summary>
		/// Gets the value of the variable.
		/// </summary>
		/// <value>The value, as a <see cref="System.Object"/>.</value>
		public override object Value {
			get {
				return this.LiteralValue;
			}
		}
        
        /// <summary>
        /// Gets the memory in which this variable is stored.
        /// </summary>
        /// <value>The memory.</value>
        public MemoryManager Memory {
            get {
				return this.Machine.Memory;
			}
        }
        
        /// <summary>
        /// Gets the size of the variable.
        /// This size is normally totally dependent on the type.
        /// </summary>
        /// <value>The size, as an int.</value>
        public int Size {
            get; protected set;
        }
        
        /// <summary>
        /// Gets the <see cref="AType"/> of this <see cref="Variable"/>.
        /// </summary>
        /// <value>The corresponding <see cref="AType"/>.</value>
        public override AType Type {
            get {
                return this.type;
            }
        }

		private AType type;
    }
}

