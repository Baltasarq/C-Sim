﻿// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Ui {
    using System.Drawing;
    using System.Windows.Forms;
    
    /// <summary>A simple input dialog.</summary>
    public class InputDialog: Form {
        /// <summary>
        /// Initializes a new <see cref="T:InputDialog"/>.
        /// </summary>
        public InputDialog(string msg)
        {
            this.InputMessage = msg;
            this.BuildIcons();
            this.Build();
            
            this.Shown += (sender, e) => {
                this.CenterToParent();
                this.Icon = this.Owner?.Icon;
            };
        }
        
        private void BuildIcons()
        {
            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            var resourceIconYes = assembly.
                GetManifestResourceStream( "CSim.Res.yes.png" );

            // Prepare icons
            if ( resourceIconYes != null ) {
                this.yesIcon = new Bitmap( resourceIconYes );
                this.noIcon = new Bitmap(
                    assembly.GetManifestResourceStream( "CSim.Res.no.png" ) );
            }

            return;
        }
        
        private void Build()
        {
            this.SuspendLayout();
        
            var lblMessage = new Label {
                Dock = DockStyle.Top,
                Text = this.InputMessage };
            
            this.edInput = new TextBox { Dock = DockStyle.Top };
            var pnlButtons = new TableLayoutPanel {
                            Dock = DockStyle.Top,
                            GrowStyle = TableLayoutPanelGrowStyle.AddColumns,
                            RowCount = 1,
                            ColumnCount = 2 };
            var btCancel = new Button { Anchor = AnchorStyles.Right,
                                        Image = noIcon,
                                        DialogResult = DialogResult.Cancel };
            var btOk = new Button { Anchor = AnchorStyles.Right,
                                    Image = this.yesIcon,
                                    DialogResult = DialogResult.OK };
            
            pnlButtons.Controls.Add( btOk );
            pnlButtons.Controls.Add( btCancel );
            
            this.AcceptButton = btOk;
            this.CancelButton = btCancel;

            var pnlMain = new TableLayoutPanel { Dock = DockStyle.Fill };            
            pnlMain.Controls.Add( lblMessage );
            pnlMain.Controls.Add( this.edInput );
            pnlMain.Controls.Add( pnlButtons );
            this.Controls.Add( pnlMain );
            this.ResumeLayout( true );
            this.Size = this.MinimumSize = new Size( 320,
                        lblMessage.Height + this.edInput.Height + pnlButtons.Height
            );
        }
        
        /// <summary>Gets the input message to use.</summary>
        public string InputMessage {
            get; private set;
        }
        
        /// <summary>Gets the resulting input.</summary>
        /// <value>The input, as a string.</value>
        public string Input {
            get {
                return edInput.Text;
            }
        }
        
        private TextBox edInput;
        private Bitmap yesIcon;
        private Bitmap noIcon;
    }
}
