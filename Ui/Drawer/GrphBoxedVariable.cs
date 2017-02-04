
namespace CSim.Ui.Drawer {
	using System.Drawing;
	using System.Collections.Generic;

	using CSim.Core;
	using CSim.Core.Variables;

	/// <summary>
	/// Represents a variable drawn in the schema.
	/// </summary>
	public abstract class GrphBoxedVariable {
		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Ui.Drawer.GrphBoxedVariable"/> class.
		/// </summary>
		/// <param name="v">The variable to be drawn.</param>
		/// <param name="grf">The graphics settings.</param>
		protected GrphBoxedVariable(Variable v, GraphInfo grf)
		{
			this.vble = v;
			this.height = this.width = -1;
			this.BoxX = this.BoxY = 0;
			this.BoxWidth = this.BoxHeight = 0;
			this.GraphInfo = grf;
		}

		/// <summary>
		/// Gets or sets the value of the variable.
		/// </summary>
		/// <value>The value, as a string.</value>
		public string StrValue {
			get; protected set;
		}

		/// <summary>
		/// Gets or sets the type of the variable.
		/// </summary>
		/// <value>The type, as a string.</value>
		public string StrType {
			get; protected set;
		}

		/// <summary>
		/// Gets or sets the name of the variable.
		/// </summary>
		/// <value>The name, as a string.</value>
		public string StrName {
			get; protected set;
		}

		/// <summary>
		/// Gets or sets the x position of the box of the variable.
		/// </summary>
		/// <value>The x, as a float.</value>
		public float X {
			get; set;
		}

		/// <summary>
		/// Gets or sets the y position of the box of the variable.
		/// </summary>
		/// <value>The y, as a float.</value>
		public float Y {
			get; set;
		}

		/// <summary>
		/// Gets or sets the box's x coordinate.
		/// (Not of the whole drawing.)
		/// This is relative to the position.
		/// <seealso cref="X"/>
		/// <seealso cref="Y"/>
		/// </summary>
		/// <value>The specific x coordinate of the (inner) box.</value>
		public float BoxX {
			get; protected set;
		}

		/// <summary>
		/// Gets or sets the box's y coordinate.
		/// (Not of the whole drawing.)
		/// This is relative to the position.
		/// <seealso cref="X"/>
		/// <seealso cref="Y"/>
		/// </summary>
		/// <value>The specific y coordinate of the (inner) box.</value>
		public float BoxY {
			get; protected set;
		}

		/// <summary>
		/// Gets or sets the box's width.
		/// (Not of the whole drawing.)
		/// </summary>
		/// <value>The box x.</value>
		public float BoxWidth {
			get; protected set;
		}

		/// <summary>
		/// Gets or sets the box's height.
		/// (Not of the whole drawing.)
		/// </summary>
		/// <value>The box y.</value>
		public float BoxHeight {
			get; protected set;
		}

		/// <summary>
		/// Gets the width of the box of the variable.
		/// </summary>
		/// <value>The width, as a float.</value>
		public float Width {
			get {
				if ( this.width < 0 ) {
					this.CalculateSize();
				}

				return this.width;
			}
			protected set {
				this.width = value;
			}
		}

		/// <summary>
		/// Gets the height of the box of the variable.
		/// </summary>
		/// <value>The height, as a float.</value>
		public float Height {
			get {
				if ( this.height < 0 ) {
					this.CalculateSize();
				}

				return this.height;
			}
			protected set {
				this.height = value;
			}
		}

		/// <summary>
		/// Gets the variable.
		/// </summary>
		/// <value>The variable to be drawn.</value>
		public Variable Variable {
			get {
				return this.vble;
			}
			protected set {
				this.vble = value;
				this.CalculateSize();
			}
		}

		/// <summary>
		/// Gets the graphic settings.
		/// </summary>
		/// <value>The graph info, as a GraphInfo object.</value>
		public GraphInfo GraphInfo {
			get {
				return this.grphInfo;
			}
			protected set {
				this.grphInfo = value;
				this.CalculateSize();
			}
		}

		/// <summary>
		/// Calculates the size of the box, in order to draw it later.
		/// </summary>
		protected abstract void CalculateSize();

		/// <summary>
		/// Draws this instance.
		/// Should be called after <see cref="CalculateSize"/>.
		/// </summary>
		public virtual void Draw()
		{
			// Erase the area to draw over
			this.DrawFilledRectangle( this.GraphInfo.BackGroundColor, this.X, this.Y, this.Width, this.Height );
		}

		/// <summary>
		/// Gets the related boxes.
		/// This is specially intended for arrays' cells and so on.
		/// </summary>
		/// <returns>The related boxes.</returns>
		public virtual IDictionary<long, GrphBoxedVariable> GetInvolvedBoxes()
		{
			var toret = new Dictionary<long, GrphBoxedVariable>();

			toret.Add( this.Variable.Address, this );
			return toret;
		}

		/// <summary>
		/// Draws a rectangle.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		protected void DrawRectangle(float x, float y, float width, float height)
		{
			this.GraphInfo.Graphics.DrawRectangle(
										this.GraphInfo.Pen,
										new Rectangle( (int) x, (int) y, (int) width, (int) height )
			);
		}

		/// <summary>
		/// Draws a filled rectangle.
		/// </summary>
		/// <param name="c">The <see cref="System.Drawing.Color"/> to fill the rectangle with.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		protected void DrawFilledRectangle(Color c, float x, float y, float width, float height)
		{
			var pen = new Pen( new SolidBrush( c ) );

			this.GraphInfo.Graphics.FillRectangle(
				pen.Brush,
				new Rectangle( (int) x, (int) y, (int) width, (int) height )
			);
		}

		/// <summary>
		/// Draws a rectangle.
		/// </summary>
		/// <param name="x1">The x coordinate for the beginning point.</param>
		/// <param name="y1">The y coordinate for the beginning point.</param>
		/// <param name="x2">The x coordinate for the ending point.</param>
		/// <param name="y2">The x coordinate for the ending point.</param>
		protected void DrawLine(float x1, float y1, float x2, float y2)
		{
			this.GraphInfo.Graphics.DrawLine(
										this.GraphInfo.Pen,
										new PointF( x1, y1 ),
										new PointF( x2, y2 ) );
		}

		/// <summary>
		/// Draws the text.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="f">The font with which to draw the text.</param>
		/// <param name="s">S.</param>
		protected void DrawText(float x, float y, Font f, string s)
		{
			this.GraphInfo.Graphics.DrawString( s, f, this.GraphInfo.Pen.Brush, x, y, StringFormat.GenericTypographic );
		}

		/// <summary>
		/// Creates the appropriate GrphBoxedVariable instance.
		/// </summary>
		/// <param name="v">The variable to be drawn, as a Variable instance.</param>
		/// <param name="grf">All graphics sets, as a GraphInfo instance.</param>
		public static GrphBoxedVariable Create(Variable v, GraphInfo grf)
		{
			GrphBoxedVariable toret = null;
			var arrayVble = v as ArrayVariable;

			if ( arrayVble != null ) {
				toret = new GrphBoxedArray( arrayVble, grf );
			} else {
				toret = new GrphBoxedPrimitive( v, grf );
			}

			return toret;
		}

		private float height;
		private float width;
		private Variable vble;
		private GraphInfo grphInfo;
	}
}
