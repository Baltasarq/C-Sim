using System;

namespace CSim.Core
{
	public static class Reserved
	{
		public const string PtrNull = "NULL";
		public const string PtrNull2 = "nullptr";

        public const string OpNew = "new";
        public const string OpDelete = "delete";
        public const string OpAccess = "*";
        public const string OpAddressOf = "&";
        public const string OpAssign = "=";

        public const string LblEOL = ";";

		public static bool IsNullId(string id)
		{
			return ( id == Reserved.PtrNull
				  || id == Reserved.PtrNull2 );
		}
	}
}

