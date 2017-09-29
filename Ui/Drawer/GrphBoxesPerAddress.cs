namespace CSim.Ui.Drawer {
    using System.Linq;
    using System.Numerics;
    using System.Collections.Generic;

    /// <summary>
    /// Keeps track of all graphic boxes pertaining to variables
    /// at a given address.
    /// </summary>
    public class GrphBoxesPerAddress {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:CSim.Ui.Drawer.GrphBoxesPerAddress"/> class.
        /// </summary>
        public GrphBoxesPerAddress()
        {
            this.boxes = new Dictionary<BigInteger, List<GrphBoxedVariable>>();
        }
        
        /// <summary>
        /// Checks whether a box with the given id at the given address exists.
        /// </summary>
        /// <returns><c>true</c>, if box was found, <c>false</c> otherwise.</returns>
        /// <param name="id">The identifier, as a string.</param>
        /// <param name="address">The address, as a string.</param>
        public bool IsBoxContainedWith(string id, BigInteger address)
        {
            bool toret = false;
            List<GrphBoxedVariable> l;
                        
            if ( this.boxes.TryGetValue( address, out l ) ) {
                toret =  !( l.TrueForAll( (box) => box.Variable.Name.Name != id ) );
            }
            
            return toret;
        }
        
        /// <summary>
        /// Adds a given range of <see cref="GrphBoxedVariable"/>'s.
        /// </summary>
        /// <param name="l">A sequence of <see cref="GrphBoxedVariable"/>'s.</param>
        public void AddRange(IEnumerable<GrphBoxedVariable> l)
        {
            foreach(GrphBoxedVariable gvble in l) {
                this.Add( gvble );
            }
            
            return;
        }
        
        /// <summary>
        /// Add a new box at its address
        /// </summary>
        /// <param name="box">A <see cref="T:GrphBoxVariable"/>.</param>
        public void Add(GrphBoxedVariable box)
        {
            BigInteger address = box.Variable.Address;
            List<GrphBoxedVariable> l;
                
            if ( !this.IsBoxContainedWith( box.Variable.Name.Name, address ) ) {
	            if ( this.boxes.TryGetValue( address, out l) ) {
	                l.Add( box );
	            } else {
	                l = new List<GrphBoxedVariable> { box };
	                this.boxes.Add( address, l );
	            }
            }
            
            return;
        }
        
        /// <summary>
        /// Gets the boxes for a given address.
        /// </summary>
        /// <returns>The boxes for that address.</returns>
        /// <param name="address">The address, as a <see cref="BigInteger"/>.</param>
        public IList<GrphBoxedVariable> GetBoxesForAddress(BigInteger address)
        {
            List<GrphBoxedVariable> toret;
            
            if ( !this.boxes.TryGetValue( address, out toret) ) {
                toret = new List<GrphBoxedVariable>();          // No elements
            }
            
            return toret;
        }
        
        /// <summary>
        /// Gets all boxes stored, no matter their addresses.
        /// </summary>        
        /// <value>An IList containing the boxes.</value>
        public IList<GrphBoxedVariable> AllBoxes {
            get {
                var toret = new List<GrphBoxedVariable>();
                
                this.boxes.Values.ToList().ForEach( (l) => {
                    l.ForEach( (box) => toret.Add( box ));
                });
                
                return toret;
            }
        }
        
        /// <summary>
        /// Gets all addressess.
        /// </summary>
        /// <value>All addressess, as a list of BigInteger's.</value>
        public IList<BigInteger> AllAddressess {
            get {
                var toret = new List<BigInteger>();
                
                this.boxes.Keys.ToList().ForEach( (address) => {
                    toret.Add( address );
                });
                
                return toret;
            }
        }
        
        /// <summary>
        /// Gets the total count of stored boxes.
        /// </summary>
        /// <value>The count, as an integer.</value>
        public int Count {
            get {
                int toret = 0;
                this.boxes.Values.ToList().ForEach( (l) => toret += l.Count );
                return toret;
            }
        }

        private readonly Dictionary<BigInteger, List<GrphBoxedVariable>> boxes;
    }
}
