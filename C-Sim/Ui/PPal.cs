// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Ui {
    using System;
    using System.Windows.Forms;
    
    using Core;
    
    /// <summary>
    /// The entry point of the application
    /// </summary>
    public class PPal {
        /// <summary>Start for all option args.</summary>
        public const string ArgPrefix = "--";
        /// <summary>Argument for showing help.</summary>
        public const string ArgHelp = "help";
        /// <summary>Arg for offering a gui.</summary>
        public const string ArgGui = "gui";
        /// <summary>Arg for not showing a gui.</summary>
        public const string ArgNoGui = "no-gui";
        /// <summary>Arg for forcing a random reset.</summary>
        public const string ArgRndReset = "rnd-reset";
        /// <summary>Arg for forcing a zeroing reset.</summary>
        public const string ArgZeroReset = "zero-reset";
        /// <summary>Contents for help.</summary>
        public const string Help = "c-sim [options] <CSim-file>"
                + "\n\t" + ArgPrefix + ArgHelp + "\t\tThis help."
                + "\n\t" + ArgPrefix + ArgGui + "\t\tStart with gui (default)"
                + "\n\t" + ArgPrefix + ArgNoGui + "\tStart without gui"
                + "\n\t" + ArgPrefix + ArgRndReset + "\tReset random reset"
                + "\n\t" + ArgPrefix + ArgZeroReset + "\tReset zero reset"
                + "\n\n";
    
        private class Cfg {
            /// <summary>Initializes a new <see cref="T:Cfg"/>.</summary>
            public Cfg()
            {
                this.UseGui = true;
            }
        
            /// <summary>Gets or sets the machine's reset being random.</summary>
            /// <value><c>true</c> for random reset, <c>false</c> otherwise.</value>
            public bool RandomReset {
                get; set;
            }
            
            /// <summary>Gets or sets the app using a gui.</summary>
            /// <value><c>true</c> for a gui, <c>false</c> otherwise.</value>
            public bool UseGui {
                get; set;
            }
            
            /// <summary>Gets or sets whether to show help.</summary>
            /// <value><c>true</c> for help, <c>false</c> otherwise.</value>
            public bool ShowHelp {
                get {
                    return this.showHelp;
                }
                set {
                    this.showHelp = value;
                    this.UseGui &= !value;
                }
            }
            
            /// <summary>Gets or sets the source file to load.</summary>
            /// <value>The file to load, as a string.</value>
            public string File {
                get; set;
            }
            
            /// <summary>Determines whether the config involves a file.</summary>
            /// <returns><c>true</c>, if a file was set, <c>false</c> otherwise.</returns>
            public bool HasFile()
            {
                return !string.IsNullOrEmpty( this.File );
            }
            
            private bool showHelp;
        }
        
        private static Cfg ProcessArgs(string[] args)
        {
            var InvCulture = StringComparison.InvariantCulture;
            var cfg = new Cfg();
            
            for(int i = 0; i < args.Length; ++i) {
                string arg = args[ i ].Trim();
            
                if ( i == ( args.Length - 1 )
                  && !arg.StartsWith( ArgPrefix, InvCulture ) )
                {
                   cfg.File = arg;     
                }
                else
                if ( arg.StartsWith( ArgPrefix, InvCulture ) ) {
                    arg = arg.Remove( 0, 2 );
                    
                    if ( arg == ArgHelp ) {
                        cfg.ShowHelp = true;
                        break;
                    }
                    else
                    if ( arg == ArgGui ) {
                        cfg.UseGui = true;
                    }
                    else
                    if ( arg == ArgNoGui ) {
                        cfg.UseGui = false;
                    }
                    else
                    if ( arg == ArgRndReset ) {
                        cfg.RandomReset = true;
                    }
                    else
                    if ( arg == ArgZeroReset ) {
                        cfg.RandomReset = false;
                    }
                }
            }
            
            return cfg;
        }
    
		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            Cfg cfg = ProcessArgs( args );
            
            if ( cfg.ShowHelp ) {
                Console.WriteLine( AppInfo.Header );
                Console.WriteLine( Help );
            }
            else {
                var m = new Machine();
                
                if ( cfg.RandomReset ) {
                    m.Reset( MemoryManager.ResetType.Random );
                }
                
	            if ( cfg.UseGui ) {
                    var mainWindow = new MainWindow( m );
                    
                    mainWindow.Show();
                    if ( cfg.HasFile() ) {
                        mainWindow.LoadProgram( cfg.File );
                    }
                    
	                Application.Run( mainWindow );
	            } else {
                    var rel = new REL( m );
                    
                    if ( cfg.HasFile() ) {
                        rel.Load( new []{ "load", cfg.File } );
                    } else {
                        rel.Run();
                    }
	            }
            }
            
            return;
        }
    }
}
