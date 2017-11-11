// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Ui {
    using System;
    using System.Windows.Forms;
    
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

