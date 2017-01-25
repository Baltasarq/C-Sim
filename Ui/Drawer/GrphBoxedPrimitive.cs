namespace CSim.Ui.Drawer {
	using System;
	using System.Drawing;
	using System.Collections.Generic;

	using CSim.Core;
	using CSim.Core.Variables;


	/// <summary>
	/// Represents a variable shown in the img window.
	/// </summary>
	public class GrphBoxedPrimitive: GrphBoxedVariable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Ui.Drawer.GrphBoxedPrimitive"/> class.
		/// </summary>
		/// <param name="v">The Variable, should be a Primitive.</param>
		/// <param name="grf">All graphics settings.</param>
		public GrphBoxedPrimitive(Variable v, GraphInfo grf)
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
				+ " [0x"
				+ this.Variable.Address.ToString( "x" ).
								PadLeft( this.Variable.Machine.WordSize * 2, '0' )
				+ ']';

			float lenValueString = this.StrValue.Length * this.GraphInfo.NormalFont.CharWidth;
			float lenTypeString = this.StrType.Length * this.GraphInfo.SmallFont.CharWidth;
			float lenNameString = this.StrName.Length * this.GraphInfo.SmallFont.CharWidth;

			this.Width = Math.Max( Math.Max( lenValueString, lenTypeString ), lenNameString );
			this.Height = this.GraphInfo.NormalFont.CharHeight + ( 2 * this.GraphInfo.SmallFont.CharHeight );
		}

		/// <summary>
		/// Finally draw the box.
		/// </summary>
		public override void Draw()
		{
			// Draw type
			this.GraphInfo.Pen.Color = Color.Black;
			this.DrawText( this.X, this.Y, this.GraphInfo.SmallFont.Font, this.StrType );

			// Draw value (box caption)
			this.GraphInfo.Pen.Color = Color.Black;
			this.DrawText( this.X + ( this.GraphInfo.NormalFont.CharWidth * 3 ) + 10,
				this.Y + this.GraphInfo.NormalFont.CharHeight,
				this.GraphInfo.NormalFont.Font, this.StrValue );

			// Draw name
			this.GraphInfo.Pen.Color = Color.Black;
			this.DrawText( this.X,
				this.Y + ( this.GraphInfo.SmallFont.CharHeight * 2 ) + 10,
				this.GraphInfo.SmallFont.Font,
			    this.StrName );

			// Draw surrounding rectangle
			if ( this.Variable is PtrVariable ) {
				this.GraphInfo.Pen.Color = Color.Black;
			} else {
				this.GraphInfo.Pen.Color = Color.Navy;

				if ( this.Variable.IsInHeap ) {
					this.GraphInfo.Pen.Color = Color.DeepPink;
				}
			}

			this.GraphInfo.Pen.Width += 1;
			this.DrawRectangle(
			    this.X + ( this.GraphInfo.NormalFont.CharWidth * 2 ),
				this.Y + this.GraphInfo.SmallFont.CharHeight,
				this.GraphInfo.NormalFont.CharWidth * ( this.StrValue.Length + 5 ),
				this.GraphInfo.NormalFont.CharHeight + 5 );
			this.GraphInfo.Pen.Width -= 1;
		}
	}
}
