namespace CSim.Core.Literals {
	using System;
	using System.Text;

	using CSim.Core.Types;

	/// <summary>
	/// Literals of type Int.
	/// </summary>
	public class VectorLiteral: Literal {
		public VectorLiteral(Machine m, Primitive t, object[] x)
			:base( m, x )
		{
			this.VectorType = new Ptr( 1, t, this.Machine.WordSize );
		}

		/// <summary>
		/// Gets the type of the vector literal.
		/// The type is CSim.Core.Types.Ptr
		/// </summary>
		/// <value>The type.</value>
		public override CSim.Core.Type Type {
			get {
				return this.VectorType;
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
			byte[] result = new byte[ this.Value.Length * this.VectorType.AssociatedType.Size ];
			Buffer.BlockCopy( this.Value, 0, result, 0, result.Length );
			return result;
		}

		/// <summary>
		/// Returns a string appropriately formatted with the value of the literal.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="CSim.Core.Literals.IntLiteral"/>.</returns>
		public override string ToString()
		{
			StringBuilder toret = new StringBuilder();
			object[] v = this.Value;
			string separator = "";

			for(int i = 0; i < v.Length; ++i) {
				toret.Append( separator );
				toret.Append( v[ i ].ToString() );
				separator = @", ";
			}

			return toret.ToString();
		}

		public CSim.Core.Types.Ptr VectorType {
			get; set;
		}
	}
}
