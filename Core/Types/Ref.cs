namespace CSim.Core.Types {
    using CSim.Core.Literals;
	using CSim.Core.Variables;

    /// <summary>
    /// Represents references. References are a kind of pointer,
    /// that cannot be changed to point to another variable, and does not
	/// need special operators, such as * and &amp;.
    /// </summary>
	public class Ref: AType {
		/// <summary>The reference part for the name of the type.</summary>
		public const string RefTypeNamePart = "&";

		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Types.Ref"/> class.
		/// </summary>
		/// <param name="associatedType">Associated <see cref="AType"/>.</param>
        internal Ref(AType associatedType)
			: base( associatedType.Machine,
                    PrepareRefTypeName( associatedType ),
                    associatedType.Machine.WordSize )
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
        /// Creates a literal for this type, given a byte sequence.
        /// </summary>
        /// <returns>The literal, as an appropriate object of a class inheriting from Literal.</returns>
        /// <param name="raw">The sequence of bytes containing the value in memory.</param>
        public override Literal CreateLiteral(byte[] raw)
        {
            return new IntLiteral( this.Machine, this.Machine.Bytes.FromBytesToInt( raw ) );
        }
        
        /// <summary>
        /// Creates a literal for this type, given a value.
        /// </summary>
        /// <returns>The literal, as an appropriate object of a class inheriting from Literal.</returns>
        /// <param name="v">The given value.</param>
        public override Literal CreateLiteral(object v)
        {
            return new IntLiteral( this.Machine, v );
        }

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Types.Ref"/>.
		/// </summary>
		/// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Types.Ref"/>.</returns>
		public override string ToString()
		{
			return this.Name;
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
        
        /// <summary>
        /// Gets the associated basic type.
        /// Note that, for int &amp; ref, returns int.
        /// </summary>
        /// <value>A <see cref="AType"/>.</value>
        public AType AssociatedType {
            get; private set;
        }
	}
}
