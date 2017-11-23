// CSim - (c) 2017 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Ui {
    using System;
    using System.IO;
    using System.Text;
    using System.Numerics;
    using System.Globalization;
    using System.Collections.Generic;
    
    using Core;

    /// <summary>
    /// Creates a Read - Eval - Loop for using the <see cref="Machine"/>.
    /// </summary>
    public class REL {
        /// <summary>The max number of bytes to dump.</summary>
        public const int DumpMaxBytes = 16;
        /// <summary>The prompt to use in the REL.</summary>
        public const string Prompt = "> ";
        /// <summary>Prefix for hex numbers.</summary>
        public const string HexPrefix = "0x";
        /// <summary>The prefix for all REL commands.</summary>
        public const string CmdPrefix = ".";
        /// <summary>The command to exit the REL.</summary>
        public const string CmdEnd = "exit";
        /// <summary>The command to show existing variables.</summary>
        public const string CmdDir = "dir";
        /// <summary>The command to show available commands.</summary>
        public const string CmdHelp = "help";
        /// <summary>The command to reset the machine.</summary>
        public const string CmdReset = "reset";
        /// <summary>The command to dump memory.</summary>
        public const string CmdDump = "dump";
        /// <summary>The command to load a program.</summary>
        public const string CmdLoad = "load";
        /// <summary>The argument to denote random.</summary>
        public const string ArgRandom = "rnd";
        /// <summary>Show the available commands.</summary>
        public const string ShowAssistance = "Type '"
                + CmdPrefix + CmdHelp + "' to show assistance.";
        /// <summary>Show the available commands.</summary>
        public const string Help = "Available commands:"
                + "\n\t" + CmdPrefix + CmdHelp + "\t\tThis help."
                + "\n\t" + CmdPrefix + CmdDir + "\t\tShow existing variables."
                + "\n\t" + CmdPrefix + CmdLoad + " /path/f\tLoads program f."
                + "\n\t" + CmdPrefix + CmdReset + "\t\tResets machine."
                + "\n\t" + CmdPrefix + CmdReset + " rnd\tResets machine with "
                                                + "random values in memory."
                + "\n\t" + CmdPrefix + CmdDump + " <addr>\tShow raw memory."                                                
                + "\n\t" + CmdPrefix + CmdEnd + "\t\tExit this REL.";
    
        /// <summary>
        /// Initializes a new <see cref="T:REL"/>.
        /// </summary>
        public REL(Machine m)
        {
            this.Machine = m;
            
            this.Machine.Inputter = DoUserInput;
            this.Machine.Outputter = Console.Write;
        }
        
        private string DoUserInput(string msg)
        {
            Console.Write( msg );
            return Console.ReadLine();
        }
        
        private string PromptOrder()
        {
            Console.Write( "\n" + Prompt );
            return Console.ReadLine();
        }
        
        private string Dump(BigInteger address, int max = DumpMaxBytes)
        {
            BigInteger addr = address;
            var toret = new StringBuilder( max + ( max * 3 ) );
            byte[] rawBytes = this.Machine.Memory.Read( address, max );
            
            // The raw byte values
            foreach(byte b in rawBytes) {
                toret.Append( b.ToString( "x2" ) );
                toret.Append( ' ' );
            }
            
            // Bytes as chars
            foreach(byte b in rawBytes) {
                char ch = b.ToChar();
                
                if ( char.IsControl( ch ) ) {
                    ch = '.';
                }
                
                toret.Append( ch );
            }
            
            return toret.ToString();
        }
        
        private string GetVariableList()
        {
            IList<Variable> vbles = this.Machine.TDS.Variables;
            var toret = new StringBuilder( 15 * vbles.Count );
            
            foreach(Variable vble in vbles) {
                toret.Append( vble.Address );
                toret.Append( "\t\t" );
                toret.Append( vble.Type );
                toret.Append( "\t\t" );
                toret.Append( vble.Name );
                toret.Append( '\n' );
            }
            
            return toret.ToString().Trim();
        }
        
        private BigInteger ParseNumericArg(string arg)
        {
            BigInteger toret = 0;
            
            if ( arg.StartsWith( HexPrefix, StringComparison.InvariantCulture ) )
            {
                string hexValue = arg.Substring( HexPrefix.Length );
                
                if ( BigInteger.TryParse( hexValue,
                                          NumberStyles.HexNumber,
                                          NumberFormatInfo.InvariantInfo,
                                          out BigInteger x ) )
                {
                    toret = x;
                }
            } else {
                if ( BigInteger.TryParse( arg, out BigInteger x ) ) {
                    toret = x;
                }
            }
            
            return toret;
        }
        
        /// <summary>
        /// Loads the file from the second of the specified args.
        /// </summary>
        /// <param name="args">An array of string.</param>
        public void Load(string[] args)
        {
            if ( args.Length >= 2 ) {
               try {
                    this.Machine.Load( args[ 1 ], (line, vble) => {} );
               } catch(IOException exc) {
                    Console.Error.WriteLine( "I/O Error: " + exc.Message );
               } catch(EngineException exc) {
                    Console.Error.WriteLine( "Error: " + exc.Message );
               }
            } else {
                Console.WriteLine( CmdLoad + " <f>??" );
            }
        }
        
        /// <summary>
        /// Runs the command in the input.
        /// </summary>
        /// <param name="input">The user's input.</param>
        private void RunCommand(string input)
        {
            var InvCulture = StringComparison.InvariantCulture;
        
            if ( input.StartsWith( CmdPrefix, InvCulture ) ) {
                input = input.Remove( 0, CmdPrefix.Length );
                var parts = input.Split( ' ' );
                string cmd = parts[ 0 ];
                
                if ( cmd == CmdEnd ) {
                    Environment.Exit( 0 );
                }
                else
                if ( cmd == CmdHelp ) {
                    Console.WriteLine( Help );
                }
                else
                if ( cmd == CmdDir ) {
                    Console.WriteLine( this.GetVariableList() );
                }
                else
                if ( cmd == CmdDump ) {
                    BigInteger address = 0;
                    
                    if ( parts.Length > 1 ) {
                        address = ParseNumericArg( parts[ 1 ]);
                    }
                    
                    Console.WriteLine( Dump( address ) );
                }
                else
                if ( cmd == CmdReset ) {
                    MemoryManager.ResetType rt = MemoryManager.ResetType.Zero;
                    
                    if ( parts.Length > 1
                      && parts[ 1 ] == ArgRandom )
                    {
                        rt = MemoryManager.ResetType.Random;
                    }
                    
                    this.Machine.Reset( rt );
                }
                else
                if ( cmd == CmdLoad ) {
                   this.Load( parts );
                } else {
                    Console.WriteLine( input + "??" );
                }
            }

            return;
        }
        
        /// <summary>Run the REL.</summary>
        public void Run()
        {
            var InvCulture = StringComparison.InvariantCulture;
            string input;

            Console.WriteLine( AppInfo.Header );
            Console.WriteLine( ShowAssistance );
            
            try {
	            input = this.PromptOrder();
	            while( input != CmdEnd ) {
	                input = input.Trim();
	                
	                if ( input.StartsWith( CmdPrefix, InvCulture ) ) {
                        this.RunCommand( input );
	                } else {
	                    try {
	                        this.Machine.Execute( input );
	                    } catch(EngineException exc)
	                    {
	                        Console.Error.WriteLine( "Error " + exc.Message );
                        }
	                   
	                }
	            
	                input = this.PromptOrder();
	            }
            }
            catch(Exception exc)
            {
                Console.Error.WriteLine( "Internal error " + exc.Message );
            }
            
            return;
        }
        
        /// <summary>Gets the machine to use in the REL.</summary>
        /// <value>The <see cref="Machine"/>.</value>
        public Machine Machine {
            get; private set;
        }
    }
}
