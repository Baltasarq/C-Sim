// CSim - (c) 2017 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Ui {
    using System;
    
    using Core;

    /// <summary>
    /// Creates a Read - Eval - Loop for using the <see cref="Machine"/>.
    /// </summary>
    public class REL {
        /// <summary>The prompt to use in the REL.</summary>
        public const string Prompt = "> ";
        /// <summary>The prefix for all REL commands.</summary>
        public const string CmdPrefix = ".";
        /// <summary>The command to exit the REL.</summary>
        public const string CmdEnd = "exit";
        /// <summary>The command to show existing variables.</summary>
        public const string CmdDir = "dir";
        /// <summary>The command to show available commands.</summary>
        public const string CmdHelp = "help";
        /// <summary>Show the available commands.</summary>
        public const string ShowAssistance = "Type '"
                + CmdPrefix + CmdHelp + "' to show assistance.";
        /// <summary>Show the available commands.</summary>
        public const string Help = "Available commands:"
                + "\n\t" + CmdPrefix + CmdHelp + "\t\tThis help."
                + "\n\t" + CmdPrefix + CmdDir + "\t\tShow existing variables."
                + "\n\t" + CmdPrefix + CmdEnd + "\t\tExit this REL.";
    
        /// <summary>
        /// Initializes a new <see cref="T:REL"/>.
        /// </summary>
        public REL(Machine m)
        {
            this.Machine = m;
            
            this.Machine.Inputter = DoUserInput;
            this.Machine.Outputter = Console.WriteLine;
        }
        
        private string DoUserInput(string msg)
        {
            Console.Write( msg );
            return Console.ReadLine();
        }
        
        private string PromptOrder()
        {
            Console.Write( Prompt );
            return Console.ReadLine();
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
	                    input = input.Remove( 0, CmdPrefix.Length );
	                    
	                    if ( input == CmdEnd ) {
	                        break;
	                    }
                        else
                        if ( input == CmdHelp ) {
                            Console.WriteLine( Help );
                        }
	                    else
	                    if ( input == CmdDir ) {
	                        foreach(Variable vble in this.Machine.TDS.Variables) {
	                            Console.WriteLine( vble.Name );
	                        }
	                    }
	                } else {
	                    try {
	                        this.Machine.Execute( input );
	                    } catch(EngineException exc)
	                    {
	                        Console.WriteLine( "Error " + exc.Message );
                        }
	                   
	                }
	            
	                input = this.PromptOrder();
	            }
            }
            catch(Exception exc)
            {
                Console.WriteLine( "Internal error " + exc.Message );
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
