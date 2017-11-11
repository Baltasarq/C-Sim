// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core {
	using System;
	using System.Collections.ObjectModel;
	using System.Collections.Generic;

	/// <summary>The snapshot manager.</summary>
	public class SnapshotManager {
		
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.SnapshotManager"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/> this snapshot manager will work with.</param>
		public SnapshotManager(Machine m)
		{
			this.Machine = m;
			this.vbles = new List<ReadOnlyCollection<Variable>>();
			this.rams = new List<ReadOnlyCollection<byte>>();
		}

		/// <summary>
		/// Saves a snapshot of the state of the <see cref="Machine"/>.
		/// </summary>
		public void SaveSnapshot() {
			this.vbles.Add( this.Machine.TDS.Variables );
			this.rams.Add( this.Machine.Memory.Raw );
		}

		/// <summary>
		/// Applies the i-th saved snapshot to the <see cref="Machine"/>.
		/// </summary>
		/// <param name="i">The index of the stored snapshots.</param>
		public void ApplySnapshot(int i)
		{
			if ( i < 0
			  || i >= this.Count )
			{
				throw new ArgumentException( "outside limits 0 < i < " + this.Count, nameof(i) );
			}

			// Apply data
			byte[] ramBytes = new byte[ this.rams[ i ].Count ];
			this.rams[ i ].CopyTo( ramBytes, 0 );
			this.Machine.Memory.Write( 0, ramBytes );

			this.Machine.TDS.Reset();
			foreach(Variable vble in this.vbles[ i ]) {
                if ( !vble.IsTemp() ) {
				    this.Machine.TDS.AddVariableInPlace( vble );
                }
			}

			return;
		}

		/// <summary>
		/// The <see cref="Machine"/> the snapshot
		/// will be recorded and applied to.
		/// </summary>
		public Machine Machine {
			get; private set;
		}

		/// <summary>
		/// Gets the count of snapshots for this <see cref="Machine"/>.
		/// </summary>
		/// <value>The count of snapshots.</value>
		public int Count {
			get { return this.vbles.Count; }
		}

		/// <summary>
		/// Reset this instance.
		/// </summary>
		public void Reset() {
			this.rams.Clear();
			this.vbles.Clear();
		}

		private List<ReadOnlyCollection<Variable>> vbles;
		private List<ReadOnlyCollection<byte>> rams;
	}
}
