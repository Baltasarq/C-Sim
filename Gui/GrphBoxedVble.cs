using System;

using CSim.Core;

namespace CSim.Gui
{
	/// <summary>
	/// Represents a variable shown in the img window.
	/// </summary>
	public class GrphBoxedVble
	{
		public GrphBoxedVble(Variable v)
		{
			this.v = v;
		}

		public float X {
			get; set;
		}

		public float Y {
			get; set;
		}

		public float Width {
			get; set;
		}

		public float Height {
			get; set;
		}

		public Variable Variable {
			get { return this.v; }
		}

        public string StrValue {
            get; set;
        }

        public string StrType {
            get; set;
        }

        public string StrName {
            get; set;
        }


		private Variable v = null;
	}
}

