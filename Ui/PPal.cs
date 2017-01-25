// Main.cs

using System;
using System.Windows.Forms;

namespace CSim.Ui {
    /// <summary>
    /// The entry point of the application
    /// </summary>
    public class PPal {
        [STAThread]
        public static void Main()
        {
            var mainWindow = new MainWindow();

            Application.Run( mainWindow );
        }
    }
}

