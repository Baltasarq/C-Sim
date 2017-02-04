
namespace CSim.Ui {
	using System;
	using System.Globalization;
	using System.Drawing;
	using System.Windows.Forms;

	using CSim.Core;

	/// <summary>The main window for the application. This builds the view.</summary>
	public partial class MainWindow {

		private void BuildInputPanel()
		{
			// Line input
			this.edInput = new ComboBox();
			this.edInput.Dock = DockStyle.Bottom;
			this.edInput.Font = this.baseFont;
			this.edInput.KeyDown += (o, e) => {
				if ( e.KeyCode == Keys.Enter ) {
					this.DoInput();
					e.Handled = true;
					e.SuppressKeyPress = true;
				}
			};

			this.pnlIO.Controls.Add( this.edInput );
		}

		private void BuildWatchesView()
		{
			this.lblWatchesValue = new Label[ NumWatches ];
			this.edWatchesLabel = new TextBox[ NumWatches ];

			for(int i = 0; i < NumWatches; ++i) {
				var lblWatch = this.lblWatchesValue[ i ] = new Label { Text = "0", Dock = DockStyle.Right };
				var edWatch = this.edWatchesLabel[ i ] = new TextBox { Dock = DockStyle.Fill };

				lblWatch.Font = new Font( FontFamily.GenericMonospace, 11 );
					
				edWatch.LostFocus += (obj, args) => this.UpdateWatches();
				lblWatch.Font = new Font( FontFamily.GenericMonospace, 11 );
			}

			return;
		}

		private void BuildWatchesPanel()
		{
			this.BuildWatchesView();
			this.gbWatches = new GroupBox { Text = "Watches", Dock = DockStyle.Fill };
			this.pnlWatches = new TableLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true };

			for(int i = 0; i < NumWatches; ++i) {
				var panel = new Panel { Dock = DockStyle.Top };
				panel.Controls.Add( this.edWatchesLabel[ i ] );
				panel.Controls.Add( this.lblWatchesValue[ i ] );
				this.pnlWatches.Controls.Add( panel );
				panel.MaximumSize = new Size( int.MaxValue, this.edWatchesLabel[ i ].Height );
			}

			this.gbWatches.Controls.Add( this.pnlWatches );
			this.splWatches.Panel2.Controls.Add( this.gbWatches );
		}

		private void BuildHistoryPanel()
		{
			this.lbHistory = new ListBox();
			this.lbHistory.Font = new Font( FontFamily.GenericMonospace, 8 );
			this.lbHistory.Dock = DockStyle.Fill;
			this.gbHistory = new GroupBox { Text = "History", Dock = DockStyle.Fill };
			this.gbHistory.Controls.Add( this.lbHistory );
			this.splWatches.Panel1.Controls.Add( this.gbHistory );

			this.lbHistory.SelectedIndexChanged += (o, e) => this.OnHistoryIndexChanged();
		}

		private void BuildMemoryViewPanel()
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
					columns[ i ].HeaderText = FromIntToHex( i - 1 );
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
			this.gbSymbols = new GroupBox { Text = "Symbols", Dock = DockStyle.Fill };
			this.tvSymbolTable = new TreeView();
			this.tvSymbolTable.Dock = DockStyle.Fill;
			this.tvSymbolTable.Font = this.baseFont;
			this.tvSymbolTable.PathSeparator = ".";
			this.tvSymbolTable.AfterSelect += (sender, e) => this.DoTreeSelect();
			this.gbSymbols.Controls.Add( this.tvSymbolTable );
			this.splSymbolTable.Panel1.Controls.Add( this.gbSymbols );
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

				this.playIconBmp = new Bitmap( System.Reflection.Assembly.
					GetEntryAssembly().
					GetManifestResourceStream( "CSim.Res.play.png" ) );

				this.stopIconBmp = new Bitmap( System.Reflection.Assembly.
					GetEntryAssembly().
					GetManifestResourceStream( "CSim.Res.stop.png" ) );

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
			this.sdDrawingBoard = new SchemaDrawer( this.machine );
		}

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
				this.aboutIconBmp, this.settingsIconBmp,
				this.playIconBmp, this.stopIconBmp
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
			mniZero.DrawItem += (o, e) => {
				double factor = (double) e.Bounds.Height / this.zeroIconBmp.Height;
				var rect = new Rectangle( e.Bounds.X, e.Bounds.Y,
					(int) ( this.zeroIconBmp.Width * factor ),
					(int) ( this.zeroIconBmp.Height * factor ) );
				e.Graphics.DrawImage( this.zeroIconBmp, rect );
			};

			var mniRandom = new MenuItem( "Random", (o, e) => DoReset( MemoryManager.ResetType.Random ) );
			mniRandom.OwnerDraw = true;
			mniRandom.DrawItem += (o, e) => {
				double factor = (double) ( e.Bounds.Height ) / this.randIconBmp.Height;
				var rect = new Rectangle( e.Bounds.X, e.Bounds.Y,
					(int) ( this.randIconBmp.Width * factor ),
					(int) ( this.randIconBmp.Height * factor ) );
				e.Graphics.DrawImage( this.randIconBmp, rect );
			};

			tbbReset.DropDownMenu = new ContextMenu( new MenuItem[]{
				mniZero, mniRandom
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
			var tbbPlay = new ToolBarButton();
			tbbPlay.ImageIndex = 12;
			this.tbbStop = new ToolBarButton();
			tbbStop.ImageIndex = 13;

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
				case 12: this.DoPlay(); break;
				case 13: this.DoStop(); break;
				default: throw new ArgumentException( "unexpected toolbar button: unhandled" );
				}
			}; 

			this.tbIconBar.Buttons.AddRange( new ToolBarButton[]{
				tbbReset, tbbOpen, tbbSave,
				tbbHex, tbbDec, tbbZoomIn,
				tbbZoomOut, tbbMem, tbbVisual,
				tbbSettings, tbbHelp, tbbAbout,
				tbbPlay, tbbStop
			});

			this.tbbStop.Enabled = false;
			this.Controls.Add( this.tbIconBar );
		}

		private void BuildTabbedPanel()
		{
			this.gbMain = new GroupBox { Text = "Memory", Dock = DockStyle.Fill };
			this.tcTabs = new TabControl();
			this.tcTabs.SuspendLayout();
			this.tcTabs.SelectedIndexChanged += (o, e) => {
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
			this.gbMain.Controls.Add( this.tcTabs );
			this.splSymbolTable.Panel2.Controls.Add( this.gbMain );
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

			// Locale
			var pnlLocales = new Panel();
			pnlLocales.Dock = DockStyle.Top;
			this.lblLocales = new Label();
			this.lblLocales.Text = L18n.Get( L18n.Id.LblLanguage );
			this.lblLocales.Dock = DockStyle.Left;

			this.cbLocales = new ComboBox();
			this.cbLocales.Dock = DockStyle.Fill;
			this.cbLocales.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbLocales.Text = Core.Locale.CurrentLocale.ToString();

			CultureInfo[] locales = CultureInfo.GetCultures( CultureTypes.SpecificCultures );
			Array.Sort( locales,
			           (CultureInfo x, CultureInfo y) => string.Compare( x.ToString(), y.ToString() )
			);

			this.cbLocales.Items.Add( "<local>" );
			foreach(CultureInfo locale in locales ) {
				this.cbLocales.Items.Add( locale.NativeName + ": " + locale );
			}

			pnlLocales.Controls.Add( this.cbLocales );
			pnlLocales.Controls.Add( this.lblLocales );
			this.pnlSettings.Controls.Add( pnlLocales );

			// Endianness
			var gbEndianness = new GroupBox();
			gbEndianness.SuspendLayout();
			gbEndianness.Text = "Endianness";
			gbEndianness.Dock = DockStyle.Top;
			var pnlEndianness = new TableLayoutPanel();
			pnlEndianness.SuspendLayout();
			pnlEndianness.Dock = DockStyle.Fill;
			this.rbEndianness1 = new RadioButton { Text = "Little endian", Dock = DockStyle.Top };
			this.rbEndianness2 = new RadioButton { Text = "Big endian", Dock = DockStyle.Top };
			pnlEndianness.Controls.Add( this.rbEndianness1 );
			pnlEndianness.Controls.Add( this.rbEndianness2 );
			gbEndianness.Controls.Add( pnlEndianness );
			pnlEndianness.ResumeLayout( false );
			gbEndianness.ResumeLayout( false );
			this.pnlSettings.Controls.Add( gbEndianness );


			// Memory width
			var gbWordSize = new GroupBox();
			gbWordSize.SuspendLayout();
			gbWordSize.Text = "Word size";
			gbWordSize.Dock = DockStyle.Top;
			var pnlWordSize = new TableLayoutPanel();
			pnlWordSize.SuspendLayout();
			pnlWordSize.Dock = DockStyle.Fill;
			this.rbWS16 = new RadioButton { Text = "16 bits", Dock = DockStyle.Top };
			this.rbWS32 = new RadioButton { Text = "32 bits", Dock = DockStyle.Top };
			this.rbWS64 = new RadioButton { Text = "64 bits", Dock = DockStyle.Top };
			pnlWordSize.Controls.Add( this.rbWS16 );
			pnlWordSize.Controls.Add( this.rbWS32 );
			pnlWordSize.Controls.Add( this.rbWS64 );
			gbWordSize.Controls.Add( pnlWordSize );
			pnlWordSize.ResumeLayout( false );
			gbWordSize.ResumeLayout( false );
			this.pnlSettings.Controls.Add( gbWordSize );

			// Finishing
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

		private void SetStartingSize()
		{
			Rectangle size = Screen.FromControl( this ).Bounds;

			if ( size.Width > 1024
		  	  && size.Height > 768 )
			{
				this.MinimumSize = new Size( 800, 600 );

				this.Width = 1024;
				this.Height = 768;
			}
			else
			if ( size.Width > 800
			  && size.Height > 600 )
			{
				this.MinimumSize = new Size( 640, 480 );

				this.Width = 800;
				this.Height = 600;
			}
			else {
				this.MinimumSize = new Size( 480, 420 );
				this.Width = 640;
				this.Height = 480;
			}

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
			this.splHistory = new SplitContainer{ Dock = DockStyle.Fill };
			this.splHistory.SuspendLayout();
			this.splHistory.Panel1.Controls.Add( this.splSymbolTable );

			// Split panel for history & watches
			this.splWatches = new SplitContainer{ Dock = DockStyle.Fill, Orientation = Orientation.Horizontal };
			this.splWatches.SuspendLayout();
			this.splHistory.Panel2.Controls.Add( this.splWatches );

			// Compose it up
			this.pnlMain.Controls.Add( splHistory );

			this.BuildTabbedPanel();
			this.BuildWatchesPanel();
			this.BuildHistoryPanel();
			this.BuildMemoryViewPanel();
			this.BuildSymbolTable();
			this.BuildDrawingTab();
			this.BuildToolbar();
			this.BuildOutput();
			this.BuildInputPanel();
			this.BuildSettingsPanel();
			this.BuildAboutPanel();
			this.BuildStatusbar();

			// End
			this.Closed += (sender, e) => this.DoQuit();
			this.Text = AppInfo.Name;
			this.SetStartingSize();
			this.Shown += (o, e) => this.FocusOnInput();
			this.splHistory.ResumeLayout( false );
			this.splSymbolTable.ResumeLayout( false );
			this.splWatches.ResumeLayout( false );
			this.tcTabs.ResumeLayout( false );
			this.pnlIO.ResumeLayout( false );
			this.pnlMain.ResumeLayout( false );
			this.ResumeLayout( false );
			this.UpdateFont( 0 );
			this.FocusOnInput();

			return;
		}


		private SplitContainer splSymbolTable;
		private SplitContainer splHistory;
		private SplitContainer splWatches;
		private Font baseFont;
		private Panel pnlAbout;
		private Panel pnlIO;
		private Panel pnlMain;
		private GroupBox gbSymbols;
		private GroupBox gbHistory;
		private GroupBox gbMain;
		private GroupBox gbWatches;
		private TableLayoutPanel pnlSettings;
		private TableLayoutPanel pnlWatches;
		private ComboBox edInput;
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
		private ToolBarButton tbbStop;
		private ComboBox cbLocales;
		private Label lblLocales;
		private RadioButton rbEndianness1;
		private RadioButton rbEndianness2;
		private RadioButton rbWS16;
		private RadioButton rbWS32;
		private RadioButton rbWS64;
		private TextBox[] edWatchesLabel;
		private Label[] lblWatchesValue;

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
		private Bitmap playIconBmp;
		private Bitmap stopIconBmp;
	}
}

