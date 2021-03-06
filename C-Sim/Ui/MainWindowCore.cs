﻿// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Ui {
	using System;
    using System.Numerics;
    using System.IO;
	using System.Globalization;
	using System.Drawing;
    using System.Windows.Forms;
	using System.Collections.ObjectModel;

	using Core;
	using Core.Exceptions;

    /// <summary>
    /// The main window of the application.
    /// </summary>
    public partial class MainWindow: Form {
        /// <summary>Text to show when there is no vble to watch.</summary>
        public const string DefaultWatchText = "[]";
        /// <summary>Text to show when the watch was not found.</summary>
        public const string ErrorWatchText = "??";
		/// <summary>The maximum number of watches.</summary>
		public const int NumWatches = 10;
		/// <summary>The step in which fonts are increased or decreased in size.</summary>
        public const int FontStep = 2;
		/// <summary>The max data columns in memory view.</summary>
        public const int MaxDataColumns = 16;
		/// <summary>The label to read the locale from configuration.</summary>
		public const string EtqLocale = "locale";
		/// <summary>The name of the cfg file (only file name).</summary>
		public const string CfgFileName = "." + AppInfo.Name + ".cfg";
		/// <summary>The default size of the drawing area.</summary>
		public readonly Size DefaultDrawingAreaSize = new Size( 640, 640 );

		/// <summary>
		/// Initializes a new <see cref="MainWindow"/>.
		/// </summary>
        public MainWindow(Machine m)
        {
            this.updatingMemoryView = false;
            this.machine = m;
			this.machine.Inputter = (msg) => this.DoUserInput( msg );
            this.machine.Outputter = (string s) => this.rtbOutput.Text += s;
			this.ReadConfiguration();

            // Prepare UI
            this.baseFont = new Font( FontFamily.GenericMonospace, 12 );
            this.Build();
			this.SetStatus();
			CurrentDir = Environment.GetFolderPath( Environment.SpecialFolder.Desktop );
			this.doNotApplySnapshot = false;

			 // Prepare environment
			this.ChangeUILanguage( Locale.CurrentLocale );
            this.DoReset();
			this.DoIncreaseFont();
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
                + ", " + this.machine.Memory.Max
				+ " bytes RAM, "
				+ ( this.machine.Endian == Machine.Endianness.BigEndian ? "big endian" : "little endian" );
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
                    BigInteger pos = vble.Address;
					int row = (int) pos / MaxDataColumns;
					int col = (int) ( ( pos % MaxDataColumns ) +1 );

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

		private void PrintError(string msg)
		{
			this.PrintOutput( "ERROR: " + msg );
		}

		/// <summary>
		/// Prints to the output.
		/// </summary>
		/// <param name='msg'>
		/// The message to print.
		/// </param>
		private void PrintOutput(string msg)
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

			while ( t < tf ) {
				Application.DoEvents();
				t = DateTime.Now;
			}
		}

		private void OnHistoryIndexChanged()
		{
			int i = this.lbHistory.SelectedIndex;

			if ( i >= 0
		  	  && !( this.doNotApplySnapshot ) )
			{
				this.machine.SnapshotManager.ApplySnapshot( i );
				this.UpdateView();
			}

			return;
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
        
        private void DoCellFormat(DataGridViewCellFormattingEventArgs e)
        {
            string val = (string) e.Value;
            
            e.Value = val.ToLower();
            e.FormattingApplied = true;
        }
        
        private void DoCellEdited(int rowIndex, int colIndex)
        {
            if ( !this.updatingMemoryView
              && colIndex > 0 )                     // First row is for indexes
            {
                string val = (string)
                            this.grdMemory.Rows[ rowIndex ].
                                Cells[ colIndex ].Value;
    
                if ( byte.TryParse(
                                    val,
                                    NumberStyles.HexNumber,
                                    NumberFormatInfo.InvariantInfo,
                                    out byte x ) )
                {
                    int pos = ( rowIndex * MaxDataColumns ) + colIndex - 1;
                    this.machine.Memory.Write( pos, new []{ x } );
                }
            }
            
            return;
        }
        
        private void DoCellValidate(DataGridViewCellValidatingEventArgs e)
        {
            int x = -1;
            bool isHex = int.TryParse(
                                    (string) e.FormattedValue,
                                    NumberStyles.HexNumber,
                                    NumberFormatInfo.InvariantInfo,
                                    out x );

            e.Cancel = ( !isHex || x < 0 || x >= byte.MaxValue );
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
		
        /// <summary>
        /// Shows the settings panel.
        /// </summary>
		private void ShowSettings()
		{
			// Prepare language
			this.cbLocales.Text = Locale.CurrentLocaleToDescription();

            // Prepare alignment
            this.chkAlign.Checked = this.machine.AlignVbles;

			// Prepare endianness
			if ( this.machine.Endian == Machine.Endianness.BigEndian ) {
				this.rbEndianness2.Checked = true;
			}
			else
			if ( this.machine.Endian == Machine.Endianness.LittleEndian ) {
				this.rbEndianness1.Checked = true;
			}

            // Prepare word size
            switch ( this.machine.WordSize ) {
            case 2:
                this.rbWS16.Checked = true;
                break;
            case 4:
                this.rbWS32.Checked = true;
                break;
            case 8:
                this.rbWS64.Checked = true;
                break;
            default:
                throw new ArgumentException( "invalid word size" );
            }

            // UI
            this.tbIconBar.Hide();
            this.splMain.Hide();
			this.pnlSettings.Show();
			this.cbLocales.Focus();
            this.SetStatus( L10n.Get( L10n.Id.ActSettings ) );
		}

        /// <summary>
        /// Applies the chosen settings.
        /// </summary>
		private void ApplySettings()
		{
            // Locales
			string locale = string.Empty;

			if ( this.cbLocales.SelectedItem != null ) {
				locale = this.cbLocales.Text;
			}

            // Alignment
            this.machine.AlignVbles = this.chkAlign.Checked;

			// Apply language configuration
			Locale.SetLocaleFromDescription( locale );
			this.ChangeUILanguage( Locale.CurrentLocale );

			// Apply endianness configuration
			var endian = Machine.Endianness.BigEndian;
			if ( this.rbEndianness1.Checked ) {
				endian = Machine.Endianness.LittleEndian;
			}

			if ( endian != this.machine.Endian ) {
                this.machine.SwitchEndianness();
			}

			// Apply word size configuration
			if ( this.rbWS16.Checked
			  && this.machine.WordSize != 2 )
			{
				this.machine.WordSize = 2;
				this.DoReset();
			}
			else
			if ( this.rbWS32.Checked
			  && this.machine.WordSize != 4 )
			{
				this.machine.WordSize = 4;
				this.DoReset();
			}
			else
			if ( this.rbWS64.Checked
			  && this.machine.WordSize != 8 )
			{
				this.machine.WordSize = 8;
				this.DoReset();
			}

			// UI
			this.UpdateView();
            this.pnlSettings.Hide();
			this.tbIconBar.Show();
            this.splMain.Show();
            this.SetStatus();
			this.FocusOnInput();
		}

		private void DoReset()
		{
			this.rtbOutput.Text = "";
			this.lbHistory.Items.Clear();
			this.sdDrawingBoard.Machine = this.machine;
			this.GoToFinalSnapshot();

			this.DoReset( MemoryManager.ResetType.Zero );
			this.UpdateView();
			this.DoSwitchToDrawing();
			this.PrintOutput( AppInfo.Header );
		}
        
        /// <summary>
        /// Loads the program in the specified fn.
        /// </summary>
        /// <param name="fn">The file name.</param>
        public void LoadProgram(string fn)
        {
            try {
                this.machine.Load(
                    fn,
                    (line, result) =>
                        this.UpdateUiForEachInstruction( line, result ) );
            } catch(IOException exc) {
                this.SetStatus( exc.Message );
            } catch(EngineException exc) {
                this.SetStatus( exc.Message );
            }
            finally {
                this.UpdateView();
            }
        }

		private void DoOpen()
		{
            var dlg = new OpenFileDialog() {
                Title = "Load session",
                Filter = AppInfo.Name + "|*." + AppInfo.FileExt + "|*|*.*",
                CheckFileExists = true,
                FileName = Path.GetFullPath( CurrentDir )
            };

            this.SetStatus();
			if ( dlg.ShowDialog() == DialogResult.OK ) {
				this.DoReset();
                this.LoadProgram( dlg.FileName );
			}

			CurrentDir = Path.GetFullPath( dlg.FileName );
			return;
		}

		private void DoSave()
		{
            var dlg = new SaveFileDialog() {
                Title = "Save session",
                Filter = AppInfo.Name + "|*." + AppInfo.FileExt + "|*|*.*",
                FileName = Path.GetFullPath( CurrentDir )
            };

            if ( dlg.ShowDialog() == DialogResult.OK ) {
				using( var writer = new StreamWriter( dlg.FileName, false ) )
				{
					foreach(string s in this.lbHistory.Items) {
						writer.WriteLine( s );
					}
				}
			}

			CurrentDir = Path.GetFullPath( dlg.FileName );
			return;
		}

		private void DoHelp()
		{
			System.Diagnostics.Process.Start( AppInfo.Web );
		}
        
        private string DoUserInput(string msg)
        {
            string toret = "";
            var dlgInput = new InputDialog( msg );
            
            if ( dlgInput.ShowDialog( this ) == DialogResult.OK ) {
                toret = dlgInput.Input;
                this.PrintOutput( toret );
            }
            
            return toret;
        }

        private void DoReset(MemoryManager.ResetType rt)
        {
			// Reset view
            this.lbHistory.Text = "";
			this.lbHistory.Items.Clear();
			this.rtbOutput.Text = "";

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
			this.UpdateWatches();
			this.DoDrawing();
			this.SetStatus();
			this.FocusOnInput();
		}

		private void UpdateWatches()
		{
			for (int i = 0; i < this.edWatchesLabel.Length; ++i) {
				Label lblWatch = this.lblWatchesValue[ i ];
				string id = this.edWatchesLabel[ i ].Text.Trim();

				if ( !string.IsNullOrEmpty( id ) ) {
	                if ( this.machine.TDS.IsIdOfExistingVariable( id ) ) {
	                    Variable vble = this.machine.TDS.LookUp( id );

                        lblWatch.ForeColor = SystemColors.InfoText;
                        lblWatch.BackColor = SystemColors.Info;
						lblWatch.Text = vble.Value.ToString();
					} else {
                        lblWatch.ForeColor = SystemColors.GrayText;
                        lblWatch.BackColor = SystemColors.Window;
                        lblWatch.Text = ErrorWatchText;
	                }
                } else {
                    if ( lblWatch.Text != DefaultWatchText ) {
	                    lblWatch.ForeColor = SystemColors.InfoText;
	                    lblWatch.BackColor = SystemColors.Info;
	                    lblWatch.Text = DefaultWatchText;
                    }
                }
            }

			return;
		}

        private void UpdateSymbolTable()
        {
            ReadOnlyCollection<Variable> variables = this.machine.TDS.Variables;

			this.tvSymbolTable.Nodes.Clear();
            foreach(Variable vble in variables) {
                if ( vble.IsTemp() ) {
                    continue;
                }

				string varTypeAddr = vble.Type.Name + " :" + vble.Type.Size
					+ " ["
					+ FromIntToPrettyHex( vble.Address, this.machine.WordSize )
					+ ']';

                var vbleNode = new TreeNode( vble.Name.Text );
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

            this.updatingMemoryView = true;
        
			// Row indexes (first cell in each row)
			for (int i = 0; i < this.grdMemory.RowCount; ++i) {
				this.grdMemory.Rows[ i ].Cells[ 0 ].Value = FromIntToHex( i );
			}

            // Contents
            for(int i = 0; i < memory.Count; ++i) {
                int row = i / MaxDataColumns;
                int col = ( i % MaxDataColumns ) + 1;

				this.grdMemory.Rows[ row ].Cells[ col ].Value = FromIntToHex( memory[ i ] );
				this.grdMemory.Rows[ row ].Cells[ col ].ToolTipText = memory[ i ].ToString();
            }

            this.updatingMemoryView = false;
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
        
        private int CharHeight
        {
            get {
                if ( this.charHeight == -1 ) {
                    Graphics grf = this.CreateGraphics();
                    SizeF fontSize = grf.MeasureString( "H", this.Font );
                    this.charHeight = (int) fontSize.Height +5;
                }

                return this.charHeight;
            }
        }

        private void DoDrawing()
        {
			// Estimate sizes
			this.bmDrawArea = new Bitmap( DefaultDrawingAreaSize.Width, DefaultDrawingAreaSize.Height );
            this.pbCanvas.Image = bmDrawArea;
			this.sdDrawingBoard.InitGraphics( this.bmDrawArea );
			this.sdDrawingBoard.CalculateSizes();

			// Adjust
			Size drawingSize = this.sdDrawingBoard.Size;
			int width = Math.Max( DefaultDrawingAreaSize.Width, drawingSize.Width );
			int height = Math.Max( DefaultDrawingAreaSize.Height, drawingSize.Height );

			if ( width != 1024
			  || height != 1024 )
			{
				this.bmDrawArea = new Bitmap( width, height );
				this.pbCanvas.Image = bmDrawArea;
			}

			// Draw
            this.sdDrawingBoard.Draw( this.bmDrawArea );
			this.FocusOnInput();
        }

		/// <summary>
		/// Gets the complete path to the cfg file.
		/// </summary>
		/// <value>The path, as a string.</value>
		public string CfgFile
		{
			get {
				if ( this.cfgFile.Trim().Length == 0 ) {
					this.cfgFile = (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
							? Environment.GetEnvironmentVariable( "HOME" )
							: Environment.ExpandEnvironmentVariables( "%HOMEDRIVE%%HOMEPATH%" );
					this.cfgFile = Path.Combine( cfgFile, CfgFileName );
				}

				return cfgFile;
			}
		}

		/// <summary>
		/// Reads the configuration from the configuration file.
		/// </summary>
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

					if ( line.ToLower().StartsWith( EtqLocale, StringComparison.InvariantCulture ) )
					{
						if ( pos > 0 ) {
							Locale.SetLocale( arg );
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
						L10n.Get( L10n.Id.StReadingConfig ),
						exc.Message ),
					AppInfo.Name,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
			}
		}

		/// <summary>
		/// Writes the configuration to the configuration file
		/// </summary>
		protected void WriteConfiguration()
		{
			try {
				var file = new StreamWriter( this.CfgFile );

				file.WriteLine( "{0}={1}", EtqLocale, Locale.GetCurrentLocaleCode() );
				file.WriteLine();
				file.Close();
			} catch(Exception exc)
			{
				MessageBox.Show(
					this,
					string.Format( "{0}:\n{1}",
						L10n.Get( L10n.Id.StWritingConfig ),
						exc.Message ),
					AppInfo.Name,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
			}
		}

		private string FromVbleToString(Variable x)
		{
			string toret = string.Format( "{0}({1} [{2}]) = {3}",
				x.Name.Value,
				x.Type,
				FromIntToPrettyHex( x.Address, this.machine.WordSize ),
                x.LiteralValue );

			// Es char *?
			if ( x.Type == this.machine.TypeSystem.GetPCharType()
              && !( x.IsTemp() ) )
			{
	            Variable str = this.machine.TDS.LookForAddress(
	                                                    x.Value.ToBigInteger() );
	            
				if ( str != null ) {
					toret += ": " + str.LiteralValue;
	            }
			}
            
			return toret;
		}
        
        private void UpdateUiForEachInstruction(string input, Variable result)
        {
            if ( result != null ) {
                this.AddToHistory( input );
                this.PrintOutput( this.FromVbleToString( result ) );
            }
            
            return;
        }

		/// <summary>
		/// Executes a given command.
		/// </summary>
		private void Execute(string input)
		{
			try {
                this.UpdateUiForEachInstruction(
                                                input,
                                                this.machine.Execute( input ) );
			}
			catch(EngineException exc)
			{
				this.PrintError( exc.Message );
			}
            #if !DEBUG
            catch(Exception exc) {
                this.PrintError( "ERROR (internal): " + exc.Message ); 
            }
            #endif
			finally {
				this.UpdateView();
			}

			return;
		}

		private void ChangeUILanguage(CultureInfo locale)
		{
			L10n.SetLanguage( locale );

			// Toolbar buttons
            this.tbbReset.ToolTipText = L10n.Get( L10n.Id.ActReset );
			this.tbbOpen.ToolTipText = L10n.Get( L10n.Id.ActOpen );
			this.tbbSave.ToolTipText = L10n.Get( L10n.Id.ActSave );
			this.tbbHex.ToolTipText = L10n.Get( L10n.Id.ActInHex );
			this.tbbDec.ToolTipText = L10n.Get( L10n.Id.ActInDec );
            this.tbbZoomIn.ToolTipText = L10n.Get( L10n.Id.ActZoomIn );
			this.tbbZoomOut.ToolTipText = L10n.Get( L10n.Id.ActZoomOut );
			this.tbbMemory.ToolTipText = L10n.Get( L10n.Id.ActShowMemory );
			this.tbbDiagram.ToolTipText = L10n.Get( L10n.Id.ActShowDiagram );
            this.tbbStop.ToolTipText = L10n.Get( L10n.Id.ActStop );
            this.tbbPlay.ToolTipText = L10n.Get( L10n.Id.ActPlay );
			this.tbbSettings.ToolTipText = L10n.Get( L10n.Id.ActSettings );
			this.tbbHelp.ToolTipText = L10n.Get( L10n.Id.ActHelp );
			this.tbbAbout.ToolTipText = L10n.Get( L10n.Id.ActAbout );

			// Tabbed panel
			this.tcTabs.TabPages[ 0 ].Text = L10n.Get( L10n.Id.ActShowMemory );
			this.tcTabs.TabPages[ 1 ].Text = L10n.Get( L10n.Id.ActShowDiagram );

			// Polishing
			this.lblLocales.Text = L10n.Get( L10n.Id.LblLanguage );
		}

		private static string FromIntToHex(BigInteger value, int wordSize = 1)
		{
			return Literal.ShortenNumber( value.ToString( "x" ).PadLeft( wordSize * 2, '0' ) );
		}

		private static string FromIntToPrettyHex(BigInteger value, int wordSize = 1)
		{
			return @"0x" + Literal.ShortenNumber( FromIntToHex( value, wordSize ) );
		}

		private bool doNotApplySnapshot;
        private int charWidth = -1;
        private int charHeight = -1;
        private Machine machine;
        private SchemaDrawer sdDrawingBoard;
		private string cfgFile = "";
        private bool updatingMemoryView;

		/// <summary>
		/// The current dir, the last one from which a file was last loaded.
		/// </summary>
		public static string CurrentDir;

    }
}

