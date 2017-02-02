namespace CSim.Core.Variables {
	using CSim.Core;
	using CSim.Core.Types;

	/// <summary>
	/// This is used when an array element must be used as a result.
	/// </summary>
	public class ArrayElement: Variable {
		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Variables.ArrayElement"/> class.
		/// This one will not be tied to a variable.
		/// </summary>
		/// <param name="id">The identifier, as a string.</param>
		/// <param name="address">The address of the array.</param>
		/// <param name="ptrType">Ptr type.</param>
		/// <param name="pos">Position.</param>
		/// <param name="machine">Machine.</param>
		public ArrayElement(string id, long address, Ptr ptrType, long pos, Machine machine)
			:base( new Id( "_" + id + "_" + pos ), ptrType.DerreferencedType, machine)
		{
			this.Offset = pos * this.Type.Size;
			this.Address = address + this.Offset;
			this.Pos = pos;
		}

		/// <summary>
		/// Gets the position in the array.
		/// </summary>
		/// <value>The position.</value>
		public long Pos {
			get; private set;
		}

		/// <summary>
		/// Gets the offset from the beginning of the array.
		/// </summary>
		/// <value>The offset, as a long.</value>
		public long Offset {
			get; private set;
		}
	}
}
