namespace CSim.Gui {
	using System;
	using System.Globalization;
	using System.IO;
	using System.Windows.Forms;
	using System.Drawing;
	using System.Collections.ObjectModel;

	using CSim.Core;
	using CSim.Core.Exceptions;

    /// <summary>
    /// The main window of the application.
    /// </summary>
    public partial class MainWindow: Form {
        public const int FontStep = 2;
        public const int MaxDataColumns = 16;
		public const string EtqLocale = "locale";
		public const string CfgFileName = "." + AppInfo.Name + ".cfg";

        public MainWindow()
        {
			this.machine = new Machine();
			this.ReadConfiguration();

            // Prepare UI
            this.baseFont = new Font( FontFamily.GenericMonospace, 12 );
            this.Build();
			this.SetStatus();
			currentDir = Environment.GetFolderPath( Environment.SpecialFolder.Desktop );
			this.doNotApplySnapshot = false;

			 // Prepare environment
			this.ChangeUILanguage( Core.Locale.CurrentLocale );
            this.DoReset();
            this.UpdateView();
			this.DoIncreaseFont();
			this.DoSwitchToDrawing();
			this.printOutput( AppInfo.Name + " v" + AppInfo.Version + '\n' );
        }

        private void DoQuit()
        {
            this.Hide();
			this.WriteConfiguration();
            Application.Exit();
        }

        private void DoAbout()
        {
            this.pnlAbout.Show();
        }

		private void SetStatus()
		{
			this.lblStatus.Text = this.machine.GetMemoryWidth()
                + " " + this.machine.Memory.Max.ToString()
				+ " bytes";
		}

		private void SetStatus(string msg)
		{
			this.lblStatus.Text = msg;
		}

        private void DoTreeSelect()
        {
            var node = this.tvSymbolTable.SelectedNode;
            string path = node.FullPath;
            string[] nodes = path.Split( this.tvSymbolTable.PathSeparator[ 0 ] );

            if ( nodes.Length > 0 ) {
                string id = nodes[ 0 ];
                
				try {
					Variable vble = this.machine.TDS.LookUp( id );
                    int pos = vble.Address;
                    int row = pos / MaxDataColumns;
                    int col = ( pos % MaxDataColumns ) +1;

                    this.grdMemory.CurrentCell = this.grdMemory.Rows[ row ].Cells[ col ];
                    this.grdMemory.CurrentCell.Selected = true;
                    this.DoSwitchToMemory();
                } catch(UnknownVbleException) {
					// ignored -- the user clicked in a node in which there is no vble id.
				}
            }

			this.FocusOnInput();
            return;
        }

		/// <summary>
		/// Adds the command entered to the history.
		/// </summary>
		/// <param name="input">The entered command, as a string</param>
		private void AddToHistory(string input)
		{
			this.doNotApplySnapshot = true;
			this.lbHistory.Items.Add( input );
			this.lbHistory.SelectedIndex = this.lbHistory.Items.Count - 1;
			this.doNotApplySnapshot = false;
		}

		/// <summary>
		/// Prints to the output.
		/// </summary>
		/// <param name='msg'>
		/// The message to print.
		/// </param>
		private void printOutput(string msg)
		{
			this.rtbOutput.AppendText( msg + '\n' );
            this.rtbOutput.SelectionStart = this.rtbOutput.Text.Length;
            this.rtbOutput.ScrollToCaret();
		}

        /// <summary>
        /// Interpret and execute the input.
        /// </summary>
        private void DoInput()
		{
			string input = this.edInput.Text;

			this.edInput.Text = "";
			this.SetStatus();
			this.GoToFinalSnapshot();
			this.Execute( input );
			this.edInput.Items.Add( input );
        }

		private void GoToFinalSnapshot() {
			int numSnapshots = this.lbHistory.Items.Count - 1;

			if ( numSnapshots > 0 ) {
				this.lbHistory.SelectedIndex = numSnapshots;
			}

			return;
		}

		private void DoStop() {
			this.GoToFinalSnapshot();
		}

		/// <summary>
		/// Waits for an amount of seconds, without blocking the GUI.
		/// <param name="seconds">The amount of seconds to wait</param>
		/// </summary>
		public static void ActiveWait(int seconds)
		{
			DateTime t = DateTime.Now;
			DateTime tf = t.AddSeconds( seconds );

			while ( t < tf )
			{
				Application.DoEvents();
				t = DateTime.Now;
			}
		}

		private void DoPlay()
		{
			this.tbbStop.Enabled = true;

			if ( this.lbHistory.Items.Count > 0 ) {
				this.lbHistory.SelectedIndex = 0;
				ActiveWait( 1 );

				while( this.lbHistory.SelectedIndex < ( this.lbHistory.Items.Count -1 ) ) {
					this.lbHistory.SelectedIndex += 1;
					ActiveWait( 1 );
				}
			}

			this.tbbStop.Enabled = false;
			return;
		}

        private void DoSwitchToMemory()
        {
            this.SwitchToTab( 0 );
        }

        private void DoSwitchToDrawing()
        {
            this.SwitchToTab( 1 );
        }

        private void SwitchToTab(int x)
        {
            this.tcTabs.SelectedIndex = x;
			this.FocusOnInput();
        }

        private void DoIncreaseFont()
        {
            float size = this.baseFont.Size + FontStep;

            this.baseFont = new Font( this.baseFont.FontFamily, size );
            this.UpdateFont( FontStep );
        }

        private void DoDecreaseFont()
        {
            float size = this.baseFont.Size - FontStep;
            
            this.baseFont = new Font( this.baseFont.FontFamily, size );
            this.UpdateFont( FontStep * -1 );
        }

		private void DoDisplayInDec()
		{
			Literal.Display = Literal.DisplayType.Dec;
			this.UpdateView();
		}

		private void DoDisplayInHex()
		{
			Literal.Display = Literal.DisplayType.Hex;
			this.UpdateView();
		}

        /// <summary>
        /// Updates the font in all visible controls.
        /// </summary>
        /// <param name='fontStep'>
        /// The difference in size between the old font and the new font.
        /// </param>
        private void UpdateFont(int fontStep)
		{
			int delta = 0;

			// Controls
			this.edInput.Font = this.baseFont;
			this.tvSymbolTable.Font = this.baseFont;
			this.lbHistory.Font = this.baseFont;

			// Determine delta factor
			if ( fontStep != 0 ) {
				delta = ( fontStep / Math.Max( Math.Abs( fontStep ), 1 ) );
			}

			// Memory view
            this.grdMemory.Font = this.baseFont;
            this.grdMemory.DefaultCellStyle.Font = new Font( this.baseFont, FontStyle.Bold );
            this.grdMemory.ColumnHeadersDefaultCellStyle.Font = this.baseFont;
			this.grdMemory.ColumnHeadersHeight = this.baseFont.Height + 2;

            foreach(DataGridViewTextBoxColumn col in this.grdMemory.Columns) {
                col.Width += (int) ( col.Width * ( 0.10 * delta ) );

                if ( col.Index == 0 ) {
                    col.DefaultCellStyle.Font = this.baseFont;
                }
            }

            foreach(DataGridViewRow row in this.grdMemory.Rows) {
                row.Height += (int) ( row.Height * ( 0.10 * delta ) );
            }

			// Drawing
			this.sdDrawingBoard.UpdateFont( fontStep );
			this.DoDrawing();

            return;
        }
		
		private void ShowSettings()
		{
			// Prepare configuration
			this.cbLocales.Text = Core.Locale.CurrentLocaleToDescription();
			// this.chkShowMainMenu.Checked = this.IsMainMenuShown;

			// UI
			this.tbIconBar.Hide();
            this.pnlMain.Hide();
            this.pnlIO.Hide();
			this.pnlSettings.Show();
			this.cbLocales.Focus();

            this.SaveCurrentStatus();
            this.SetStatus( L18n.Get( L18n.Id.ActSettings ) );
		}

		private void ChangeSettings()
		{
			string locale = string.Empty;

			if ( this.cbLocales.SelectedItem != null ) {
				locale = this.cbLocales.Text;
			}

			// Apply configuration
			Core.Locale.SetLocaleFromDescription( locale );
			this.ChangeUILanguage( Core.Locale.CurrentLocale );
			// this.MainMenuStrip.Visible = this.IsMainMenuShown = this.chkShowMainMenu.Checked;

			// UI
            this.pnlSettings.Hide();
            this.RestoreStatus();
			this.tbIconBar.Show();
            this.pnlMain.Show();
            this.pnlIO.Show();
		}

		/// <summary>
		/// Saves the current status.
		/// </summary>
		protected void SaveCurrentStatus()
		{
			this.statusBeforeSettings = this.lblStatus.Text;
		}

		/// <summary>
		/// Restores the status.
		/// </summary>
		protected void RestoreStatus()
		{
			this.SetStatus( this.statusBeforeSettings );
		}

		private void DoReset()
		{
			this.DoReset( MemoryManager.ResetType.Zero );
		}

		private void DoOpen()
		{
			var dlg = new OpenFileDialog();

			dlg.Title = "Load session";
			dlg.Filter = AppInfo.Name + "|*." + AppInfo.FileExt + "|*|*.*";
			dlg.CheckFileExists = true;
			dlg.FileName = Path.GetFullPath( currentDir );

			this.SetStatus();
			if ( dlg.ShowDialog() == DialogResult.OK ) {
				this.DoReset();

				try {
					using (var sr = new StreamReader( dlg.FileName )) {
						string line = sr.ReadLine();
						while ( line != null ) {
							this.Execute( line );
							line = sr.ReadLine();
						}
					}
				} catch(IOException exc) {
					this.SetStatus( exc.Message );
				} catch(EngineException exc) {
					this.SetStatus( exc.Message );
				}
				finally {
					this.UpdateView();
				}
			}

			currentDir = Path.GetFullPath( dlg.FileName );
			return;
		}

		private void DoSave()
		{
			var dlg = new SaveFileDialog();

			dlg.Title = "Save session";
			dlg.Filter = AppInfo.Name + "|*." + AppInfo.FileExt + "|*|*.*";
			dlg.FileName = Path.GetFullPath( currentDir );

			if ( dlg.ShowDialog() == DialogResult.OK ) {
				using( StreamWriter writer = new StreamWriter( dlg.FileName, false ) )
				{
					foreach(string s in this.lbHistory.Items) {
						writer.WriteLine( s );
					}
				}
			}

			currentDir = Path.GetFullPath( dlg.FileName );
			return;
		}

		private void DoHelp()
		{
			System.Diagnostics.Process.Start( AppInfo.Web );
		}

        private void DoReset(MemoryManager.ResetType rt)
        {
			// Reset view
            this.lbHistory.Text = "";
			this.lbHistory.Items.Clear();

			// Reset machine
			this.machine.Reset( rt );
            this.UpdateView();
        }

		private void FocusOnInput()
		{
			this.edInput.Focus();
		}

		private void UpdateView()
		{
			this.UpdateMemoryView();
            this.UpdateSymbolTable();
			this.DoDrawing();

			this.FocusOnInput();
		}

        private void UpdateSymbolTable()
        {
            ReadOnlyCollection<Variable> variables = this.machine.TDS.Variables;

			this.tvSymbolTable.Nodes.Clear();

            foreach(Variable vble in variables) {
                string varTypeAddr = vble.Type.Name + " :" + vble.Type.Size
                    + " [" + Literal.ToPrettyNumber( vble.Address ) + ']';

                var vbleNode = new TreeNode( vble.Name.Name );
                var typeNode = new TreeNode( varTypeAddr );
                var contentsNode = new TreeNode( " = " + vble.LiteralValue );

                vbleNode.Nodes.Add( typeNode );
                vbleNode.Nodes.Add( contentsNode );
                this.tvSymbolTable.Nodes.Add( vbleNode );
            }

            this.tvSymbolTable.ExpandAll();
            return;
        }

        private void UpdateMemoryView()
		{
            var memory = this.machine.Memory.Raw;

			// Row indexes (first cell in each row)
			for (int i = 0; i < this.grdMemory.RowCount; ++i) {
				this.grdMemory.Rows[ i ].Cells[ 0 ].Value = Literal.ToHex( i * MaxDataColumns );
			}

            // Contents
            for(int i = 0; i < memory.Count; ++i) {
                int row = i / MaxDataColumns;
                int col = ( i % MaxDataColumns ) + 1;

                this.grdMemory.Rows[ row ].Cells[ col ].Value = Literal.ToHex( memory[ i ] );
				this.grdMemory.Rows[ row ].Cells[ col ].ToolTipText = memory[ i ].ToString();
            }

			this.FocusOnInput();
            return;
        }

        private int CharWidth
        {
            get {
                if ( this.charWidth == -1 ) {
                    Graphics grf = this.CreateGraphics();
                    SizeF fontSize = grf.MeasureString( "W", this.Font );
                    this.charWidth = (int) fontSize.Width +5;
                }

                return this.charWidth;
            }
        }

        private void DoDrawing()
        {
            this.bmDrawArea = new Bitmap( 4096, 512 );
            this.pbCanvas.Image = bmDrawArea;
            this.sdDrawingBoard.Draw( this.bmDrawArea );
			this.FocusOnInput();
        }

		public string CfgFile
		{
			get {
				if ( this.cfgFile.Trim().Length == 0 ) {
					this.cfgFile = (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
							? Environment.GetEnvironmentVariable( "HOME" )
							: Environment.ExpandEnvironmentVariables( "%HOMEDRIVE%%HOMEPATH%" );
					this.cfgFile = System.IO.Path.Combine( cfgFile, CfgFileName );
				}

				return cfgFile;
			}
		}

		protected void ReadConfiguration()
		{
			string line;
			StreamReader file = null;

			try {
				try {
					file = new StreamReader( CfgFile );
				} catch(Exception) {
					var fileCreate = new StreamWriter( CfgFile );
					fileCreate.Close();
					file = new StreamReader( CfgFile );
				}

				line = file.ReadLine();
				while( !file.EndOfStream ) {
					int pos = line.IndexOf( '=' );
					string arg = line.Substring( pos + 1 ).Trim();

					if ( line.ToLower().StartsWith( EtqLocale ) ) {
						if ( pos > 0 ) {
							Core.Locale.SetLocale( arg );
						}
					}

					line = file.ReadLine();
				}

				file.Close();
			} catch(Exception exc)
			{
				MessageBox.Show(
					this,
					string.Format( "{0}:\n{1}",
						L18n.Get( L18n.Id.StReadingConfig ),
						exc.Message ),
					AppInfo.Name,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
			}
		}

		protected void WriteConfiguration()
		{
			try {
				var file = new StreamWriter( this.CfgFile );

				file.WriteLine( "{0}={1}", EtqLocale, Core.Locale.GetCurrentLocaleCode() );
				file.WriteLine();
				file.Close();
			} catch(Exception exc)
			{
				MessageBox.Show(
					this,
					string.Format( "{0}:\n{1}",
						L18n.Get( L18n.Id.StWritingConfig ),
						exc.Message ),
					AppInfo.Name,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
			}
		}

		/// <summary>
		/// Executes a given command.
		/// </summary>
		private void Execute(string input)
		{
			Variable result = null;

			try {
				// Do it
				result = this.machine.Execute( input );
				this.AddToHistory( input );

				// Update output
				this.printOutput( string.Format( "{0}({1} [{2}]) = {3}",
					result.Name.Value,
					result.Type,
					Literal.ToPrettyNumber( result.Address ),
					result.LiteralValue ) );
			}
			catch(EngineException exc)
			{
				this.SetStatus( exc.Message );
				this.AddToHistory( input );
			}
			finally {
				this.UpdateView();
			}

			return;
		}

		private void ChangeUILanguage(CultureInfo locale)
		{
			L18n.SetLanguage( locale );

			// Toolbar buttons
			ToolBar.ToolBarButtonCollection tbButtons = this.tbIconBar.Buttons; 
			for(int i = 0; i < tbButtons.Count; ++i) {
				ToolBarButton button = tbButtons[ i ];

				switch( i ) {
					case 0: button.ToolTipText = L18n.Get( L18n.Id.ActReset );
							break;
					case 1: button.ToolTipText = L18n.Get( L18n.Id.ActOpen );
							break;
					case 2: button.ToolTipText = L18n.Get( L18n.Id.ActSave );
							break;
					case 3: button.ToolTipText = L18n.Get( L18n.Id.ActInHex );
							break;
					case 4: button.ToolTipText = L18n.Get( L18n.Id.ActInDec );
							break;
					case 5: button.ToolTipText = L18n.Get( L18n.Id.ActZoomIn );
							break;
					case 6: button.ToolTipText = L18n.Get( L18n.Id.ActZoomOut );
							break;
					case 7: button.ToolTipText = L18n.Get( L18n.Id.ActShowMemory );
							break;
					case 8: button.ToolTipText = L18n.Get( L18n.Id.ActShowDiagram );
							break;
					case 9: button.ToolTipText = L18n.Get( L18n.Id.ActSettings );
							break;
					case 10: button.ToolTipText = L18n.Get( L18n.Id.ActHelp );
							break;
					case 11: button.ToolTipText = L18n.Get( L18n.Id.ActAbout );
							break;
					case 12: button.ToolTipText = L18n.Get( L18n.Id.ActPlay );
							break;
					case 13: button.ToolTipText = L18n.Get( L18n.Id.ActStop );
							break;
				default:
					throw new ArgumentException( "unexpected toolbar button: unhandled" );
				}
			}

			// Tabbed panel
			this.tcTabs.TabPages[ 0 ].Text = L18n.Get( L18n.Id.ActShowMemory );
			this.tcTabs.TabPages[ 1 ].Text = L18n.Get( L18n.Id.ActShowDiagram );

			// Polishing
			this.lblLocales.Text = L18n.Get( L18n.Id.LblLanguage );
		}

		private bool doNotApplySnapshot;
        private int charWidth = -1;
        private Machine machine;
		private string statusBeforeSettings;
        private SchemaDrawer sdDrawingBoard;
		public static string currentDir;
		private string cfgFile = "";
    }
}

