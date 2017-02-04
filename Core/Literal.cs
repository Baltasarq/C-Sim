
namespace CSim.Core {
	using System;
	using System.Text;
	using CSim.Core.Literals;

    /// <summary>
    /// Literals are rvalues.
    /// This is the parent class for literals of all types.
    /// </summary>
    public abstract class Literal: RValue {

		/// <summary>Supported kinds of number display</summary>
		public enum DisplayType {
			///<summary>Decimal</summary>
			Dec,
			/// <summary>Hexadecimal</summary>
			Hex
		};

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Literal"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/> this literal is created for.</param>
		/// <param name="v">The value, as a <see cref="System.Object"/>.</param>
		protected Literal(Machine m, object v)
		{
			this.val = v;
			this.Machine = m;
		}

        /// <summary>
        /// The value stored, of no type at this level.
        /// </summary>
        /// <value>The value.</value>
        public override object Value {
			get {
				return this.val;
			}
        }

		/// <summary>
		/// Gets or sets the machine this literal pertains to.
		/// </summary>
		/// <value>The machine.</value>
		public Machine Machine {
			get; set;
		}

        /// <summary>
        /// Gets the raw value of the literal, as secquence of bytes.
		/// We need the machine to do that, since it handles endianness.
        /// </summary>
        /// <return>The raw value.</return>
		public abstract byte[] GetRawValue();

		/// <summary>
		/// Converts the current value, in little endian, to hexadecimal.
		/// </summary>
		/// <returns>The little endian to hex.</returns>
		protected string FromLittleEndianToHex()
		{
			var toret = new StringBuilder();
			byte[] bytes = this.GetRawValue();

			foreach(byte bt in bytes) {
				toret.Insert( 0, bt.ToString( "x" ).PadLeft( 2, '0' ) );
			}

			return toret.ToString();
		}

		/// <summary>
		/// Converts the current value, in big endian, to hexadecimal.
		/// </summary>
		/// <returns>The big endian to hex.</returns>
		protected string FromBigEndianToHex()
		{
			var toret = new StringBuilder();
			byte[] bytes = this.GetRawValue();

			for(int i = bytes.Length -1; i >= 0; --i) {
				toret.Insert( 0, bytes[ i ].ToString( "x" ).PadLeft( 2, '0' ) );
			}

			return toret.ToString();			
		}

		/// <summary>
		/// Returns the current value as an hex value.
		/// </summary>
		/// <returns>
		/// The hex value, as a string.
		/// </returns>
		public string ToHex()
		{
			string toret;

			if ( this.Machine.IsBigEndian ) {
				toret = FromBigEndianToHex();
			} else {
				toret = FromLittleEndianToHex();
			}

			return ShortenNumber( toret );
		}

		/// <summary>
		/// Inserts a '0x' at the beginning of the hex value.
		/// </summary>
		/// <returns>The pretty hex, a a string.</returns>
		/// <seealso cref="Literal.ToHex"/>
		public string ToPrettyHex()
		{
			return "0x" + this.ToHex();
		}

		/// <summary>
		/// Converts a value to decimal, ocuppying three chars.
		/// </summary>
		/// <returns>The value, as a string.</returns>
		public string ToDec()
		{
			return ShortenNumber( this.GetValueAsInt().ToString().PadLeft( 4, '0' ) );
		}

		/// <summary>
		/// Converts the info to the shortest possible string.
		/// </summary>
		/// <returns>The array element, represented as a string.</returns>
		public virtual string AsArrayElement()
		{
			return this.ToDec();
		}

		/// <summary>
		/// Converts a value to the pretty representation selected.
		/// </summary>
		/// <returns>The pretty number, as a string.</returns>
		public string ToPrettyNumber()
		{
			string toret;

			if ( Literal.Display == DisplayType.Dec ) {
				toret = this.ToDec();
			} else {
				toret = this.ToPrettyHex();
			}

			return toret;
		}

		/// <summary>
		/// Shorts the given value, removing zeroes at the left.
		/// </summary>
		/// <returns>The shortest possible version of the value.</returns>
		/// <param name="value">The numeric value, decimal or hexadecimal, as string.</param>
		public static string ShortenNumber(string value)
		{
			int zeroes = 0;

			// How many zeroes at the left?
			value = value.Trim();
			while( zeroes < value.Length
				&& value[ zeroes ] == '0' )
			{
				++zeroes;
			}

			// Remove them!
			value = value.Substring( zeroes );

			// Less than two or four digits?
			if ( value.Length < 2
			  || ( value.Length < 4 && value.Length != 2 ) )
			{
				value = "0" + value;
			}					

			return value;
		}

		/// <summary>
		/// Gets the value as int, if possible.
		/// </summary>
		/// <returns>The value as int.</returns>
		public long GetValueAsInt()
		{
			long toret = 0;

			if ( this is IntLiteral
			  || this is CharLiteral
			  || this is DoubleLiteral )
			{
				toret = Convert.ToInt64( this.Value );
			}

			return toret;
		}

		/// <summary>
		/// The type of display (<see cref="DisplayType"/>),
		/// to apply to all values while converting them to string.
		/// </summary>
		public static DisplayType Display = DisplayType.Hex;

		private object val;
    }
}

