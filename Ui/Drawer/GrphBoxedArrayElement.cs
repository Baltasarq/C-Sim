namespace CSim.Ui.Drawer {
    using Core;
    
    /// <summary>
    /// A fake GrphBox, just used to represent array elements.
    /// Nothing is drawn, since the <see cref="GrphBoxedArray"/>
    /// already does that. The existence of this GrphBox is for Boxes
    /// of pointers being able to point to individual array elements, and
    /// from array elements to other boxes.
    /// </summary>
    public class GrphBoxedArrayElement: GrphBoxedVariable {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:CSim.Ui.Drawer.GrphBoxedArrayElement"/> class.
        /// </summary>
        /// <param name="v">The <see cref="Variable"/> to represent.</param>
        /// <param name="grf">Grf.</param>
        public GrphBoxedArrayElement(Variable v, GraphInfo grf)
            :base( v, grf )
        {
        }
        
        /// <summary>
        /// Calculates the size of the box, in order to draw it later.
        /// </summary>
        protected override void CalculateSize()
        {
            this.StrValue = "";
            this.StrName = "";
            this.StrType = "";
            this.BoxWidth = 8;
            this.BoxHeight = 0;
            this.BoxX = 0;
            this.BoxY = 0;
            this.Width = 10;
            this.Height = this.GraphInfo.NormalFont.CharHeight
                            + ( 2 * this.GraphInfo.SmallFont.CharHeight ) + 5;
        }
        
        /// <summary>
        /// Finally draw the box.
        /// </summary>
        public override void Draw()
        {
        }
    }
}
