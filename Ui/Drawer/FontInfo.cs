using System;
using System.Drawing;

namespace CSim.Ui.Drawer {
	/// <summary>
	/// Holds all the needed info about a font in order to calculate sizes.
	/// </summary>
	public class FontInfo {
		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Ui.Drawer.FontInfo"/> class.
		/// </summary>
		/// <param name="f">The font, as a Font instance.</param>
		/// <param name="grf">The graphics this font will be drawn on, in order to calculate measures</param>
		public FontInfo(Font f, Graphics grf)
		{
			this.Font = f;
			this.Graphics = grf;
		}

		/// <summary>
		/// Gets or sets the font.
		/// </summary>
		/// <value>The font, as a Font instance.</value>
		public Font Font {
			get {
				return this.font;
			}
			set {
				this.font = value;
				this.Update();
			}
		}

		/// <summary>
		/// Gets the width of a char.
		/// </summary>
		/// <value>The width of the char for this font and graphics.</value>
		public float CharWidth {
			get; private set;
		}

		/// <summary>
		/// Gets the height of a char.
		/// </summary>
		/// <value>The height of the char for this font and graphics.</value>
		public float CharHeight {
			get; private set;
		}

		/// <summary>
		/// Gets or sets the graphics the front will be drawn on.
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

	    /// <summary>
	    /// Calculates and updates the measures of a single char for this font.
	    /// </summary>
		private void Update()
		{
		    if ( this.Graphics != null ) {
		        SizeF fontSize = this.Graphics.MeasureString( "W", this.Font );
		        this.CharWidth = fontSize.Width + 1;
		        this.CharHeight = fontSize.Height + 1;
		    }

		    return;
		}

		private Font font;
		private Graphics grf;
	}
}
