namespace CSim.Ui.Drawer {
	using System;
    using System.Linq;
    using System.Numerics;
	using System.Drawing;
	using System.Collections.Generic;

	using CSim.Core;
	using CSim.Core.Literals;
	using CSim.Core.Types;
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
            int count = this.ArrayVariable.Count.ToInt32();
			string completeContents = "";
			string separator = "";
			
			var toret = new string[ count ];
            Literal[] lits = this.ArrayVariable.ExtractArrayElementsValues();

			for (int i = 0; i < count; ++i) {
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
			var elementsType = ( (Ptr) this.ArrayVariable.Type ).DerreferencedType;
			this.StrValues = this.ExtractArrayElementValues();

			// Determine values width
			foreach (string lit in this.StrValues) {
				this.BoxWidth += ( lit.Length * this.GraphInfo.NormalFont.CharWidth ) + 5;
			}

			// Determine other sizes
			this.StrName = this.Variable.Name.Name;
			this.StrType = elementsType
				+ "[" + this.ArrayVariable.Count
				+ "] :" + elementsType.Size * this.ArrayVariable.Count
				+ " ["
				+ new IntLiteral( this.Variable.Machine, this.Variable.Address ).ToPrettyNumber()
				+ ']';

			float lenTypeString = this.StrType.Length * this.GraphInfo.SmallFont.CharWidth;
			float lenNameString = this.StrName.Length * this.GraphInfo.SmallFont.CharWidth;

			this.BoxX = this.GraphInfo.NormalFont.CharWidth * 2;
			this.BoxY = this.GraphInfo.SmallFont.CharHeight;
			this.Width = Math.Max( Math.Max( this.BoxWidth, lenTypeString ), lenNameString );
			this.Height = this.GraphInfo.NormalFont.CharHeight + ( 2 * this.GraphInfo.SmallFont.CharHeight ) + 5;
            
            // Create box for array elements
            this.elementBoxes = new List<GrphBoxedVariable>();
            
            if ( elementsType is Ptr ) {
	            float space = 0;
	            for(int i = 0; i < this.ArrayVariable.Count; ++i) {
	                float posX = this.StartArrayElementsText + space;
	                	                
                    var box = new GrphBoxedArrayElement(
                                Variable.CreateTempVariableForArrayElement(
                                        this.ArrayVariable.Name.Name,
                                        this.ArrayVariable.Address,
                                        (Ptr) this.ArrayVariable.Type,
                                        i ),
                                this.GraphInfo )
                            {
                                X = posX,
                                Y = this.Y,
                            };
                            
                    this.elementBoxes.Add( box );
                    space += this.CalculateSpaceForElement( i );
	            }
            }
            
            return;
		}

		/// <summary>
		/// Draw this array.
		/// </summary>
		public override void Draw()
		{
            AType elementsType = ( (Ptr) this.ArrayVariable.Type ).DerreferencedType; 
			base.Draw();

			// Determine color for the surrounding rectangle
			Color clRectangle = Color.Black;

			if ( this.Variable.IsInHeap ) {
				clRectangle = Color.DeepPink;
			}

			// Draw type
			this.GraphInfo.Pen.Color = Color.Black;
			this.DrawText( this.X, this.Y, this.GraphInfo.SmallFont.Font, this.StrType );

			// Draw elements in the array
			this.GraphInfo.Pen.Color = Color.Black;
			float space = 0;
			for (int i = 0; i < this.StrValues.Length; ++i) {
                float posX = this.StartArrayElementsText + space;
            
                // Element
				this.DrawText(
					posX,
				    this.Y + this.GraphInfo.NormalFont.CharHeight,
				    this.GraphInfo.NormalFont.Font,
					this.StrValues[ i ] );
				
                // Show division
                space += this.CalculateSpaceForElement( i );
                this.DrawLine(
                        this.StartArrayElementsText + space - 1,
                        this.Y + this.GraphInfo.SmallFont.CharHeight,
                        this.StartArrayElementsText + space - 1,
                        this.Y + ( this.GraphInfo.SmallFont.CharHeight * 2 ) + 5 );
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
        /// Gets the start of text for array elements (x coordinate).
        /// </summary>
        /// <value>The start, as a float.</value>
        public float StartArrayElementsText {
            get {
                return this.X + ( this.GraphInfo.NormalFont.CharWidth * 3 );
            }
        }
        
        /// <summary>
        /// Calculates the space needed for a given array element.
        /// </summary>
        /// <returns>The space needed for array element.</returns>
        /// <param name="i">The index of the array element.</param>
        public float CalculateSpaceForElement(int i)
        {
            return ( this.StrValues[ i ].Length
                    * this.GraphInfo.NormalFont.CharWidth ) + 3;
        }

		/// <summary>
		/// Gets the related boxes.
		/// This will return the individual items as if they were boxes.
		/// </summary>
		/// <returns>The related boxes.</returns>
		public override List<GrphBoxedVariable> GetInvolvedBoxes()
		{
			List<GrphBoxedVariable> toret = base.GetInvolvedBoxes();
            
            toret.AddRange( this.elementBoxes );
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
        
        /// <summary>
        /// Gets the array variable. Reinterprets the Variable property.
        /// </summary>
        /// <value>The <see cref="ArrayVariable"/>.</value>
        /// <seealso cref="GrphBoxedVariable.Variable"/>
        public ArrayVariable ArrayVariable {
            get {
                return (ArrayVariable) this.Variable;
            }
        }
        
        private List<GrphBoxedVariable> elementBoxes;
	}
}
