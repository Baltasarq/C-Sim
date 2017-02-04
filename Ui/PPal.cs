// Main.cs

using System;
using System.Windows.Forms;

namespace CSim.Ui {
    /// <summary>
    /// The entry point of the application
    /// </summary>
    public class PPal {
		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
        [STAThread]
        public static void Main()
        {
			Application.Run( new MainWindow() );
        }
    }
}

