// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Ui.Drawer {
	using System.Drawing;
    using System.Text;
	using System.Collections.Generic;

	/// <summary>
	/// It manages the arrangement of variable boxes in rows.
	/// </summary>
	public class GrphRows {
		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Ui.Drawer.GrphRows"/> class.
		/// </summary>
		/// <param name="hGap">The horizontal separation among boxes.</param>
		/// <param name="vGap">The vertical separation among boxes.</param>
		/// <param name="max">The maximum number of boxes per row</param>
		public GrphRows(int hGap, int vGap, int max)
		{
			this.rows = new List<List<GrphBoxedVariable>>();

			this.HGap = hGap;
			this.VGap = vGap;
			this.MaxElements = max;

			this.CreateNewRow();
		}

		private void CreateNewRow()
		{
			this.rows.Add( new List<GrphBoxedVariable>() );
		}

		/// <summary>
		/// Forces the creation of a new row.
		/// </summary>
		public void ForceNewRow()
		{
			this.CreateNewRow();
		}

		/// <summary>
		/// Adds the box to the current row, adds a new row if needed.
		/// </summary>
		/// <param name="box"></param>
		public void AddBox(GrphBoxedVariable box)
		{
			this.AddRowIfNeeded();
			IList<GrphBoxedVariable> lastRow = this.LastRow;
			GrphBoxedVariable lastBoxInRow = this.GetLastBoxInRow( lastRow );

			if ( lastBoxInRow != null ) {
				box.X = lastBoxInRow.X + lastBoxInRow.Width + this.HGap;
				box.Y = lastBoxInRow.Y;
			} else {
				float verticalSupplement = 0;
				var lastBoxInPreviousRow = this.GetLastBoxInPreviousRow();

				if ( lastBoxInPreviousRow != null ) {
					verticalSupplement = lastBoxInPreviousRow.Y + lastBoxInPreviousRow.Height;
				}

				box.X = this.HGap;
				box.Y = verticalSupplement + this.VGap;
			}

			lastRow.Add( box );

			return;
		}

		/// <summary>
		/// Gets the last box in the last row.
		/// </summary>
		/// <returns>The last box in the last row, null if no boxes exist.</returns>
		/// <param name="row">Optionally, the row in which to find the last box.</param>
		public GrphBoxedVariable GetLastBoxInRow(IList<GrphBoxedVariable> row = null)
		{
			GrphBoxedVariable toret = null;

			if ( this.rows.Count > 0 ) {
				if ( row == null ) {
					row = this.LastRow;
				}

				if ( row.Count > 0 ) {
					toret = row[ row.Count - 1 ];
				}
			}

			return toret;
		}

		/// <summary>
		/// Gets the last box in the previous row to the one passed.
		/// </summary>
		/// <returns>The last box in the previous row, null if no previous row exist.</returns>
		public GrphBoxedVariable GetLastBoxInPreviousRow()
		{
			GrphBoxedVariable toret = null;
			int delta = 1;

			while ( this.rows.Count > delta ) {
				IList<GrphBoxedVariable> previousRow = this.rows[ this.rows.Count - delta - 1 ];

				if ( previousRow.Count > 0 ) {
					toret = previousRow[ previousRow.Count - 1 ];
					break;
				}

				++delta;
			}

			return toret;
		}
        
        /// <summary>
        /// Gets the index of the last row.
        /// </summary>
        /// <value>The index.</value>
        public int LastRowIndex {
            get {
                return this.rows.Count - 1;
            }
        }

		/// <summary>
		/// Gets the last row in use.
		/// </summary>
		/// <value>The last row, as an IList instance.</value>
		public IList<GrphBoxedVariable> LastRow {
			get {
				return this.rows[ this.LastRowIndex ];
			}
		}

		private void AddRowIfNeeded()
		{
			var row = this.LastRow;

			if ( row != null
			  && row.Count >= this.MaxElements )
			{
				this.CreateNewRow();
			}		

			return;
		}

		/// <summary>
		/// Calculates the size of the needed area for drawing the schema.
		/// </summary>
		/// <returns>The area size, as a <see cref="Size"/> object.</returns>
		public Size CalculateAreaSize()
		{
			var toret = new Size();

			foreach (List<GrphBoxedVariable> row in this.rows) {
				if ( row.Count < 1 ) {
					continue;
				}

				toret.Height += (int) row[ 0 ].Height + this.HGap;

				int width = 0;
				foreach (GrphBoxedVariable box in row) {
					width += (int) box.Width;
				}

				width += row.Count * this.HGap;
				toret.Width = System.Math.Max( toret.Width, width );
			}

			return toret;
		}

		/// <summary>
		/// Max elements per row
		/// </summary>
		/// <value>The max elements.</value>
		public int MaxElements {
			get; private set;
		}

		/// <summary>
		/// Gets the separation between boxes.
		/// </summary>
		/// <value>The horizontal separation, as an int.</value>
		public int HGap {
			get; private set;
		}

		/// <summary>
		/// Gets the separation between rows of boxes.
		/// </summary>
		/// <value>The vertical separation, as an int.</value>
		public int VGap {
			get; private set;
		}
        
        /// <summary>
        /// Gets the count of total elements.
        /// </summary>
        /// <value>The count, as an int.</value>
        public int Count {
            get {
                int toret = 0;
                
                this.rows.ForEach( (row) => toret += row.Count );
                return toret;
            }
        }
        
        /// <summary>
        /// Gets all the rows as a single string.
        /// </summary>
        /// <returns>The as string.</returns>
        public string RowsAsString()
        {
            StringBuilder toret = new StringBuilder();
            
            this.rows.ForEach( (row) => {
                toret.Append( string.Join( ", ", row) );
                toret.Append( '\n' );
            });
            
            return toret.ToString();
        }
        
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Ui.Drawer.GrphRows"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Ui.Drawer.GrphRows"/>.</returns>
        public override string ToString()
        {
            return string.Format( "[GrphRows: LastRow={0}, MaxElements={1}, HGap={2}, VGap={3}\nRows={4}\n#={5}]",
                            string.Join( ", ", this.LastRow ),
                            MaxElements, HGap, VGap,
                            this.RowsAsString(),
                            this.Count );
        }

		private List<List<GrphBoxedVariable>> rows;
	}
}

