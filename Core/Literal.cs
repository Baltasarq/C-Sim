using System;

using CSim.Core.Types;
using CSim.Core.Literals;
using CSim.Core.Exceptions;

namespace CSim.Core {
    /// <summary>
    /// Literals are rvalues.
    /// This is the parent class for literals of all types.
    /// </summary>
    public abstract class Literal: RValue {

		// Supported kinds of number display
		public enum DisplayType { Dec, Hex };

		public Literal(Machine m, object v)
		{
			this.Value = v;
			this.machine = m;
		}

        /// <summary>
        /// The value stored, of no type at this level.
        /// </summary>
        /// <value>The value.</value>
        public override object Value {
			get;
        }

		/// <summary>
		/// Gets or sets the machine this literal pertains to.
		/// </summary>
		/// <value>The machine.</value>
		public Machine Machine {
			get {
				return this.machine;
			}
		}

        /// <summary>
        /// Gets the raw value of the literal, as secquence of bytes.
		/// We need the machine to do that, since it handles endianness.
        /// </summary>
        /// <return>The raw value.</return>
		public abstract byte[] GetRawValue(Machine m);

		/// <summary>
		/// Returns the given value as an hex value.
		/// </summary>
		/// <returns>
		/// The hex value, as a string.
		/// </returns>
		/// <param name='value'>
		/// The value, as an integer.
		/// </param>
		public static string ToHex(int value)
		{
			return value.ToString( "x" ).PadLeft( 2, '0' );
		}

		/// <summary>
		/// Inserts a '0x' at the beginning of the hex value.
		/// </summary>
		/// <returns>The pretty hex, a a string.</returns>
		/// <param name="value">The value to convert, as an int.</param>
		/// <seealso cref="Literal.ToHex"/>
		public static string ToPrettyHex(int value)
		{
			return "0x" + ToHex( value );
		}

		/// <summary>
		/// Converts a value to decimal, ocuppying three chars.
		/// </summary>
		/// <returns>The value, as a string.</returns>
		/// <param name="value">The value, as a string.</param>
		public static string ToDec(int value)
		{
			return value.ToString().PadLeft( 3, '0' );
		}

		/// <summary>
		/// Converts a value to the pretty representation selected.
		/// </summary>
		/// <returns>The pretty number, as a string.</returns>
		/// <param name="value">The value, as an int.</param>
		public static string ToPrettyNumber(int value)
		{
			string toret;

			if ( Display == DisplayType.Dec ) {
				toret = ToDec( value );
			}
			else {
				toret = ToPrettyHex( value );
			}

			return toret;
		}

		/// <summary>
		/// Gets the value as int, if possible.
		/// </summary>
		/// <returns>The value as int.</returns>
		public int GetValueAsInt() {
			int toret = 0;

			if ( this is IntLiteral
			  || this is CharLiteral
			  || this is DoubleLiteral )
			{
				toret = Convert.ToInt32( this.Value );
			} else {
				throw new TypeMismatchException( this.Value.ToString() );
			}

			return toret;
		}

		private Machine machine;
		public static DisplayType Display = DisplayType.Hex;
    }
}

