namespace CSim.Core.Literals {
    using System.Text;
    using System.Numerics;

    using CSim.Core.Types;

    /// <summary>
    /// Literals of type Int.
    /// </summary>
    public class ArrayLiteral: Literal {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Literals.VectorLiteral"/> class.
		/// </summary>
		/// <param name="t">A <see cref="Primitive"/> <see cref="Variable"/>.</param>
		/// <param name="x">The sequence of values to store in the literal.</param>
		public ArrayLiteral(Primitive t, object[] x)
			:base( t.Machine, x )
		{
            this.ArrayType = new Ptr( 1, t );
		}

		/// <summary>
        /// Gets the type of the <see cref="ArrayLiteral"/>.
        /// The type is <see cref="CSim.Core.Types.Ptr"/>.
		/// </summary>
        /// <value>The <see cref="AType"/>.</value>
		public override AType Type {
			get {
				return this.ArrayType;
			}
		}

		/// <summary>
		/// The value stored, as an integer value.
		/// </summary>
		/// <value>The value.</value>
		public new object[] Value {
			get {
				return (object[]) base.Value;
			}
		}

		/// <summary>
		/// Gets the raw value of the literal, as sequence of bytes.
		/// </summary>
		/// <value>The raw value.</value>
		public override byte[] GetRawValue()
		{
			byte[] result = new byte[ this.Value.Length * this.ArrayType.AssociatedType.Size ];
			System.Buffer.BlockCopy( this.Value, 0, result, 0, result.Length );
			return result;
		}

		/// <summary>
		/// Returns a string appropriately formatted with the value of the literal.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="CSim.Core.Literals.IntLiteral"/>.</returns>
		public override string ToString()
		{
			var toret = new StringBuilder();
			object[] v = this.Value;
			string separator = "";

			for(int i = 0; i < v.Length; ++i) {
				toret.Append( separator );
				toret.Append( v[ i ].ToString() );
				separator = @", ";
			}

			return toret.ToString();
		}

        /// <summary>
        /// Gets the value as a <see cref="BigInteger"/>.
        /// </summary>
        /// <returns>Does not return any value.</returns>
        /// <exception cref="EngineException">Always.</exception>
        public override BigInteger GetValueAsInteger()
        {
            throw new EngineException( "cannot transform literal to int" );
        }

        /// <summary>
        /// Gets or sets the type of the vector.
        /// (Which is <see cref="Ptr"/>.)
        /// </summary>
        /// <value>The <see cref="AType"/> of the vector.</value>
        public Ptr ArrayType {
			get; set;
		}
	}
}
