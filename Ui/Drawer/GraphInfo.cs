using System;
using System.Drawing;

namespace CSim.Ui.Drawer {
	/// <summary>
	/// Stores all info needed for drawing.
	/// </summary>
	public class GraphInfo {
		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Ui.Drawer.GraphInfo"/> class.
		/// </summary>
		/// <param name="grf">The graphics to draw on</param>
		/// <param name="pen">The pen to draw with</param>
		/// <param name="fSmall">The small font.</param>
		/// <param name="fNormal">The normal font.</param>
		/// <param name="hGap">The horizontal separation.</param>
		/// <param name="vGap">The vertical separation.</param>
		public GraphInfo(Graphics grf, Pen pen, Font fSmall, Font fNormal, int hGap, int vGap)
		{
			this.Graphics = grf;
			this.NormalFont = new FontInfo( fNormal, grf );
			this.SmallFont = new FontInfo( fSmall, grf );
			this.Pen = pen;
			this.HGap = hGap;
			this.VGap = vGap;
		}

		/// <summary>
		/// Gets or sets the normal font.
		/// </summary>
		/// <value>The normal font, as a FontInfo instance.</value>
		public FontInfo NormalFont {
			get; set;
		}

		/// <summary>
		/// Gets or sets the small font.
		/// </summary>
		/// <value>The small font, as a FontInfo instance.</value>
		public FontInfo SmallFont {
			get; set;
		}

		/// <summary>
		/// Gets or sets the graphics.
		/// </summary>
		/// <value>The graphics, as a Graphics instance.</value>
		public Graphics Graphics {
			get {
				return this.grf;
			}
			set {
				this.grf = value;
				this.Update();
			}
		}

		private void Update()
		{
			if ( this.Pen == null ) {
				this.Pen = new Pen( Brushes.Navy );
			}

			if ( this.NormalFont == null ) {
				var font = new Font( FontFamily.GenericMonospace, 12 );
				this.NormalFont = new FontInfo( font, this.Graphics );
			} else {
				this.NormalFont.Graphics = this.Graphics;
			}

			if ( this.SmallFont == null ) {
				var font = new Font( FontFamily.GenericMonospace, 10 );
				this.SmallFont = new FontInfo( font, this.Graphics );
			} else {
				this.SmallFont.Graphics = this.Graphics;
			}

			return;
		}

		/// <summary>
		/// Gets or sets the pen for drawing.
		/// </summary>
		/// <value>The pen, as a Pen instance.</value>
		public Pen Pen {
			get; set;
		}

		/// <summary>
		/// Gets or sets the horizontal separation.
		/// </summary>
		/// <value>The horizontal gap.</value>
		public int HGap {
			get; set;
		}

		/// <summary>
		/// Gets or sets the vertical separation.
		/// </summary>
		/// <value>The vertical gap.</value>
		public int VGap {
			get; set;
		}

		private Graphics grf;
	}
}
