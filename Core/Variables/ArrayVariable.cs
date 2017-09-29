using CSim.Core.Types;

namespace CSim.Core.Variables {
    using System.Numerics;
    using CSim.Core.Literals;
    
	/// <summary>
	/// An array variable.
	/// </summary>
	public class ArrayVariable: Variable {

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Variables.ArrayVariable"/> class.
		/// </summary>
		/// <param name="id">The identifier for the array.</param>
        /// <param name="t">The <see cref="AType"/> for elements (associated type).</param>
		/// <param name="count">The number of elements.</param>
		public ArrayVariable(Id id, AType t, BigInteger count)
			: base( id, t.Machine.TypeSystem.GetPtrType( t ) )
		{
			this.Count = count;
			this.Size = t.Size * (int) count;
		}

        /// <summary>
        /// Gets the associated type.
        /// This is a shortcut to the type stored in the array.
        /// Note that it returns "int**" for an int**[].
        /// </summary>
        /// <value>The associated type, as a <see cref="AType"/> instance.</value>
        public AType ElementsType {
            get {
                return ( (Ptr) this.Type ).DerreferencedType;
            }
        }
        
        /// <summary>
        /// Extracts the array elements' bytes.
        /// </summary>
        /// <returns>A primitive byte array.
        ///          The first dimension is the number of elements of the array.
        ///          The second dimension is the size of the data type.
        /// </returns>
        public byte[][] ExtractArrayElementsRaw()
        {
            return this.Machine.Memory.ExtractArrayElementValues(
                                                this.ElementsType,
                                                this.Address,
                                                this.Count );
        }

		/// <summary>
		/// Extracts the values of an array from memory.
		/// </summary>
		public Literal[] ExtractArrayElementsValues()
		{
			var toret = new Literal[ (int) this.Count ];
			byte[][] rawValues = this.ExtractArrayElementsRaw();
			
			for (int i = 0; i < rawValues.Length; ++i) {
				toret[ i ] = this.Machine.TypeSystem.CreateLiteral(
                                            this.ElementsType, rawValues[ i ] );
			}

			return toret;
		}

        /// <summary>
        /// Gets or sets the values associated to this pointer,
        /// reading or writing in memory.
        /// A pointer's value is actually always an integer, same size as
        /// memory width.
        /// </summary>
        /// <value>The value, as an <see cref="IntLiteral"/>.</value>
        public new ArrayLiteral LiteralValue {
            get {
                return (ArrayLiteral)
						this.Memory.CreateLiteral( this.Address, this.Type );
            }
            set {
                this.Memory.Write( this.Address, value.GetRawValue() );
            }
        }

		/// <summary>
		/// Gets the size of the vector, in elements, not bytes.
		/// This size is totally dependent on the type.
		/// </summary>
		/// <value>The size, as an int.</value>
		public BigInteger Count {
			get; set;
		}
	}
}
