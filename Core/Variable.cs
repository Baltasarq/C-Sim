
namespace CSim.Core {
	
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
		{
			this.Machine = m;
			this.Address = -1;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Variable"/> class.
		/// </summary>
		/// <param name="id">An <see cref="Id"/> for the variable.</param>
		/// <param name="t">A <see cref="Type"/> for the variable.</param>
		/// <param name="m">The <see cref="Machine"/> the variable will be in.</param>
		public Variable(Id id, Type t, Machine m)
			: this( m )
		{
			this.Name = id;
			this.type = t;
			this.Address = -1;
			this.Size = this.type.Size;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Variable"/> class.
		/// </summary>
		/// <param name="id">An <see cref="Id"/> for the variable.</param>
		/// <param name="t">A <see cref="Type"/> for the variable.</param>
		/// <param name="m">The <see cref="Machine"/> the variable will be in.</param>
		/// <param name="address">The address the variable will be sitting in.</param>
        public Variable(Id id, Type t, Machine m, long address)
            : this( id, t, m )
        {
            this.Address = address;
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
		/// Sets the type, internally.
		/// </summary>
		/// <param name="t">The new type, as a Type object.</param>
		protected void SetType(Type t)
		{
			this.type = t;
		}


		/// <summary>
		/// Gets or sets the address this variable exists in.
		/// </summary>
		/// <value>The address, as an int.</value>
        public long Address {
            get; set;
        }

		/// <summary>
		/// Gets the type of this variable.
		/// </summary>
		/// <value>The type.</value>
        public override Type Type {
            get {
				return this.type;
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
		/// Gets the type of the variable.
		/// It is aware of references.
		/// (accessing a reference is the same as accessing the target)
		/// </summary>
		/// <returns>
		/// The type of the associated type in references,
		/// the type itself of any other variable kind otherwise.
		/// </returns>
		public virtual Type GetTargetType()
		{
			return this.Type;
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
                this.Memory.Write( this.Address, value.GetRawValue() );
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
        /// Returns a <see cref="System.String"/> that represents the current <see cref="CSim.Core.Variable"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="CSim.Core.Variable"/>.</returns>
        public override string ToString()
        {
            return this.Type + " " + this.Name.Name;
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
		/// Gets the machine in which the variable exists.
		/// </summary>
		/// <value>The machine.</value>
		public Machine Machine {
			get; private set;
		}

		private Type type;
    }
}

