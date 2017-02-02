namespace CSim.Ui.Drawer {
	using System;
	using System.Drawing;
	using System.Collections.Generic;
	using System.Windows.Forms.Design;

	using CSim.Core;
	using CSim.Core.Types;
	using CSim.Core.Literals;
	using CSim.Core.Variables;


	/// <summary>
	/// A variable to be drawn, that is actually an array
	/// </summary>
	public class GrphBoxedArray: GrphBoxedVariable {
		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Ui.Drawer.GrphBoxedArray"/> class.
		/// </summary>
		/// <param name="array">The array, as an ArrayVariable.</param>
		/// <param name="grf">The graphics settings, as a GraphInfo.</param>
		public GrphBoxedArray(ArrayVariable array, GraphInfo grf)
			:base( array, grf )
		{
		}

		/// <summary>
		/// Extracts the values of an array from memory.
		/// </summary>
		private string[] ExtractArrayElementValues()
		{
			string completeContents = "";
			string separator = "";
			var arrVble = this.Variable as ArrayVariable;
			Literal[] lits = arrVble.ExtractArrayElementValues();
			var toret = new string[ arrVble.Count ];

			for (int i = 0; i < arrVble.Count; ++i) {
				toret[ i ] = lits[ i ].AsArrayElement();
				completeContents += separator + toret[ i ];
				separator = ", ";
			}

			this.StrValue = completeContents;
			return toret;
		}

		/// <summary>
		/// Calculates the size.
		/// </summary>
		protected override void CalculateSize()
		{
			this.StrValues = this.ExtractArrayElementValues();

			// Determine values width
			foreach (string lit in this.StrValues) {
				this.BoxWidth += ( lit.Length * this.GraphInfo.NormalFont.CharWidth ) + 5;
			}

			// Determine other sizes
			this.StrName = this.Variable.Name.Name;
			this.StrType = this.Variable.Type
				+ " :" + this.Variable.Type.Size
				+ " ["
				+ new IntLiteral( this.Variable.Machine, this.Variable.Address ).ToPrettyNumber()
				+ ']';

			float lenTypeString = this.StrType.Length * this.GraphInfo.SmallFont.CharWidth;
			float lenNameString = this.StrName.Length * this.GraphInfo.SmallFont.CharWidth;

			this.BoxX = this.GraphInfo.NormalFont.CharWidth * 2;
			this.BoxY = this.GraphInfo.SmallFont.CharHeight;
			this.Width = Math.Max( Math.Max( this.BoxWidth, lenTypeString ), lenNameString );
			this.Height = this.GraphInfo.NormalFont.CharHeight + ( 2 * this.GraphInfo.SmallFont.CharHeight ) + 5;
		}

		/// <summary>
		/// Draw this array.
		/// </summary>
		public override void Draw()
		{
			base.Draw();

			// Determine color for the surrounding rectangle
			Color clRectangle = Color.Black;

			if ( this.Variable.IsInHeap ) {
				clRectangle = Color.DeepPink;
			}

			// Draw type
			this.GraphInfo.Pen.Color = Color.Black;
			this.DrawText( this.X, this.Y, this.GraphInfo.SmallFont.Font, this.StrType );

			// Draw value (box caption)
			this.GraphInfo.Pen.Color = Color.Black;
			float space = 0;
			float beginning = this.X + ( this.GraphInfo.NormalFont.CharWidth * 3 );
			for (int i = 0; i < this.StrValues.Length; ++i) {
				this.DrawText(
					beginning + space,
				    this.Y + this.GraphInfo.NormalFont.CharHeight,
				    this.GraphInfo.NormalFont.Font,
					this.StrValues[ i ] );
				
				space += ( this.StrValues[ i ].Length * this.GraphInfo.NormalFont.CharWidth ) + 3;
				this.DrawLine(
						beginning + space - 1, this.Y + this.GraphInfo.SmallFont.CharHeight,
						beginning + space - 1, this.Y + ( this.GraphInfo.SmallFont.CharHeight * 2 ) + 5 );
			}

			// Draw name
			this.GraphInfo.Pen.Color = Color.Black;
			this.DrawText( this.X,
			              this.Y + ( this.GraphInfo.SmallFont.CharHeight * 2 ) + 10,
			              this.GraphInfo.SmallFont.Font,
			              this.StrName );

			// Draw surrounding rectangle
			this.GraphInfo.Pen.Color = clRectangle;
			this.GraphInfo.Pen.Width += 1;
			this.DrawRectangle(
				this.X + ( this.GraphInfo.NormalFont.CharWidth * 2 ),
				this.Y + this.GraphInfo.SmallFont.CharHeight,
				this.BoxWidth,
				this.GraphInfo.NormalFont.CharHeight + 5 );
			this.GraphInfo.Pen.Width -= 1;
		}

		/// <summary>
		/// Gets the related boxes.
		/// This will return the individual items as if they were boxes.
		/// </summary>
		/// <returns>The related boxes.</returns>
		public override IDictionary<long, GrphBoxedVariable> GetInvolvedBoxes ()
		{
			IDictionary<long, GrphBoxedVariable> toret = base.GetInvolvedBoxes();


			return toret;
		}

		/// <summary>
		/// Gets or sets the string values corresponding
		/// to each member of the array.
		/// </summary>
		/// <value>The string values, as a string[].</value>
		public string[] StrValues {
			get; protected set;
		}
	}
}
