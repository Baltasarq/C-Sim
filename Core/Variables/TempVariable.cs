﻿//
// TempVariable.cs
//
// Author:
//       baltasarq <baltasarq@gmail.com>
//
// Copyright (c) 2017 baltasarq
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;

namespace CSim.Core.Variables {
	/// <summary>
	/// A temp variable that should be collected
	/// before the next execution.
	/// </summary>
	public class TempVariable: Variable {
		/// <summary>The prefix for labels for temp variables.</summary>
		public const string EtqTempVariable = "_aux__";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Variables.TempVariable"/> class.
		/// </summary>
		/// <param name="t">A <see cref="CSim.Core.Type"/>.</param>
		/// <param name="m">A <see cref="Machine"/>.</param>
		public TempVariable(Core.Type t, Machine m)
			:base( new Id( "aux" ), t, m )
		{
			this.SetNameWithoutChecking( EtqTempVariable + NumTemps );
			++NumTemps;
		}

		/// <summary>
		/// Gets or sets the number temp variables.
		/// </summary>
		/// <value>The number of temps, as an int.</value>
		public static int NumTemps {
			get; private set;
		}
	}
}

