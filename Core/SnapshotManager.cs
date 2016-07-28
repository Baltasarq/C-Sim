namespace CSim.Core {
	using System;
	using System.Collections.ObjectModel;
	using System.Collections.Generic;

	public class SnapshotManager {
		public SnapshotManager(Machine m)
		{
			this.Machine = m;
			this.vbles = new List<ReadOnlyCollection<Variable>>();
			this.rams = new List<ReadOnlyCollection<byte>>();
		}

		public void SaveSnapshot() {
			this.vbles.Add( this.Machine.TDS.Variables );
			this.rams.Add( this.Machine.Memory.Raw );
		}

		public void ApplySnapshot(int i)
		{
			if ( i < 0
			  || i >= this.Count )
			{
				throw new ArgumentException( "outside limits 0 < num < " + this.Count, "num" );
			}

			// Apply data
			byte[] ramBytes = new byte[ this.rams[ i ].Count ];
			this.rams[ i ].CopyTo( ramBytes, 0 );
			this.Machine.Memory.Write( 0, ramBytes );

			this.Machine.TDS.Reset();
			foreach(Variable vble in this.vbles[ i ]) {
				this.Machine.TDS.AddVariableInPlace( vble );
			}

			return;
		}

		public Machine Machine {
			get; private set;
		}

		public int Count {
			get { return this.vbles.Count; }
		}

		public void Reset() {
			this.rams.Clear();
			this.vbles.Clear();
		}

		private List<ReadOnlyCollection<Variable>> vbles;
		private List<ReadOnlyCollection<byte>> rams;
	}
}
