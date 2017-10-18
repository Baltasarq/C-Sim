namespace CSim.Core.Types {
	using CSim.Core.Variables;

    /// <summary>
    /// Represents references. References are a kind of pointer,
    /// that cannot be changed to point to another variable, and does not
	/// need special operators, such as * and &amp;.
    /// </summary>
	public class Ref: Indirection {
		/// <summary>The reference part for the name of the type.</summary>
		public const string RefTypeNamePart = "&";

		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Types.Ref"/> class.
		/// </summary>
		/// <param name="associatedType">Associated <see cref="AType"/>.</param>
        internal Ref(AType associatedType)
			: base( associatedType.Machine, PrepareRefTypeName( associatedType ) )
        {
            this.AssociatedType = associatedType;
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
        /// Prepares the type name for this reference.
        /// A reference to a reference is a reference.
        /// </summary>
        /// <returns>The name, as a string like int&amp;.</returns>
        /// <param name="t">Any <see cref="AType"/>.</param>
        private static string PrepareRefTypeName(AType t)
        {
            string toret = t.Name;
            
            if ( !( t is Ref ) ) {
                toret += RefTypeNamePart;
            }
            
            return toret;
        }        
	}
}
