using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using CSim.Core;
using CSim.Core.Variables;
using CSim.Core.Literals;

namespace CSim.Gui
{
    public class SchemaDrawer
    {
		public const int HGap = 25;
		public const int VGap = 5;

        public SchemaDrawer(SymbolTable tds)
        {
            this.tds = tds;
            this.normalFont = new Font( FontFamily.GenericMonospace, 10 );
			this.smallFont = new Font( FontFamily.GenericMonospace, 8 );
			this.boxes = new Dictionary<int, GrphBoxedVble>();
        }

        private void Init(Bitmap board)
        {
            this.bmBoard = board;
            this.board = Graphics.FromImage( this.bmBoard );
            this.pen = new Pen( Brushes.Navy );
			this.boxes.Clear();
        }

        private void DrawRectangle(float x, float y, float width, float height)
        {
            board.DrawRectangle( pen, new Rectangle( (int) x, (int) y, (int) width, (int) height ) );
        }

        private void DrawConnection(float x1, float y1, float x2, float y2)
        {
			this.pen.Color = Color.DarkGray;

            // Line
            board.DrawLine( pen, x1, y1, x2, y2 );
          
            // Arrow
            var vertices = new Point[] { 
                new Point( (int) ( x2 - 10 ), (int) y2 ),
                new Point( (int) ( x2 + 10 ), (int) y2 ),
                new Point( (int) ( x2 + 2.5 ), (int) ( y2 - 20 ) )
            };
            board.FillPolygon( this.pen.Brush, vertices );

			this.pen.Color = Color.Black;
        }

        private void DrawText(float x, float y, Font f, string s)
        {
            board.DrawString( s, f, pen.Brush, x, y, StringFormat.GenericTypographic );
        }

        private void DrawBox(GrphBoxedVble box)
		{
			float charWidthNormalFont = this.CharWidth( this.NormalFont );
			float charHeightSmallFont = this.CharHeight( this.SmallFont );
			float charHeightNormalFont = this.CharHeight( this.NormalFont );

            // Draw type
			pen.Color = Color.Black;
			this.DrawText( box.X, box.Y, this.SmallFont, box.StrType );

			// Draw value (box caption)
			pen.Color = Color.Black;
			this.DrawText( box.X + ( charWidthNormalFont * 2 ) + charWidthNormalFont,
			               box.Y + charHeightNormalFont,
 			               this.NormalFont, box.StrValue );

			// Draw name
			pen.Color = Color.Black;
			this.DrawText( box.X,
			               box.Y + charHeightSmallFont + charHeightSmallFont + ( VGap *2 ),
			               this.SmallFont, box.StrName );


			// Draw surrounding rectangle
			if ( box.Variable is PtrVariable ) {
				pen.Color = Color.Black;
			} else {
				pen.Color = Color.Navy;
				if ( box.Variable.IsInHeap ) {
					pen.Color = Color.DeepPink;
				}
			}

			pen.Width += 1;
            this.DrawRectangle( box.X + ( charWidthNormalFont *2 ), 
			                    box.Y + charHeightSmallFont,
			                    charWidthNormalFont * ( box.StrValue.Length + 1 ),
			                    charHeightNormalFont +5 );
			pen.Width -= 1;

			return;
        }

        private void Cls()
        {
            board.Clear( Color.GhostWhite );
        }

        private void DrawNullPointer(GrphBoxedVble box)
        {
            var start = new Point( (int) ( box.X + ( box.Width / 2 ) ), (int) ( box.Y -5 ) );
            var end1 = new Point( start.X, start.Y -20 );
            var end2 = new Point( end1.X + 20, end1.Y );
            var end3 = new Point( end2.X, end2.Y + 10 );

            // The semi-rectangle
            this.pen.Color = Color.DarkGray;
            board.DrawLine( this.pen, start, end1 );
            board.DrawLine( this.pen, end1, end2 );
            board.DrawLine( this.pen, end2, end3 );

            // The "ground" mark
            board.DrawLine(
                this.pen, end3.X - 10, end3.Y, end3.X + 10, end3.Y
            );
            
            board.DrawLine(
                this.pen, end3.X - 7, end3.Y + 2, end3.X + 7, end3.Y + 2
            );
            
            board.DrawLine(
                this.pen, end3.X - 3, end3.Y + 4, end3.X + 3, end3.Y + 4
            );

            this.pen.Color = Color.Black;
        }

		public void DrawRelationship(GrphBoxedVble box)
		{
			var vble = box.Variable as PtrVariable;

			if ( vble != null ) {
				int address = vble.LiteralValue.Value;

				GrphBoxedVble pointedVble = null;

				if ( address == 0 ) {
                    this.DrawNullPointer( box );
                }
                else
				if ( this.boxes.TryGetValue( address, out pointedVble ) ) {
					// Draw the connection between each other
					this.DrawConnection(
						box.X + ( box.Width / 2 ), box.Y -5,
						pointedVble.X + ( pointedVble.Width / 2 ), pointedVble.Y + pointedVble.Height +5
					);
				} else {
					this.pen.Color = Color.OrangeRed;
                    var start = new Point( (int) ( box.X + ( box.Width / 2 ) ), (int) ( box.Y -5 ) );
                    var end = new Point( (int) ( box.X + ( box.Width / 2 ) ), (int) ( box.Y -25 ) );

					board.DrawLine( this.pen, start, end );

                    board.DrawLine(
                        this.pen, end.X - 10, end.Y, end.X + 10, end.Y
                    );

					this.pen.Color = Color.Black;
				}
			}

			return;
		}

		private void CalculateBoxSize(GrphBoxedVble box)
		{
			float charWidthNormalFont = this.CharWidth( this.NormalFont );
			float charWidthSmallFont = this.CharWidth( this.SmallFont );
			float charHeightNormalFont = this.CharHeight( this.NormalFont );
			float charHeightSmallFont = this.CharHeight( this.SmallFont );

			box.StrValue = box.Variable.LiteralValue.ToString();
            box.StrName = box.Variable.Name.Value;
            box.StrType = box.Variable.Type.ToString()
                + " :" + box.Variable.Type.Size
                + " [" + Literal.ToPrettyNumber( box.Variable.Address ) + ']';

			float lenValueString = ( box.StrValue.Length +3 ) * charWidthNormalFont;
			float lenTypeString = box.StrType.Length * charWidthSmallFont;
			float lenNameString = box.StrName.Length * charWidthSmallFont;

			box.Width = Math.Max( Math.Max( lenValueString, lenTypeString ), lenNameString );
			box.Height = charHeightNormalFont + ( 2 * charHeightSmallFont ) + ( VGap * 2 );

	        if ( box.Variable is PtrVariable ) {
				box.X = this.ptrBoxCol;
	            box.Y = this.ptrBoxRow;
				this.ptrBoxCol += box.Width + HGap;
	        } else {
				box.X = this.vbleBoxCol;
	            box.Y = this.vbleBoxRow;
				this.vbleBoxCol += box.Width + HGap;
	        }
		}

        public void Draw(Bitmap bmBoard)
		{
            this.Init( bmBoard );
            this.Cls();

			this.vbleBoxCol = 10;
			this.ptrBoxCol = 10;
			this.vbleBoxRow = 10;
			this.ptrBoxRow = 250;

			// Create boxes
            foreach(Variable v in this.tds.Variables) {
				var box = new GrphBoxedVble( v );

				this.CalculateBoxSize( box );
				boxes.Add( v.Address, box );
            }

			// Draw boxes
			foreach(GrphBoxedVble box in this.boxes.Values) {
				this.DrawBox( box );
			}

			// Draw relationship lines
			foreach(GrphBoxedVble box in this.boxes.Values) {
				if ( box.Variable is PtrVariable ) {
					this.DrawRelationship( box );
				}
			}

            return;
        }

        private float CharWidth(Font f)
        {
            SizeF fontSize = board.MeasureString( "W", f );
            return ( fontSize.Width +1 );
        }

        private float CharHeight(Font f)
        {
            SizeF fontSize = board.MeasureString( "W", f );
            return ( fontSize.Height +2 );
        }

        public Font NormalFont {
            get { return this.normalFont; }
        }

		public Font SmallFont {
            get { return this.smallFont; }
        }

		public void UpdateFont(int step)
		{
            this.normalFont = new Font( this.normalFont.FontFamily, this.normalFont.Size + step );
			this.smallFont = new Font( this.smallFont.FontFamily, this.smallFont.Size + step );
		}

        private SymbolTable tds = null;
		private Dictionary<int, GrphBoxedVble> boxes = null;

        private Bitmap bmBoard;
        private Graphics board;
        private Pen pen = null;
        private Font normalFont = null;
		private Font smallFont = null;

		private float vbleBoxCol;
		private float ptrBoxCol;
		private float vbleBoxRow;
		private float ptrBoxRow;

    }
}

