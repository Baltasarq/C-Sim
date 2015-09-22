using System;
using System.Collections.Generic;

namespace CSim.Core {
	/// <summary>
	/// Arch manager.
	/// Facade: it handles the common tasks.
	/// </summary>
	public static class ArchManager {

		/// <summary>
		/// Parses given input and executes its opcodes.
		/// </summary>
		/// <param name="input">The user's input, as a string.</param>
        /// <param name="machine">The machine in which to execute the input.</param>
		public static Variable[] Execute(Machine machine, string input)
		{
			var resultsDictionary = new Dictionary<string, Variable>();
			var parser = new Parser( input, machine );

			try {
				// Execute opcodes
				foreach(Opcode opcode in parser.Parse()) {
					Variable result = opcode.Execute();
                    if ( result != null
                      && !( resultsDictionary.ContainsKey( result.Name.Value ) ) )
					{
						resultsDictionary.Add( result.Name.Value, result );
					}
				}
			}
			finally {
				machine.TDS.Collect();
			}

			// Return the vector of results
			var toret = new Variable[ resultsDictionary.Count ];
			resultsDictionary.Values.CopyTo( toret, 0 );
			return toret;
		}
    }
}

