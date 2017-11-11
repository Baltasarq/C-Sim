// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Ui {
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Forms;

    using Core;

    /// <summary>The main window for the application. This builds the view.</summary>
    public partial class MainWindow {
        private void BuildInputPanel()
        {
            // Line input
            this.edInput = new ComboBox() {
                Dock = DockStyle.Bottom,
                Font = this.baseFont
            };

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
                var lblWatch = this.lblWatchesValue[ i ] = new Label {
                    Text = DefaultWatchText,
                    Dock = DockStyle.Right,
                    ForeColor = SystemColors.InfoText,
                    BackColor = SystemColors.Info
                };
                var edWatch = this.edWatchesLabel[ i ] = new TextBox {
                    Dock = DockStyle.Fill
                };

                lblWatch.Font = new Font( FontFamily.GenericMonospace, 11 );
                    
                edWatch.LostFocus += (obj, args) => this.UpdateWatches();
                edWatch.KeyUp += (object sender, KeyEventArgs e) => {
                    if ( e.KeyCode == Keys.Enter ) {
                        this.UpdateWatches();
                    }
                };
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
            this.lbHistory = new ListBox() {
                Font = new Font( FontFamily.GenericMonospace, 8 ),
                Dock = DockStyle.Fill,
                HorizontalScrollbar = true
            };

            this.gbHistory = new GroupBox { Text = "History", Dock = DockStyle.Fill };
            this.gbHistory.Controls.Add( this.lbHistory );
            this.splWatches.Panel1.Controls.Add( this.gbHistory );

            this.lbHistory.SelectedIndexChanged += (o, e) => this.OnHistoryIndexChanged();
        }

        private void BuildMemoryViewPanel()
        {
            // Grid for memory
            int columnWidth = ( this.CharWidth * 2 );
            this.grdMemory = new DataGridView() {
                AllowUserToResizeRows = false,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                AutoGenerateColumns = false,
                MultiSelect = false,
                ShowCellToolTips = true
            };

            // Style for regular columns
            var style = new DataGridViewCellStyle() {
                Font = new Font( this.baseFont, FontStyle.Bold ),
                BackColor = Color.AntiqueWhite,
                ForeColor = Color.Black,
                SelectionBackColor = Color.Black,
                SelectionForeColor = Color.AntiqueWhite
            };

            this.grdMemory.DefaultCellStyle = style;

            // Style for headers
            var styleHeaders = new DataGridViewCellStyle() {
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font( this.baseFont, FontStyle.Bold )
            };

            this.grdMemory.ColumnHeadersDefaultCellStyle = styleHeaders;

            // Style for first column
            var styleFirstColumn = new DataGridViewCellStyle() {
                Font = new Font( this.baseFont, FontStyle.Bold ),
                BackColor = Color.Black,
                ForeColor = Color.White
            };

            // Create columns
            DataGridViewTextBoxColumn[] columns =
                new DataGridViewTextBoxColumn[ MaxDataColumns +1 ];

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
            var rows = new DataGridViewRow[ (int) this.machine.Memory.Max / MaxDataColumns ];

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
            this.gbSymbols = new GroupBox {
                Dock = DockStyle.Fill,
                Text = "Symbols"
            };

            this.tvSymbolTable = new TreeView() {
                Dock = DockStyle.Fill,
                Font = this.baseFont,
                PathSeparator = "."
            };

            this.tvSymbolTable.AfterSelect += (sender, e) => this.DoTreeSelect();
            this.gbSymbols.Controls.Add( this.tvSymbolTable );
            this.splSymbolTable.Panel1.Controls.Add( this.gbSymbols );
            this.splSymbolTable.SplitterDistance = this.CharWidth;
        }

        private void BuildAboutPanel()
        {
            // Panel for about info
            this.pnlAbout = new Panel() {
                Dock = DockStyle.Bottom,
                BackColor = SystemColors.Info
            };
            this.pnlAbout.SuspendLayout();
            var lblAbout = new Label() {
                Text = AppInfo.Name
                        + " v" + AppInfo.Version + ", " + AppInfo.Author,
                Dock = DockStyle.Left,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = true,
                ForeColor = SystemColors.InfoText
            };
            var font = new Font( lblAbout.Font, FontStyle.Bold );
            font = new Font( font.FontFamily, 14 );
            lblAbout.Font = font;
            
            var btCloseAboutPanel = new Button() {
                    Text = "X",
                    Dock = DockStyle.Right,
                    Width = this.CharWidth * 5,
                    FlatStyle = FlatStyle.Flat,
            };
            btCloseAboutPanel.FlatAppearance.BorderSize = 0;
            btCloseAboutPanel.Font = new Font( btCloseAboutPanel.Font, FontStyle.Bold );
            btCloseAboutPanel.ForeColor = SystemColors.InfoText;
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
            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            var resourceAppIcon = assembly.
                GetManifestResourceStream( "CSim.Res.appIcon.png" );

            // Prepare icons
            if ( resourceAppIcon != null ) {
                this.appIconBmp = new Bitmap( resourceAppIcon );
                this.Icon = Icon.FromHandle( this.appIconBmp.GetHicon() );

                this.backIconBmp = new Bitmap(
                    assembly.GetManifestResourceStream( "CSim.Res.back.png" ) );

                this.playIconBmp = new Bitmap(
                    assembly.GetManifestResourceStream( "CSim.Res.play.png" ) );

                this.stopIconBmp = new Bitmap(
                    assembly.GetManifestResourceStream( "CSim.Res.stop.png" ) );

                this.openIconBmp = new Bitmap(
                    assembly.GetManifestResourceStream( "CSim.Res.open.png" ) );

                this.saveIconBmp = new Bitmap(
                    assembly.GetManifestResourceStream( "CSim.Res.save.png" ) );

                this.resetIconBmp = new Bitmap(
                    assembly.GetManifestResourceStream( "CSim.Res.reset.png" ) );

                this.zoomInIconBmp = new Bitmap(
                    assembly.GetManifestResourceStream( "CSim.Res.zoom_in.png" ) );

                this.zoomOutIconBmp = new Bitmap(
                    assembly.GetManifestResourceStream( "CSim.Res.zoom_out.png" ) );

                this.aboutIconBmp = new Bitmap(
                    assembly.GetManifestResourceStream( "CSim.Res.info.png" ) );

                this.helpIconBmp = new Bitmap(
                    assembly.GetManifestResourceStream( "CSim.Res.help.png" ) );

                this.memoryIconBmp = new Bitmap(
                    assembly.GetManifestResourceStream( "CSim.Res.memory.png" ) );

                this.diagramIconBmp = new Bitmap(
                    assembly.GetManifestResourceStream( "CSim.Res.diagram.png" ) );

                this.hexIconBmp = new Bitmap(
                    assembly.GetManifestResourceStream( "CSim.Res.hex.png" ) );

                this.decIconBmp = new Bitmap(
                    assembly.GetManifestResourceStream( "CSim.Res.dec.png" ) );

                this.zeroIconBmp = new Bitmap(
                    assembly.GetManifestResourceStream( "CSim.Res.zero.png" ) );

                this.randIconBmp = new Bitmap(
                    assembly.GetManifestResourceStream( "CSim.Res.random.png" ) );

                this.settingsIconBmp = new Bitmap(
                    assembly.GetManifestResourceStream( "CSim.Res.settings.png" ) );
            }

            return;
        }

        private void BuildDrawingTab()
        {
            var scroll = new Panel() {
                Dock = DockStyle.Fill, AutoScroll = true
            };

            this.pbCanvas = new PictureBox() {
                BackColor = Color.GhostWhite,
                ForeColor = Color.Navy,
                Anchor = AnchorStyles.Left | AnchorStyles.Top,
                SizeMode = PictureBoxSizeMode.AutoSize
            };

            scroll.Controls.Add( this.pbCanvas );
            this.tcTabs.TabPages[ 1 ].Controls.Add( scroll );
            this.sdDrawingBoard = new SchemaDrawer( this.machine );
        }

        private void BuildToolbar()
        {
            // Image list
            var imgList = new ImageList() { ImageSize = new Size( 24, 24 ) };
            imgList.Images.AddRange( new []{
                this.resetIconBmp, this.openIconBmp,
                this.saveIconBmp, this.hexIconBmp,
                this.decIconBmp, this.zoomInIconBmp,
                this.zoomOutIconBmp, this.diagramIconBmp,
                this.memoryIconBmp, this.helpIconBmp,
                this.aboutIconBmp, this.settingsIconBmp,
                this.playIconBmp, this.stopIconBmp
            } );

            // Toolbar
            this.tbIconBar = new ToolBar() {
                AutoSize = true,
                Dock = DockStyle.Top,
                Appearance = ToolBarAppearance.Flat,
                BorderStyle = BorderStyle.None,
                ImageList = imgList,
                ShowToolTips = true,
            };

            // Toolbar buttons
            this.tbbOpen = new ToolBarButton()      { ImageIndex = 1 };
            this.tbbSave = new ToolBarButton()      { ImageIndex = 2 };
            this.tbbHex = new ToolBarButton()       { ImageIndex = 3 };
            this.tbbDec = new ToolBarButton()       { ImageIndex = 4 };
            this.tbbZoomIn = new ToolBarButton()    { ImageIndex = 5 };
            this.tbbZoomOut = new ToolBarButton()   { ImageIndex = 6 };
            this.tbbDiagram = new ToolBarButton()   { ImageIndex = 7 };
            this.tbbMemory = new ToolBarButton()    { ImageIndex = 8 };
            this.tbbPlay = new ToolBarButton()      { ImageIndex = 12 };
            this.tbbStop = new ToolBarButton() {
                ImageIndex = 13,
                Enabled = false,
            };
            this.tbbHelp = new ToolBarButton()      { ImageIndex = 9 };
            this.tbbAbout = new ToolBarButton()     { ImageIndex = 10 };
            this.tbbSettings = new ToolBarButton()  { ImageIndex = 11 };
            this.tbbReset = new ToolBarButton() {
                ImageIndex = 0,
                Style = ToolBarButtonStyle.DropDownButton
            };

            var mniZero = new MenuItem( "Zero", (o, e) => DoReset() ) {
                OwnerDraw = true
            };

            mniZero.DrawItem += (o, e) => {
                double factor = (double) e.Bounds.Height / this.zeroIconBmp.Height;
                var rect = new Rectangle( e.Bounds.X, e.Bounds.Y,
                    (int) ( this.zeroIconBmp.Width * factor ),
                    (int) ( this.zeroIconBmp.Height * factor ) );
                e.Graphics.DrawImage( this.zeroIconBmp, rect );
            };

            var mniRandom = new MenuItem( "Random", (o, e) => DoReset( MemoryManager.ResetType.Random ) ) {
                OwnerDraw = true
            };

            mniRandom.DrawItem += (o, e) => {
                double factor = (double) ( e.Bounds.Height ) / this.randIconBmp.Height;
                var rect = new Rectangle( e.Bounds.X, e.Bounds.Y,
                    (int) ( this.randIconBmp.Width * factor ),
                    (int) ( this.randIconBmp.Height * factor ) );
                e.Graphics.DrawImage( this.randIconBmp, rect );
            };

            tbbReset.DropDownMenu = new ContextMenu( new []{ mniZero, mniRandom } );

            // Action for each button
            this.tbIconBar.ButtonClick += (object o, ToolBarButtonClickEventArgs e) => {
                if ( e.Button == this.tbbReset ) {
                    this.DoReset();
                }
                else
                if ( e.Button == this.tbbOpen ) {
                    this.DoOpen();
                }
                else
                if ( e.Button == this.tbbSave ) {
                    this.DoSave();
                }
                else
                if ( e.Button == this.tbbHex ) {
                    this.DoDisplayInHex();
                }
                else
                if ( e.Button == this.tbbDec ) {
                    this.DoDisplayInDec();
                }
                else
                if ( e.Button == this.tbbZoomIn ) {
                    this.DoIncreaseFont();
                }
                else
                if ( e.Button == this.tbbZoomOut ) {
                    this.DoDecreaseFont();
                }
                else
                if ( e.Button == this.tbbDiagram ) {
                    this.DoSwitchToDrawing();
                }
                else
                if ( e.Button == this.tbbMemory ) {
                    this.DoSwitchToMemory();
                }
                else
                if ( e.Button == this.tbbPlay ) {
                    this.DoPlay();
                }
                else
                if ( e.Button == this.tbbStop ) {
                    this.DoStop();
                }
                else
                if ( e.Button == this.tbbHelp ) {
                    this.DoHelp();
                }
                else
                if ( e.Button == this.tbbAbout ) {
                    this.DoAbout();
                }
                else
                if ( e.Button == this.tbbSettings ) {
                    this.ShowSettings();
                }
                else {
                    throw new ArgumentException( "unexpected toolbar button: unhandled" );
                }
            };
            
            this.tbIconBar.Buttons.AddRange( new[]{
                tbbSettings,
                new ToolBarButton() { Style = ToolBarButtonStyle.Separator },
                tbbReset,
                new ToolBarButton() { Style = ToolBarButtonStyle.Separator },
                tbbOpen, tbbSave,
                new ToolBarButton() { Style = ToolBarButtonStyle.Separator },
                tbbHex, tbbDec,
                new ToolBarButton() { Style = ToolBarButtonStyle.Separator },
                tbbZoomIn, tbbZoomOut,
                new ToolBarButton() { Style = ToolBarButtonStyle.Separator },
                tbbMemory, tbbDiagram,
                new ToolBarButton() { Style = ToolBarButtonStyle.Separator },
                tbbPlay, tbbStop,
                new ToolBarButton() { Style = ToolBarButtonStyle.Separator },
                tbbHelp, tbbAbout
            });

            this.Controls.Add( this.tbIconBar );
        }

        private void BuildTabbedPanel()
        {
            this.gbMain = new GroupBox { Text = "Memory", Dock = DockStyle.Fill };
            this.tcTabs = new TabControl() {
                Dock = DockStyle.Fill,
                Alignment = TabAlignment.Bottom,
                ImageList = new ImageList()
            };

            this.tcTabs.ImageList.ImageSize = new Size( 16, 16 );
            this.tcTabs.SuspendLayout();
            this.tcTabs.SelectedIndexChanged += (o, e) => {
                if ( tcTabs.SelectedIndex == 1 ) {
                    this.DoDrawing();
                }
                this.FocusOnInput();
            };
            this.tcTabs.TabPages.Add( new TabPage( "" ) );
            this.tcTabs.TabPages.Add( new TabPage( "" ) );
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
            this.rtbOutput = new RichTextBox() {
                Font = this.baseFont,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                BackColor = Color.Black
            };
            this.rtbOutput.PreviewKeyDown += (sender, e) => {
                this.edInput.Focus();
            };
            
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
            this.pnlSettings = new TableLayoutPanel {
                BackColor = SystemColors.Window,
                ForeColor = SystemColors.WindowText,
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                GrowStyle = TableLayoutPanelGrowStyle.AddRows
            };
            this.pnlSettings.SuspendLayout();

            // Button
            var btClose = new Button {
                BackColor = SystemColors.Info,
                ForeColor = SystemColors.InfoText,
                Image = this.backIconBmp,
                Anchor = AnchorStyles.Left | AnchorStyles.Top,
                Font = new Font( Font, FontStyle.Bold ),
                FlatStyle = FlatStyle.Flat
            };
            btClose.FlatAppearance.BorderSize = 0;
            btClose.Click += (sender, e) => this.ApplySettings();
            this.pnlSettings.Controls.Add( btClose );

            // Locale
            var pnlLocales = new Panel {
                Dock = DockStyle.Top,
            };
            this.lblLocales = new Label {
                Text = L18n.Get( L18n.Id.LblLanguage ),
                Dock = DockStyle.Left,
            };
            this.cbLocales = new ComboBox {
                ForeColor = SystemColors.Menu,
                BackColor = SystemColors.MenuText,
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Text = Core.Locale.CurrentLocale.ToString(),
            };

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
            var gbEndianness = new GroupBox { Text = "Endianness", Dock = DockStyle.Top };
            gbEndianness.SuspendLayout();
            var pnlEndianness = new TableLayoutPanel { Dock = DockStyle.Fill };
            pnlEndianness.SuspendLayout();
            this.rbEndianness1 = new RadioButton {
                Text = "Little endian",
                Dock = DockStyle.Top,
            };
            this.rbEndianness2 = new RadioButton {
                Text = "Big endian",
                Dock = DockStyle.Top,
            };
            pnlEndianness.Controls.Add( this.rbEndianness1 );
            pnlEndianness.Controls.Add( this.rbEndianness2 );
            gbEndianness.Controls.Add( pnlEndianness );
            pnlEndianness.ResumeLayout( false );
            gbEndianness.ResumeLayout( false );
            this.pnlSettings.Controls.Add( gbEndianness );

            // Alignment
            var gbAlignment = new GroupBox { Text = "Alignment", Dock = DockStyle.Top };
            var pnlAlignment = new TableLayoutPanel { Dock = DockStyle.Fill };
            gbAlignment.SuspendLayout();
            pnlAlignment.SuspendLayout();
            this.chkAlign = new CheckBox {
                Text = "Align variables in memory",
                Dock = DockStyle.Top,
            };
            pnlAlignment.Controls.Add( this.chkAlign );
            pnlAlignment.ResumeLayout( false );
            gbAlignment.ResumeLayout( false );
            gbAlignment.Controls.Add( pnlAlignment );
            this.pnlSettings.Controls.Add( gbAlignment );


            // Memory width
            var gbWordSize = new GroupBox { Text = "Word size", Dock = DockStyle.Top };
            var pnlWordSize = new TableLayoutPanel { Dock = DockStyle.Fill };
            gbWordSize.SuspendLayout();
            pnlWordSize.SuspendLayout();
            this.rbWS16 = new RadioButton {
                Text = "16 bits",
                Dock = DockStyle.Top,
            };
            this.rbWS32 = new RadioButton {
                Text = "32 bits",
                Dock = DockStyle.Top,
            };
            this.rbWS64 = new RadioButton {
                Text = "64 bits",
                Dock = DockStyle.Top,
            };
            pnlWordSize.Controls.Add( this.rbWS16 );
            pnlWordSize.Controls.Add( this.rbWS32 );
            pnlWordSize.Controls.Add( this.rbWS64 );
            gbWordSize.Controls.Add( pnlWordSize );
            pnlWordSize.ResumeLayout( false );
            gbWordSize.ResumeLayout( false );
            this.pnlSettings.Controls.Add( gbWordSize );

            // Finishing
            this.pnlSettings.PreviewKeyDown += ( o, evt ) => {
                this.ApplySettings();
            };
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
            this.splMain = new SplitContainer() {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal
            };
            this.pnlIO = new Panel()    { Dock = DockStyle.Fill };
            this.pnlMain = new Panel()  { Dock = DockStyle.Fill };
            this.pnlIO.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.Controls.Add( splMain );
            this.splMain.Panel1.Controls.Add( pnlMain );
            this.splMain.Panel2.Controls.Add( pnlIO );

            // Split panel for symbol explorer
            // 1 - symbol table, 2 - tabs
            this.splSymbolTable = new SplitContainer() { Dock = DockStyle.Fill };
            this.splSymbolTable.SuspendLayout();

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
            this.splMain.SplitterDistance =
                this.ClientSize.Height
                - this.tbIconBar.Height
                - this.sbStatus.Height
                - ( this.CharHeight * 5 );

            return;
        }


        private SplitContainer splMain;
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
        private ComboBox cbLocales;
        private Label lblLocales;
        private CheckBox chkAlign;
        private RadioButton rbEndianness1;
        private RadioButton rbEndianness2;
        private RadioButton rbWS16;
        private RadioButton rbWS32;
        private RadioButton rbWS64;
        private TextBox[] edWatchesLabel;
        private Label[] lblWatchesValue;

        private ToolBarButton tbbReset;
        private ToolBarButton tbbOpen;
        private ToolBarButton tbbSave;
        private ToolBarButton tbbHex;
        private ToolBarButton tbbDec;
        private ToolBarButton tbbZoomIn;
        private ToolBarButton tbbZoomOut;
        private ToolBarButton tbbMemory;
        private ToolBarButton tbbDiagram;
        private ToolBarButton tbbPlay;
        private ToolBarButton tbbStop;
        private ToolBarButton tbbHelp;
        private ToolBarButton tbbAbout;
        private ToolBarButton tbbSettings;

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

