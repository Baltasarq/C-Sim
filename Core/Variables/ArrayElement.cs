namespace CSim.Core.Variables {
	using CSim.Core;
	using CSim.Core.Types;

	/// <summary>
	/// This is used when an array element must be used as a result.
	/// </summary>
	public class ArrayElement: Variable {
		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Variables.ArrayElement"/> class.
		/// </summary>
		/// <param name="array">The array variable.</param>
		/// <param name="ptrType">The type of the array.</param>
		/// <param name="pos">The postion of the element</param>
		/// <param name="machine">The machine this variable will belong to.</param>
		public ArrayElement(Variable array, Ptr ptrType, long pos, Machine machine)
			:base( new Id( array.Name.Name + "_#" + pos ), ptrType.AssociatedType, machine)
		{
			this.Variable = array;
			this.Offset = pos * this.Type.Size;
			this.Address = this.Variable.Address + this.Offset;
		}

		/// <summary>
		/// Gets the array element as a regular variable.
		/// </summary>
		/// <value>The element, as a variable.</value>
		public Variable Variable {
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
