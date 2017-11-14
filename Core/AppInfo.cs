// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core {
	/// <summary>App versioning info.</summary>
    public static class AppInfo
    {
		/// <summary>Name for the app.</summary>
		public const string Name = "C-Sim";
		/// <summary>Author of the app.</summary>
        public const string Author = "jbgarcia@uvigo.es";
		/// <summary>Version of the app.</summary>
        public const string Version = "1.7.2 20171106";
		/// <summary>Website for this app.</summary>
		public const string Web = "http://webs.uvigo.es/jbgarcia/prys/csim/";
		/// <summary>Extension for files created by this app.</summary>
		public const string FileExt = "csim";
        ///<summary>The header to show at the top of output.</summary>
        public static readonly string Header = Name + " v" + Version + '\n';
    }
}
