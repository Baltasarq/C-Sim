// MainWindow.cs

using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.ObjectModel;

using CSim.Core;
using CSim.Core.Variables;
using CSim.Core.Exceptions;

namespace CSim.Gui {
    /// <summary>
    /// The main window of the application.
    /// </summary>
    public class MainWindow: Form {
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
			this.lbHistory.Items.Add( input );
			this.lbHistory.SelectedIndex = this.lbHistory.Items.Count - 1;
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
			Variable[] results = new Variable[ 0 ];
			string input = this.edInput.Text;

			// Prepare interface
			this.edInput.Text = "";
			this.SetStatus();

			try {
				// Do it
                results = ArchManager.Execute( this.machine, input );
				this.AddToHistory( input );

				// Update output
				foreach(Variable v in results) {
					this.printOutput( string.Format( "{0}({1} [{2}]) = {3}",
					                                v.Name.Value,
					                                v.Type,
					                                Literal.ToPrettyNumber( v.Address ),
                                                    v.LiteralValue ) );
				}
			}
			catch(EngineException exc)
			{
				this.SetStatus( exc.Message );
			}
			finally {
				this.UpdateView( results );
			}
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
		
        private void BuildInput()
        {
            // Line input
            this.edInput = new TextBox();
            this.edInput.Dock = DockStyle.Bottom;
            this.edInput.Font = this.baseFont;
			this.edInput.KeyDown += delegate(object sender, KeyEventArgs e) {
                if ( e.KeyCode == Keys.Enter ) {
                    this.DoInput();
					e.Handled = true;
					e.SuppressKeyPress = true;
                }
            };

            this.pnlIO.Controls.Add( this.edInput );
        }

        private void BuildHistory()
        {
            this.lbHistory = new ListBox();
            this.lbHistory.Font = this.baseFont;
            this.lbHistory.Dock = DockStyle.Fill;
            this.splHistory.Panel2.Controls.Add( this.lbHistory );
        }

        private void BuildMemoryView()
        {
            // Grid for memory
            int columnWidth = ( this.CharWidth * 2 );
            this.grdMemory = new DataGridView();
            this.grdMemory.AllowUserToResizeRows = false;
            this.grdMemory.AllowUserToAddRows = false;
            this.grdMemory.RowHeadersVisible = false;
            this.grdMemory.AutoGenerateColumns = false;
            this.grdMemory.MultiSelect = false;
			this.grdMemory.ShowCellToolTips = true;

            // Style for regular columns
            var style = new DataGridViewCellStyle();
            style.Font = new Font( this.baseFont, FontStyle.Bold );
            style.BackColor = Color.AntiqueWhite;
            style.ForeColor = Color.Black;
			style.SelectionBackColor = Color.Black;
			style.SelectionForeColor = Color.AntiqueWhite;
            this.grdMemory.DefaultCellStyle = style;

            // Style for headers
            var styleHeaders = new DataGridViewCellStyle();
            styleHeaders.BackColor = Color.Black;
            styleHeaders.ForeColor = Color.White;
            styleHeaders.Font = new Font( this.baseFont, FontStyle.Bold );
            this.grdMemory.ColumnHeadersDefaultCellStyle = styleHeaders;

            // Style for first column
            var styleFirstColumn = new DataGridViewCellStyle();
            styleFirstColumn.Font = new Font( this.baseFont, FontStyle.Bold );
            styleFirstColumn.BackColor = Color.Black;
            styleFirstColumn.ForeColor = Color.White;

            // Create columns
            DataGridViewTextBoxColumn[] columns = new DataGridViewTextBoxColumn[ MaxDataColumns +1 ];

            for(int i = 0; i < columns.Length; ++i) {
                columns[ i ] = new DataGridViewTextBoxColumn();

                if ( i == 0 ) {
                    columns[ i ].DefaultCellStyle = styleFirstColumn;
                    columns[ i ].HeaderText = "/";
                    columns[ i ].Width = columnWidth + ( (int) ( columnWidth * 0.2 ) );
                } else {
                    columns[ i ].HeaderText = Literal.ToHex( i -1 );
                    columns[ i ].Width = columnWidth;
                }

                columns[ i ].SortMode = DataGridViewColumnSortMode.NotSortable;
                columns[ i ].ReadOnly = true;
            }

            this.grdMemory.Columns.AddRange( columns );

            // Create rows
            var rows = new DataGridViewRow[ this.machine.Memory.Max / MaxDataColumns ];

            for(int i = 0; i < rows.Length; ++i) {
                rows[ i ] = new DataGridViewRow();
            }

            this.grdMemory.Rows.AddRange( rows );

            this.grdMemory.Dock = DockStyle.Fill;
            this.tcTabs.TabPages[ 0 ].Controls.Add( this.grdMemory );
            this.splHistory.SplitterDistance = this.CharWidth * 10;
        }

        private void BuildSymbolTable()
        {
            // Symbol table
            this.tvSymbolTable = new TreeView();
            this.tvSymbolTable.Dock = DockStyle.Fill;
            this.tvSymbolTable.Font = this.baseFont;
            this.tvSymbolTable.PathSeparator = ".";
            this.tvSymbolTable.AfterSelect += (sender, e) => this.DoTreeSelect();
            this.splSymbolTable.Panel1.Controls.Add( this.tvSymbolTable );
            this.splSymbolTable.SplitterDistance = this.CharWidth;
        }

        private void BuildAboutPanel()
        {
            // Panel for about info
            this.pnlAbout = new Panel();
            this.pnlAbout.SuspendLayout();
            this.pnlAbout.Dock = DockStyle.Bottom;
            this.pnlAbout.BackColor = Color.LightYellow;
            var lblAbout = new Label();
            lblAbout.Text = AppInfo.Name + " v" + AppInfo.Version + ", " + AppInfo.Author;
            lblAbout.Dock = DockStyle.Left;
            lblAbout.TextAlign = ContentAlignment.MiddleCenter;
            lblAbout.AutoSize = true;
            var font = new Font( lblAbout.Font, FontStyle.Bold );
            font = new Font( font.FontFamily, 14 );
            lblAbout.Font = font;
            var btCloseAboutPanel = new Button();
            btCloseAboutPanel.Text = "X";
            btCloseAboutPanel.Font = new Font( btCloseAboutPanel.Font, FontStyle.Bold );
            btCloseAboutPanel.Dock = DockStyle.Right;
            btCloseAboutPanel.Width = this.CharWidth * 5;
            btCloseAboutPanel.FlatStyle = FlatStyle.Flat;
            btCloseAboutPanel.FlatAppearance.BorderSize = 0;
			btCloseAboutPanel.Click += (obj, args) => this.pnlAbout.Hide();
            this.pnlAbout.Controls.Add( lblAbout );
            this.pnlAbout.Controls.Add( btCloseAboutPanel );
            this.pnlAbout.Hide();
            this.pnlAbout.MinimumSize = new Size( this.Width, lblAbout.Height +5 );
            this.pnlAbout.MaximumSize = new Size( Int32.MaxValue, lblAbout.Height +5 );
            this.pnlAbout.ResumeLayout();
            this.Controls.Add( this.pnlAbout );
        }

        private void BuildIcons()
        {
			var resourceAppIcon = System.Reflection.Assembly.
			                      GetEntryAssembly().
			                      GetManifestResourceStream( "CSim.Res.appIcon.png" );

            // Prepare icons
			if ( resourceAppIcon != null ) {
				this.appIconBmp = new Bitmap( resourceAppIcon );
				this.Icon = Icon.FromHandle( this.appIconBmp.GetHicon() );

				this.backIconBmp = new Bitmap( System.Reflection.Assembly.
					GetEntryAssembly().
					GetManifestResourceStream( "CSim.Res.back.png" ) );

				this.openIconBmp = new Bitmap( System.Reflection.Assembly.
					GetEntryAssembly().
					GetManifestResourceStream( "CSim.Res.open.png" ) );

				this.saveIconBmp = new Bitmap( System.Reflection.Assembly.
					GetEntryAssembly().
					GetManifestResourceStream( "CSim.Res.save.png" ) );

				this.resetIconBmp = new Bitmap( System.Reflection.Assembly.
					GetEntryAssembly().
					GetManifestResourceStream( "CSim.Res.reset.png" ) );

				this.zoomInIconBmp = new Bitmap( System.Reflection.Assembly.
					GetEntryAssembly().
					GetManifestResourceStream( "CSim.Res.zoom_in.png" ) );

				this.zoomOutIconBmp = new Bitmap( System.Reflection.Assembly.
					GetEntryAssembly().
					GetManifestResourceStream( "CSim.Res.zoom_out.png" ) );

				this.aboutIconBmp = new Bitmap( System.Reflection.Assembly.
					GetEntryAssembly().
					GetManifestResourceStream( "CSim.Res.info.png" ) );

				this.helpIconBmp = new Bitmap( System.Reflection.Assembly.
					GetEntryAssembly().
					GetManifestResourceStream( "CSim.Res.help.png" ) );

				this.memoryIconBmp = new Bitmap( System.Reflection.Assembly.
					GetEntryAssembly().
					GetManifestResourceStream( "CSim.Res.memory.png" ) );

				this.diagramIconBmp = new Bitmap( System.Reflection.Assembly.
					GetEntryAssembly().
					GetManifestResourceStream( "CSim.Res.diagram.png" ) );

				this.hexIconBmp = new Bitmap( System.Reflection.Assembly.
					GetEntryAssembly().
					GetManifestResourceStream( "CSim.Res.hex.png" ) );

				this.decIconBmp = new Bitmap( System.Reflection.Assembly.
					GetEntryAssembly().
					GetManifestResourceStream( "CSim.Res.dec.png" ) );

				this.zeroIconBmp = new Bitmap( System.Reflection.Assembly.
					GetEntryAssembly().
					GetManifestResourceStream( "CSim.Res.zero.png" ) );

				this.randIconBmp = new Bitmap( System.Reflection.Assembly.
					GetEntryAssembly().
					GetManifestResourceStream( "CSim.Res.random.png" ) );

				this.settingsIconBmp = new Bitmap( System.Reflection.Assembly.
					GetEntryAssembly().
					GetManifestResourceStream( "CSim.Res.settings.png" ) );
			}

			return;
        }

        private void BuildDrawingTab()
        {
            var scroll = new Panel();

            this.pbCanvas = new PictureBox();
            this.pbCanvas.BackColor = Color.GhostWhite;
            this.pbCanvas.ForeColor = Color.Navy;
            this.pbCanvas.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            this.pbCanvas.SizeMode = PictureBoxSizeMode.AutoSize;

            scroll.Dock = DockStyle.Fill;
            scroll.Controls.Add( this.pbCanvas );
            scroll.AutoScroll = true;
           
            this.tcTabs.TabPages[ 1 ].Controls.Add( scroll );
            this.sdDrawingBoard = new SchemaDrawer( this.machine.TDS );
        }
/*
		private void BuildToolbar()
		{
			var tbZero = new ToolStripButton( "Zero",
				this.zeroIconBmp, (o, e) => this.DoReset(),
				"Zero" );
			var tbRandom = new ToolStripButton( "Random",
				this.randIconBmp, (o, e) => this.DoReset( MemoryManager.ResetType.Random ),
				"Random" );

			tbZero.DisplayStyle = tbRandom.DisplayStyle = ToolStripItemDisplayStyle.Image;

			this.tbIconBar = new ToolStrip( new ToolStripItem[]
			{
					new ToolStripDropDownButton( "Reset", this.resetIconBmp,
						new ToolStripButton[] { tbZero, tbRandom } ),
					new ToolStripButton( "Open", this.openIconBmp, (o, e) => this.DoOpen(), "Open" ),
					new ToolStripButton( "Save", this.saveIconBmp, (o, e) => this.DoSave(), "Save" ),
					new ToolStripButton( "Hex", this.hexIconBmp, (o, e) => this.DoDisplayInHex(), "Hex" ),
					new ToolStripButton( "Dec", this.decIconBmp, (o, e) => this.DoDisplayInDec(), "Dec"  ),
					new ToolStripButton( "Zoom In", this.zoomInIconBmp, (o, e) => this.DoIncreaseFont(), "ZoomIn" ),
					new ToolStripButton( "Zoom out", this.zoomOutIconBmp, (o, e) => this.DoDecreaseFont(), "ZoomOut"  ),
					new ToolStripButton( "Memory", this.memoryIconBmp, (o, e) => this.DoSwitchToMemory(), "Memory" ),
					new ToolStripButton( "Visual", this.diagramIconBmp, (o, e) => this.DoSwitchToDrawing(), "Visual"  ),
					new ToolStripButton( "Help", this.helpIconBmp, (o, e) => this.DoHelp(), "Help" ),
					new ToolStripButton( "About", this.aboutIconBmp, (o, e) => this.DoAbout(), "About" ),
			} );

			// Remove the appearance of text from all of the buttons
			foreach (ToolStripItem bt in this.tbIconBar.Items) {
				bt.DisplayStyle = ToolStripItemDisplayStyle.Image;
			}

			this.tbIconBar.Dock = DockStyle.Top;
			this.tbIconBar.GripStyle = ToolStripGripStyle.Hidden;
			this.tbIconBar.AutoSize = false;
			this.tbIconBar.Height = 24;
			this.tbIconBar.ImageScalingSize = new Size( 24, 24 );
			this.Controls.Add( this.tbIconBar );
		}
*/
		private void BuildToolbar()
		{
            // Image list
			var imgList = new ImageList();
            imgList.ImageSize = new System.Drawing.Size( 24, 24 );
            imgList.Images.AddRange( new Bitmap[] {
                this.resetIconBmp, this.openIconBmp,
                this.saveIconBmp, this.hexIconBmp,
                this.decIconBmp, this.zoomInIconBmp,
                this.zoomOutIconBmp, this.memoryIconBmp,
                this.diagramIconBmp, this.helpIconBmp,
                this.aboutIconBmp, this.settingsIconBmp
            } );

            // Toolbar
			this.tbIconBar = new ToolBar();
			this.tbIconBar.AutoSize = true;
			this.tbIconBar.Dock = DockStyle.Top;
			this.tbIconBar.Appearance = ToolBarAppearance.Flat;
			this.tbIconBar.BorderStyle = BorderStyle.None;
			this.tbIconBar.ImageList = imgList;
			this.tbIconBar.ShowToolTips = true;

            // Toolbar buttons
			var tbbReset = new ToolBarButton();
			tbbReset.ImageIndex = 0;
			tbbReset.Style = ToolBarButtonStyle.DropDownButton;

			var mniZero = new MenuItem( "Zero", (o, e) => DoReset() );
			mniZero.OwnerDraw = true;
			mniZero.DrawItem += delegate(object sender, DrawItemEventArgs e) {
				double factor = (double) e.Bounds.Height / this.zeroIconBmp.Height;
				var rect = new Rectangle( e.Bounds.X, e.Bounds.Y,
				                         (int) ( this.zeroIconBmp.Width * factor ),
				                         (int) ( this.zeroIconBmp.Height * factor ) );
				e.Graphics.DrawImage( this.zeroIconBmp, rect );
			};

			var mniRandom = new MenuItem( "Random", (o, e) => DoReset( MemoryManager.ResetType.Random ) );
			mniRandom.OwnerDraw = true;
			mniRandom.DrawItem += delegate(object sender, DrawItemEventArgs e) {
				double factor = (double) ( e.Bounds.Height ) / this.randIconBmp.Height;
				var rect = new Rectangle( e.Bounds.X, e.Bounds.Y,
				                         (int) ( this.randIconBmp.Width * factor ),
				                         (int) ( this.randIconBmp.Height * factor ) );
				e.Graphics.DrawImage( this.randIconBmp, rect );
			};

			tbbReset.DropDownMenu = new ContextMenu( new MenuItem[]{
				mniZero, mniRandom,
			});

			var tbbOpen = new ToolBarButton();
			tbbOpen.ImageIndex = 1;
			var tbbSave = new ToolBarButton();
			tbbSave.ImageIndex = 2;
			var tbbHex = new ToolBarButton();
			tbbHex.ImageIndex = 3;
			var tbbDec = new ToolBarButton();
			tbbDec.ImageIndex = 4;
			var tbbZoomIn = new ToolBarButton();
			tbbZoomIn.ImageIndex = 5;
			var tbbZoomOut = new ToolBarButton();
			tbbZoomOut.ImageIndex = 6;
			var tbbMem = new ToolBarButton();
			tbbMem.ImageIndex = 7;
			var tbbVisual = new ToolBarButton();
			tbbVisual.ImageIndex = 8;
			var tbbHelp = new ToolBarButton();
			tbbHelp.ImageIndex = 9;
			var tbbAbout = new ToolBarButton();
            tbbAbout.ImageIndex = 10;
			var tbbSettings = new ToolBarButton();
            tbbSettings.ImageIndex = 11;

			this.tbIconBar.ButtonClick += (object o, ToolBarButtonClickEventArgs e) => {
				switch( this.tbIconBar.Buttons.IndexOf( e.Button ) ) {
					case 0: this.DoReset(); break;
					case 1: this.DoOpen(); break;
					case 2: this.DoSave(); break;
					case 3: this.DoDisplayInHex(); break;
					case 4: this.DoDisplayInDec(); break;
					case 5: this.DoIncreaseFont(); break;
					case 6: this.DoDecreaseFont(); break;
					case 7: this.DoSwitchToMemory(); break;
					case 8: this.DoSwitchToDrawing(); break;
					case 9: this.ShowSettings(); break;
					case 10: this.DoHelp(); break;
					case 11: this.DoAbout(); break;
					default: throw new ArgumentException( "unexpected toolbar button: unhandled" );
				}
			}; 

			this.tbIconBar.Buttons.AddRange( new ToolBarButton[]{
				tbbReset, tbbOpen, tbbSave,
				tbbHex, tbbDec, tbbZoomIn,
				tbbZoomOut, tbbMem, tbbVisual,
				tbbSettings, tbbHelp, tbbAbout
			});

			this.Controls.Add( this.tbIconBar );
		}

		private void BuildTabbedPanel()
		{
			this.tcTabs = new TabControl();
            this.tcTabs.SuspendLayout();
            this.tcTabs.SelectedIndexChanged += delegate(object obj, EventArgs args) {
                if ( tcTabs.SelectedIndex == 1 ) {
                    this.DoDrawing();
                }
				this.FocusOnInput();
            };
            this.tcTabs.Dock = DockStyle.Fill;
			this.tcTabs.Alignment = TabAlignment.Bottom;
			this.tcTabs.TabPages.Add( new TabPage( "" ) );
			this.tcTabs.TabPages.Add( new TabPage( "" ) );
			this.tcTabs.ImageList = new ImageList();
			this.tcTabs.ImageList.ImageSize = new Size( 16, 16 );
			this.tcTabs.ImageList.Images.AddRange( new Image[] {
				this.memoryIconBmp, this.diagramIconBmp
			});
			this.tcTabs.TabPages[ 0 ].ImageIndex = 0;
			this.tcTabs.TabPages[ 1 ].ImageIndex = 1;
			this.splSymbolTable.Panel2.Controls.Add( this.tcTabs );
		}

		private void BuildOutput()
		{
			this.rtbOutput = new RichTextBox();
            this.rtbOutput.Font = this.baseFont;
            this.rtbOutput.ReadOnly = true;
            this.rtbOutput.Dock = DockStyle.Fill;
            this.pnlIO.Controls.Add( this.rtbOutput );
		}

		private void BuildStatusbar()
		{
			this.sbStatus = new StatusStrip( );
			this.lblStatus = new ToolStripStatusLabel( );
			this.sbStatus.Items.Add( this.lblStatus );
            this.Controls.Add( sbStatus );
		}

		private void BuildSettingsPanel()
		{
			this.pnlSettings = new TableLayoutPanel();
			this.pnlSettings.BackColor = Color.White;
            this.pnlSettings.Dock = DockStyle.Fill;
			this.pnlSettings.ColumnCount = 1;
			this.pnlSettings.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
			this.pnlSettings.SuspendLayout();

			// Button
			var btClose = new Button();
			btClose.BackColor = Color.White;
			btClose.Image = this.backIconBmp;
			btClose.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			btClose.Font = new Font( btClose.Font, FontStyle.Bold );
			btClose.FlatStyle = FlatStyle.Flat;
			btClose.FlatAppearance.BorderSize = 0;
			btClose.Click += (sender, e) => this.ChangeSettings();
			this.pnlSettings.Controls.Add( btClose );

			/*
			var pnlChks = new TableLayoutPanel();
			pnlChks.Dock = DockStyle.Top;
			pnlChks.SuspendLayout();
			this.pnlSettings.Controls.Add( pnlChks );

			// Checkbox about showing the menu
			this.chkShowMainMenu = new CheckBox();
			this.chkShowMainMenu.Dock = DockStyle.Top;
			this.chkShowMainMenu.Text = StringsL18n.Get( StringsL18n.StringId.CkShowMenu );
			this.chkShowMainMenu.Checked = this.IsMainMenuShown;
			pnlChks.Controls.Add( this.chkShowMainMenu );
			*/

			// Locale
			var pnlLocales = new Panel();
			pnlLocales.Dock = DockStyle.Top;
			this.lblLocales = new Label();
			this.lblLocales.Text = L18n.Get( L18n.Id.LblLanguage );;
			this.lblLocales.Dock = DockStyle.Left;

			this.cbLocales = new ComboBox();
			this.cbLocales.Dock = DockStyle.Fill;
			this.cbLocales.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbLocales.Text = Core.Locale.CurrentLocale.ToString();

			CultureInfo[] locales = CultureInfo.GetCultures( CultureTypes.SpecificCultures );
			Array.Sort( locales,
				((CultureInfo x, CultureInfo y) => x.ToString().CompareTo( y.ToString() ) )
			);

			this.cbLocales.Items.Add( "<local>" );
			foreach(CultureInfo locale in locales ) {
				this.cbLocales.Items.Add( locale.NativeName + ": " + locale.ToString() );
			}

			pnlLocales.Controls.Add( this.cbLocales );
			pnlLocales.Controls.Add( this.lblLocales );
			this.pnlSettings.Controls.Add( pnlLocales );

			// Finishing
			// pnlChks.ResumeLayout( false );
			this.pnlSettings.ResumeLayout( false );
			this.pnlSettings.Hide();
            this.Controls.Add( this.pnlSettings );

			// Sizes
			this.pnlSettings.MinimumSize = new Size(
				this.ClientSize.Width,
				SystemInformation.VirtualScreen.Height
			);
			return;
		}

        private void Build()
        {
            // Start
			this.BuildIcons();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.SuspendLayout();

            // Main panels
            this.pnlIO = new Panel();
            this.pnlIO.Dock = DockStyle.Bottom;
            this.pnlMain = new Panel();
            this.pnlMain.Dock = DockStyle.Fill;
            this.pnlIO.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.Controls.Add( pnlMain );
            this.Controls.Add( pnlIO );

            // Split panel for symbol explorer
			// 1 - symbol table, 2 - tabs
            this.splSymbolTable = new SplitContainer();
            this.splSymbolTable.SuspendLayout();
            this.splSymbolTable.Dock = DockStyle.Fill;

            // Split panel for history
			// 1 - splSymbolTable (everything else), 2 - history
            this.splHistory = new SplitContainer();
            this.splHistory.Dock = DockStyle.Fill;
            this.splHistory.SuspendLayout();
            this.splHistory.Panel1.Controls.Add( this.splSymbolTable );

            // Compose it up
            this.pnlMain.Controls.Add( splHistory );

			this.BuildTabbedPanel();
            this.BuildHistory();
            this.BuildMemoryView();
            this.BuildSymbolTable();
			this.BuildDrawingTab();
			this.BuildToolbar();
			this.BuildOutput();
            this.BuildInput();
			this.BuildSettingsPanel();
			this.BuildAboutPanel();
			this.BuildStatusbar();

            // End
			this.Closed += (sender, e) => this.DoQuit();
			this.Text = AppInfo.Name;
            this.MinimumSize = new Size( 800, 600 );
			this.Shown += (o, e) => this.FocusOnInput();
            this.splHistory.ResumeLayout( false );
            this.splSymbolTable.ResumeLayout( false );
            this.tcTabs.ResumeLayout( false );
            this.pnlIO.ResumeLayout( false );
            this.pnlMain.ResumeLayout( false );
            this.ResumeLayout( false );
            this.UpdateFont( 0 );
			this.FocusOnInput();
            
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
							ArchManager.Execute( this.machine, line );
							this.AddToHistory( line );
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
				File.WriteAllText( dlg.FileName, this.lbHistory.Text );
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
            this.lbHistory.Text = "";
            this.machine.Memory.Reset( rt );
            this.machine.Reset();
            this.UpdateView();
        }

		private void FocusOnInput()
		{
			this.edInput.Focus();
		}

		private void UpdateView(Variable[] vars)
		{
			this.UpdateMemoryView( vars );
            this.UpdateSymbolTable();
			this.DoDrawing();

			this.FocusOnInput();
		}

        private void UpdateView()
        {
			this.UpdateView( new Variable[ 0 ] );
        }

        private void UpdateSymbolTable()
        {
            ReadOnlyCollection<Variable> variables = this.machine.TDS.Variables;

			this.tvSymbolTable.Nodes.Clear();

            foreach(Variable vble in variables) {
                string varTypeAddr = vble.Type.Name + " :" + vble.Type.Size
                    + " [" + Literal.ToPrettyNumber( vble.Address ) + ']';

                var vbleNode = new TreeNode( vble.Name.Value );
                var typeNode = new TreeNode( varTypeAddr );
                var contentsNode = new TreeNode( " = " + vble.LiteralValue );

                vbleNode.Nodes.Add( typeNode );
                vbleNode.Nodes.Add( contentsNode );
                this.tvSymbolTable.Nodes.Add( vbleNode );
            }

            this.tvSymbolTable.ExpandAll();
            return;
        }

        private void UpdateMemoryView(Variable[] vars)
		{
            var memory = this.machine.Memory.Raw;

			if ( vars.Length == 0 ) {
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
			} else {
				foreach(Variable vble in vars) {
					if ( !( vble is TempVariable ) ) {
						for(int i = 0; i < vble.Size; ++i) {
							int pos = vble.Address + i;
							int row = pos / MaxDataColumns;
		                	int col = ( pos % MaxDataColumns ) + 1;

							this.grdMemory.Rows[ row ].Cells[ col ].Value = Literal.ToHex( memory[ pos ] );
							this.grdMemory.Rows[ row ].Cells[ col ].ToolTipText = memory[ pos ].ToString();
						}
					}
				}
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

        private int charWidth = -1;
        private SplitContainer splSymbolTable;
        private SplitContainer splHistory;
        private Font baseFont;
        private Panel pnlAbout;
        private Panel pnlIO;
        private Panel pnlMain;
		private TableLayoutPanel pnlSettings;
        private TextBox edInput;
        private DataGridView grdMemory;
        private TreeView tvSymbolTable;
		private StatusStrip sbStatus;
		private ToolStripStatusLabel lblStatus;
        private ListBox lbHistory;
		private RichTextBox rtbOutput;
        private TabControl tcTabs;
        private PictureBox pbCanvas;
        private Bitmap bmDrawArea;
		private ToolBar tbIconBar;
		private ComboBox cbLocales;
		private Label lblLocales;

		private Bitmap appIconBmp;
		private Bitmap backIconBmp;
		private Bitmap openIconBmp;
		private Bitmap saveIconBmp;
		private Bitmap resetIconBmp;
		private Bitmap zoomInIconBmp;
		private Bitmap zoomOutIconBmp;
		private Bitmap helpIconBmp;
		private Bitmap aboutIconBmp;
		private Bitmap memoryIconBmp;
		private Bitmap diagramIconBmp;
		private Bitmap hexIconBmp;
		private Bitmap decIconBmp;
		private Bitmap zeroIconBmp;
		private Bitmap randIconBmp;
		private Bitmap settingsIconBmp;

        private Machine machine;
		private string statusBeforeSettings;
        private SchemaDrawer sdDrawingBoard;
		public static string currentDir;
		private string cfgFile = "";
    }
}

