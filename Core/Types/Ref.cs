namespace CSim.Core.Types {
	using CSim.Core.Variables;

    /// <summary>
    /// Represents references. References are a kind of pointer,
    /// that cannot be changed to point to another variable, and does not
	/// need special operators, such as * and &amp;.
    /// </summary>
	public class Ref: Ptr {
		/// <summary>The reference part for the name of the type.</summary>
		public const string RefTypeNamePart = "&";

		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Types.Ref"/> class.
		/// </summary>
		/// <param name="associatedType">Associated <see cref="AType"/>.</param>
        internal Ref(AType associatedType)
			: base( associatedType.Name + RefTypeNamePart, associatedType )
        {
        }

		/// <summary>
		/// Creates a corresponding variable.
		/// </summary>
		/// <returns>A <see cref="Variable"/> with this <see cref="AType"/>.</returns>
		/// <param name="strId">The identifier for the variable.</param>
		public override Variable CreateVariable(string strId)
		{
			return new RefVariable( new Id( this.Machine, strId ), this );
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Types.Ref"/>.
		/// </summary>
		/// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Types.Ref"/>.</returns>
		public override string ToString()
		{
			return this.Name;
		}
	}
}

