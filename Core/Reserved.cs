namespace CSim.Core {
	/// <summary>Reserved keywords.</summary>
	public static class Reserved {
		/// <summary>C/C++'s NULL.</summary>
		public const string PtrNull = "NULL";
		/// <summary>C++'s new null.</summary>
		public const string PtrNull2 = "nullptr";
		/// <summary>C++'s new operator</summary>
        public const string OpNew = "new";
		/// <summary>C++ delete operator</summary>
        public const string OpDelete = "delete";
		/// <summary>C/C++ access to pointer.</summary>
        public const string OpAccess = "*";
		/// <summary>C/C++ get address of.</summary>
        public const string OpAddressOf = "&";
		/// <summary>C/C++ assign operator.</summary>
        public const string OpAssign = "=";
		/// <summary>Label for "end of sentence"</summary>
        public const string LblEOL = ";";

		/// <summary>
		/// Determines whether the string is a null id or not.
		/// </summary>
		/// <returns><c>true</c>, if the given string is a null identifier, <c>false</c> otherwise.</returns>
		/// <param name="id">Identifier.</param>
		public static bool IsNullId(string id)
		{
			return ( id == Reserved.PtrNull
				  || id == Reserved.PtrNull2 );
		}
	}
}

