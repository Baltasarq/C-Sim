using CSim.Core.Literals;

namespace CSimTests {
	using NUnit.Framework;
	using CSim.Core;
	using CSim.Core.Types;

	[TestFixture]
	public class TypeTests {

		[OneTimeSetUp]
		public void Init()
		{
			this.vm = new Machine();

			this.any_t = Any.Get( this.vm );
			this.int_t = this.vm.TypeSystem.GetIntType();
			this.char_t = this.vm.TypeSystem.GetCharType();
			this.double_t = this.vm.TypeSystem.GetDoubleType();
		}

		[Test]
		public void ByteConversion()
		{
			long int_v = 5;
			char char_v = 'a';
			double double_v = 7.7;

			long res_int_v;
			char res_char_v;
			double res_double_v;

			// Convert to bytes
			byte[] bytes_int_v = this.vm.Bytes.FromIntToBytes( int_v );
			byte[] bytes_char_v = this.vm.Bytes.FromCharToBytes( char_v );
			byte[] bytes_double_v = this.vm.Bytes.FromDoubleToBytes( double_v );

			// Convert back to values
			res_int_v = this.vm.Bytes.FromBytesToInt( bytes_int_v );
			res_char_v = this.vm.Bytes.FromBytesToChar( bytes_char_v );
			res_double_v = this.vm.Bytes.FromBytesToDouble( bytes_double_v );

			// Check the round-trip was succesful
			Assert.AreEqual( char_v, res_char_v );
			Assert.AreEqual( int_v, res_int_v );
			Assert.AreEqual( double_v, res_double_v );
		}

		[Test]
		public void Primitives()
		{
			Assert.IsNotNull( int_t );
			Assert.IsNotNull( char_t );
			Assert.IsNotNull( double_t );

			Assert.AreEqual( CSim.Core.Types.Primitives.Int.TypeName, int_t.Name );
			Assert.AreEqual( CSim.Core.Types.Primitives.Char.TypeName, char_t.Name );
			Assert.AreEqual( CSim.Core.Types.Primitives.Double.TypeName, double_t.Name );
		}

		[Test]
		public void Ptrs()
		{
			var int_pt = this.vm.TypeSystem.GetPtrType( this.int_t );
			var char_pt = this.vm.TypeSystem.GetPtrType( this.char_t );
			var double_pt = this.vm.TypeSystem.GetPtrType( this.double_t );

			Assert.IsNotNull( int_pt );
			Assert.IsNotNull( char_pt );
			Assert.IsNotNull( double_pt );

			Assert.AreEqual( CSim.Core.Types.Primitives.Int.TypeName + CSim.Core.Types.Ptr.PtrTypeNamePart, int_pt.Name );
			Assert.AreEqual( CSim.Core.Types.Primitives.Char.TypeName + CSim.Core.Types.Ptr.PtrTypeNamePart, char_pt.Name );
			Assert.AreEqual( CSim.Core.Types.Primitives.Double.TypeName + CSim.Core.Types.Ptr.PtrTypeNamePart, double_pt.Name );
		}

		[Test]
		public void Refs()
		{
			var int_rt = this.vm.TypeSystem.GetRefType( this.int_t );
			var char_rt = this.vm.TypeSystem.GetRefType( this.char_t );
			var double_rt = this.vm.TypeSystem.GetRefType( this.double_t );

			Assert.IsNotNull( int_rt );
			Assert.IsNotNull( char_rt );
			Assert.IsNotNull( double_rt );

			Assert.AreEqual( CSim.Core.Types.Primitives.Int.TypeName + CSim.Core.Types.Ref.RefTypeNamePart, int_rt.Name );
			Assert.AreEqual( CSim.Core.Types.Primitives.Char.TypeName + CSim.Core.Types.Ref.RefTypeNamePart, char_rt.Name );
			Assert.AreEqual( CSim.Core.Types.Primitives.Double.TypeName + CSim.Core.Types.Ref.RefTypeNamePart, double_rt.Name );
		}

		[Test]
		public void TypeRepresentation()
		{
			var bytes = new byte[] {
				this.vm.Bytes.FromTypeToBytes( this.any_t )[ 0 ],
				this.vm.Bytes.FromTypeToBytes( this.char_t )[ 0 ],
				this.vm.Bytes.FromTypeToBytes( this.int_t )[ 0 ],
				this.vm.Bytes.FromTypeToBytes( this.double_t )[ 0 ],
				this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetPtrType( this.any_t ) )[ 0 ],
				this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetPtrType( this.char_t ) )[ 0 ],
				this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetPtrType( this.int_t ) )[ 0 ],
				this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetPtrType( this.double_t ) )[ 0 ],
				this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetRefType( this.any_t ) )[ 0 ],
				this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetRefType( this.char_t ) )[ 0 ],
				this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetRefType( this.int_t ) )[ 0 ],
				this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetRefType( this.double_t ) )[ 0 ],
			};
			var types = new AType[] {
				this.any_t,
				this.char_t,
				this.int_t,
				this.double_t,
				this.vm.TypeSystem.GetPtrType( this.any_t ),
				this.vm.TypeSystem.GetPtrType( this.char_t ),
				this.vm.TypeSystem.GetPtrType( this.int_t ),
				this.vm.TypeSystem.GetPtrType( this.double_t )
			};

			for(int i = 0; i < types.Length; ++i) {
				var t = this.vm.Bytes.FromBytesToType( new []{ bytes[ i ] } );
				Assert.AreSame( types[ i ], t, t + " != " + types[ i ] );
			}
		}

		private Machine vm;
		private AType any_t;
		private AType int_t;
        private AType char_t;
        private AType double_t;
	}
}

