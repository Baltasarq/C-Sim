namespace CSim.Ui.Drawer {
	using System;
	using System.Drawing;

	using CSim.Core;
	using CSim.Core.Literals;
	using CSim.Core.Variables;


	/// <summary>
	/// Represents a variable shown in the img window.
	/// </summary>
	public class GrphBoxedBasic: GrphBoxedVariable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Ui.Drawer.GrphBoxedBasic"/> class.
		/// </summary>
		/// <param name="v">The Variable, should be a Primitive.</param>
		/// <param name="grf">All graphics settings.</param>
		public GrphBoxedBasic(Variable v, GraphInfo grf)
			:base( v, grf )
		{
		}

		/// <summary>
		/// Calculates the size of the box, in order to draw it later.
		/// </summary>
		protected override void CalculateSize()
		{
			this.StrValue = this.Variable.LiteralValue.ToPrettyNumber();
			this.StrName = this.Variable.Name.Name;
			this.StrType = this.Variable.Type
				+ " :" + this.Variable.Type.Size
				+ " ["
				+ new IntLiteral( this.Variable.Machine, this.Variable.Address ).ToPrettyNumber()
				+ ']';

			this.BoxWidth = ( this.StrValue.Length * this.GraphInfo.NormalFont.CharWidth ) + 10;
			this.BoxHeight = this.GraphInfo.NormalFont.CharHeight;
			float lenTypeString = this.StrType.Length * this.GraphInfo.SmallFont.CharWidth;
			float lenNameString = this.StrName.Length * this.GraphInfo.SmallFont.CharWidth;

			this.BoxX = this.GraphInfo.NormalFont.CharWidth * 2;
			this.BoxY = this.GraphInfo.SmallFont.CharHeight;
			this.Width = Math.Max( Math.Max( this.BoxWidth, lenTypeString ), lenNameString );
			this.Height = this.GraphInfo.NormalFont.CharHeight + ( 2 * this.GraphInfo.SmallFont.CharHeight ) + 5;
		}

		/// <summary>
		/// Finally draw the box.
		/// </summary>
		public override void Draw()
		{
			base.Draw();

			// Draw type
			this.GraphInfo.Pen.Color = Color.Black;
			this.DrawText( this.X, this.Y, this.GraphInfo.SmallFont.Font, this.StrType );

			// Draw value (box caption)
			this.GraphInfo.Pen.Color = Color.Black;
			this.DrawText(
				this.X + ( this.GraphInfo.SmallFont.CharWidth * 3 ),
				this.Y + this.GraphInfo.NormalFont.CharHeight,
				this.GraphInfo.NormalFont.Font, this.StrValue );

			// Draw name
			this.GraphInfo.Pen.Color = Color.Black;
			this.DrawText( this.X,
				this.Y + ( this.GraphInfo.SmallFont.CharHeight * 2 ) + 10,
				this.GraphInfo.SmallFont.Font,
			    this.StrName );

			// Draw surrounding rectangle
			if ( this.Variable.IsPtr ) {
				this.GraphInfo.Pen.Color = Color.Black;
			} else {
				this.GraphInfo.Pen.Color = Color.Navy;

				if ( this.Variable.IsInHeap ) {
					this.GraphInfo.Pen.Color = Color.DeepPink;
				}
			}

			this.GraphInfo.Pen.Width += 1;
			this.DrawRectangle(
				this.X + this.BoxX,
				this.Y + this.BoxY,
				this.BoxWidth,
				this.BoxHeight );
			this.GraphInfo.Pen.Width -= 1;            
		}
	}
}
