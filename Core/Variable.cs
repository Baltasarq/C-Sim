
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
			this.Size = this.type.Size;
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
        /// Determines whether this variable is temporal or not.
        /// Temporal variables do not have a position in memory.
        /// </summary>
        /// <returns><c>true</c>, if is temp, <c>false</c> otherwise.</returns>
        public bool IsTemp()
        {
            return ( this.Address == -1 );
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
        /// Gets a value indicating whether
        /// this <see cref="T:CSim.Core.Variable"/> is an indrection,
        /// (i.e., pointer, reference...)
        /// </summary>
        /// <returns><c>true</c> if is a pointer; otherwise, <c>false</c>.</returns>
        public bool IsIndirection()
        {
            return this.Type is Types.Indirection;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is in heap.
        /// </summary>
        /// <value><c>true</c> if this instance is in heap; otherwise, <c>false</c>.</value>
		public bool IsInHeap {
			get { return this.Name.IsHeapId(); }
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
        /// Gets or sets the value associated to this variable,
        /// reading or writing in memory.
        /// </summary>
        /// <value>The value, as a literal.</value>
        public virtual Literal LiteralValue {
            get { return this.GetValue(); }
            set { this.SetValue( value ); }
        }
        
        /// <summary>
        /// Gets the value from memory.
        /// </summary>
        /// <returns>The value from memory, as a <see cref="Literal"/>.</returns>
        private Literal GetValueFromMemory()
        {
            return this.Memory.CreateLiteral( this.Address, this.Type );
        }
        
        /// <summary>
        /// Sets the value to memory.
        /// </summary>
        /// <param name="value">The value, as a <see cref="Literal"/>.</param>
        private void SetValueToMemory(Literal value)
        {
            var lit = this.type.CreateLiteral( value.Value );
            this.Memory.Write( this.Address, lit.GetRawValue() );
        }
        
        /// <summary>
        /// Creates a temporal variable for an array element.
        /// A temporal variable is simply not registered in the
        /// <see cref="SymbolTable"/>.
        /// </summary>
        /// <returns>The for array element.</returns>
        /// <param name="arrayId">Array identifier.</param>
        /// <param name="arrayAddress">Array address.</param>
        /// <param name="arrayType">Array type.</param>
        /// <param name="pos">Position.</param>
        public static Variable CreateTempVariableForArrayElement(
                    string arrayId,
                    BigInteger arrayAddress,
                    Types.Ptr arrayType,
                    int pos)
        {
            AType elementType = arrayType.DerreferencedType;
            Variable toret = CreateTempVariable( elementType );
            
            toret.Address = arrayAddress + ( elementType.Size * pos );
            
            toret.Name.Name += "_" + arrayId + "_" + pos;
            return toret;
        }
        
        /// <summary>
        /// Creates a temporal variable with this type.
        /// A temporal variable is simply not registered in the
        /// <see cref="SymbolTable"/>.
        /// </summary>
        /// <param name="type">A <see cref="AType"/> to create the variable for.</param> 
        /// <returns>A <see cref="Variable"/> with the given <see cref="AType"/>.</returns>
        public static Variable CreateTempVariable(AType type)
        {
            ++NumAuxVariables;
            
            return type.CreateVariable(
                            Reserved.PrefixTempVariable + NumAuxVariables
            );
        }
        
        /// <summary>
        /// Creates a temporal variable with this type.
        /// A temporal variable is simply not registered in the
        /// <see cref="SymbolTable"/>.
        /// </summary>
        /// <param name="lit">A <see cref="Literal"/> to create the variable for.</param> 
        /// <returns>A <see cref="Variable"/> with the given <see cref="Literal"/>.</returns>
        public static Variable CreateTempVariable(Literal lit)
        {
            ++NumAuxVariables;
            
            var toret = CreateTempVariable( lit.Type );
            toret.LiteralValue = lit;
            
            return toret;
        }
        
        /// <summary>
        /// Creates a temporal variable with <see cref="Types.Primitives.Int"/>.
        /// A temporal variable is simply not registered in the
        /// <see cref="SymbolTable"/>.
        /// </summary>
        /// <param name="m">The <see cref="Machine"/> to create the variable for.</param> 
        /// <param name="x">An integer to create the variable for.</param> 
        /// <returns>A <see cref="Variable"/> with the given integer.</returns>
        public static Variable CreateTempVariable(Machine m, BigInteger x)
        {
            ++NumAuxVariables;
            
            var litValue = new Literals.IntLiteral( m, x );
            var toret = CreateTempVariable( litValue.Type );
            toret.LiteralValue = litValue;
            
            return toret;
        }
        
        /// <summary>
        /// Creates a temporal variable with <see cref="Types.Primitives.Double"/>.
        /// A temporal variable is simply not registered in the
        /// <see cref="SymbolTable"/>.
        /// </summary>
        /// <param name="m">The <see cref="Machine"/> to create the variable for.</param> 
        /// <param name="x">A double to create the variable for.</param> 
        /// <returns>A <see cref="Variable"/> with the given double.</returns>
        public static Variable CreateTempVariable(Machine m, double x)
        {
            ++NumAuxVariables;
            
            var litValue = new Literals.DoubleLiteral( m, x );
            var toret = CreateTempVariable( litValue.Type );
            toret.LiteralValue = litValue;
            
            return toret;
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
        /// Gets or sets the name of the variable.
        /// </summary>
        /// <value>The name, as a string.</value>
        public Id Name {
            get; set;
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
        
        /// <summary>
        /// Gets or sets the address this variable exists in.
        /// </summary>
        /// <value>The address, as an int.</value>
        public BigInteger Address {
            get {
                return address;
            }
            set {
                if ( value < 0 ) {
                    this.address = -1;
                    this.GetValue = () => Local;
                    this.SetValue = (literal) => Local = literal;
                } else {
                    this.address = value;
                    this.GetValue = this.GetValueFromMemory;
                    this.SetValue = this.SetValueToMemory;
                }
                
                return;
            }
        }
        
        // These are references to the actual functions used.
        private System.Func<Literal> GetValue;
        private System.Action<Literal> SetValue;

        /// <summary>
        /// Gets or sets the local value of the variable.
        /// Used when there is no address for it.
        /// </summary>
        /// <value>The local, as <see cref="Literal"/> .</value>
        private Literal Local {
            get; set;
        }
        
        /// <summary>The type of the variable.</summary> 
		private AType type;
        
        /// <summary>
        /// The address for this variable. -1 when not registered in the symbol
        /// table, which means it is a temporal variable, and the local storage
        /// is employed.
        /// </summary>
        private BigInteger address;
        
        /// <summary>The number of aux variables.</summary>
        private static int NumAuxVariables = 0;
    }
}

