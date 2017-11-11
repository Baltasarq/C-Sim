// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core {
    using System;
    
	/// <summary>Reserved keywords.</summary>
	public static class Reserved {
		/// <summary>C/C++'s NULL.</summary>
		public const string PtrNull = "NULL";
		/// <summary>C++'s new style null.</summary>
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
        /// <summary>Label for comments prefix</summary>
        public const string LblComment = "//";
        /// <summary>The prefix for labels for temp variables.</summary>
        public const string PrefixTempVariable = "_aux__";
        /// <summary>Prefix to use for memory blocks (heap)</summary>
        public const string PrefixMemBlockName = "_mblk_num_";
        
        /// <summary>The reserved words.</summary>
        public static readonly string[] ReservedWords = new []{
            PtrNull, PtrNull2, OpNew, OpDelete,
            "if", "while", "do", "for", "switch", "break", "case", "goto",
            "var", "foreach", "static", "int", "double", "bool", "char"
        };

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
        
        /// <summary>
        /// Checks whether this identifier is reserved.
        /// </summary>
        /// <returns><c>true</c>, if reserved, <c>false</c> otherwise.</returns>
        /// <param name="id">The identifier, as a string.</param>
        public static bool IsReserved(string id)
        {
            bool toret = IsNullId( id );
            
            if ( !toret ) {
                toret = id.StartsWith( PrefixTempVariable,
                                       StringComparison.InvariantCulture );
                                       
                if ( !toret ) {
                    toret = id.StartsWith( PrefixTempVariable,
                                       StringComparison.InvariantCulture );
                                       
                    if ( !toret ) {
                        foreach(string rw in ReservedWords) {
                            if ( id == rw ) {
                                toret = true;
                                break;
                            }
                        }
                    }
                }
            }
            
            return toret;
        }
	}
}
