// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSimTests {
    using System.Numerics;
	using NUnit.Framework;
    
	using CSim.Core;
    using CSim.Core.Types;
	using CSim.Core.Types.Primitives;

	[TestFixture]
	public class TypeTests {

		[TestFixtureSetUp]
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
			BigInteger int_v = 5;
			char char_v = 'a';
			double double_v = 7.7;

			BigInteger res_int_v;
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
            // Char
            Assert.IsNotNull( this.char_t );
            Assert.AreEqual( Char.TypeName, char_t.Name );
            Assert.AreEqual( Char.TypeName, char_t.Name );
            Assert.AreEqual( 1, char_t.Size );
            
            // Int8
            var int8_t = this.vm.TypeSystem.GetBasicType( Int8.TypeName );
            Assert.IsNotNull( int8_t );
            Assert.AreEqual( Int8.TypeName, int8_t.Name );
            Assert.AreEqual( 1, Int8.LengthInBytes );
            Assert.AreEqual( Int8.LengthInBytes, int8_t.Size );
            
            // UInt8
            var uint8_t = this.vm.TypeSystem.GetBasicType( UInt8.TypeName );
            Assert.IsNotNull( uint8_t );
            Assert.AreEqual( UInt8.TypeName, uint8_t.Name );
            Assert.AreEqual( 1, UInt8.LengthInBytes );
            Assert.AreEqual( UInt8.LengthInBytes, uint8_t.Size );
            
            // Int16
            var int16_t = this.vm.TypeSystem.GetBasicType( Int16.TypeName );
            Assert.IsNotNull( int16_t );
            Assert.AreEqual( Int16.TypeName, int16_t.Name );
            Assert.AreEqual( 2, Int16.LengthInBytes );
            Assert.AreEqual( Int16.LengthInBytes, int16_t.Size );
            
            // UInt16
            var uint16_t = this.vm.TypeSystem.GetBasicType( UInt16.TypeName );
            Assert.IsNotNull( uint16_t );
            Assert.AreEqual( UInt16.TypeName, uint16_t.Name );
            Assert.AreEqual( 2, UInt16.LengthInBytes );
            Assert.AreEqual( UInt16.LengthInBytes, uint16_t.Size );
            
            // Int32
            var int32_t = this.vm.TypeSystem.GetBasicType( Int32.TypeName );
            Assert.IsNotNull( int32_t );
            Assert.AreEqual( Int32.TypeName, int32_t.Name );
            Assert.AreEqual( 4, Int32.LengthInBytes );
            Assert.AreEqual( Int32.LengthInBytes, int32_t.Size );
            
            // UInt32
            var uint32_t = this.vm.TypeSystem.GetBasicType( UInt32.TypeName );
            Assert.IsNotNull( uint32_t );
            Assert.AreEqual( UInt32.TypeName, uint32_t.Name );
            Assert.AreEqual( 4, UInt32.LengthInBytes );
            Assert.AreEqual( UInt32.LengthInBytes, uint32_t.Size );
            
            // Int32
            var int64_t = this.vm.TypeSystem.GetBasicType( Int64.TypeName );
            Assert.IsNotNull( int64_t );
            Assert.AreEqual( Int64.TypeName, int64_t.Name );
            Assert.AreEqual( 8, Int64.LengthInBytes );
            Assert.AreEqual( Int64.LengthInBytes, int64_t.Size );
            
            // UInt32
            var uint64_t = this.vm.TypeSystem.GetBasicType( UInt64.TypeName );
            Assert.IsNotNull( uint64_t );
            Assert.AreEqual( UInt64.TypeName, uint64_t.Name );
            Assert.AreEqual( 8, UInt64.LengthInBytes );
            Assert.AreEqual( UInt64.LengthInBytes, uint64_t.Size );
            
            // Short
            var short_t = this.vm.TypeSystem.GetBasicType( Short.TypeName );
            Assert.IsNotNull( short_t );
            Assert.AreEqual( Short.TypeName, short_t.Name );
            Assert.AreEqual( this.vm.WordSize >> 1, short_t.Size );
            
            // UShort
            var ushort_t = this.vm.TypeSystem.GetBasicType( UShort.TypeName );
            Assert.IsNotNull( ushort_t );
            Assert.AreEqual( UShort.TypeName, ushort_t.Name );
            Assert.AreEqual( this.vm.WordSize >> 1, ushort_t.Size );
            
            // Int
			Assert.IsNotNull( this.int_t );
            Assert.AreEqual( Int.TypeName, int_t.Name );
            Assert.AreEqual( this.vm.WordSize, this.int_t.Size );
            
            // UInt
            var uint_t = this.vm.TypeSystem.GetBasicType( UInt.TypeName );
            Assert.IsNotNull( uint_t );
            Assert.AreEqual( UInt.TypeName, uint_t.Name );
            Assert.AreEqual( this.vm.WordSize, uint_t.Size );
            
            // Long
            var long_t = this.vm.TypeSystem.GetBasicType( Long.TypeName );
            Assert.IsNotNull( long_t );
            Assert.AreEqual( Long.TypeName, long_t.Name );
            Assert.AreEqual( this.vm.WordSize << 1, long_t.Size );
            
            // ULong
            var ulong_t = this.vm.TypeSystem.GetBasicType( ULong.TypeName );
            Assert.IsNotNull( ulong_t );
            Assert.AreEqual( ULong.TypeName, ulong_t.Name );
            Assert.AreEqual( this.vm.WordSize << 1, ulong_t.Size );
            
            // Float
            var float_t = this.vm.TypeSystem.GetBasicType( Float.TypeName );
            Assert.IsNotNull( float_t );
            Assert.AreEqual( Float.TypeName, float_t.Name );
            Assert.AreEqual( this.vm.WordSize, float_t.Size );
            
            // Double
            Assert.IsNotNull( double_t );
			Assert.AreEqual( Double.TypeName, double_t.Name );
            Assert.AreEqual( this.vm.WordSize << 1, this.double_t.Size );
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
		public void TypeByteRepresentation()
		{
			var bytes = new byte[] {
				this.vm.Bytes.FromTypeToBytes( this.any_t )[ 0 ],
				this.vm.Bytes.FromTypeToBytes( this.char_t )[ 0 ],
                this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetBasicType( Int8.TypeName ) )[ 0 ],
                this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetBasicType( UInt8.TypeName ) )[ 0 ],
                this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetBasicType( Short.TypeName ) )[ 0 ],
                this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetBasicType( UShort.TypeName ) )[ 0 ],
                this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetBasicType( Int16.TypeName ) )[ 0 ],
                this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetBasicType( UInt16.TypeName ) )[ 0 ],
                this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetBasicType( Int32.TypeName ) )[ 0 ],
                this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetBasicType( UInt32.TypeName ) )[ 0 ],
				this.vm.Bytes.FromTypeToBytes( this.int_t )[ 0 ],
                this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetBasicType( UInt.TypeName ) )[ 0 ],
                this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetBasicType( Int64.TypeName ) )[ 0 ],
                this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetBasicType( UInt64.TypeName ) )[ 0 ],
                this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetBasicType( Long.TypeName ) )[ 0 ],
                this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetBasicType( ULong.TypeName ) )[ 0 ],
                this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetBasicType( Float.TypeName ) )[ 0 ],
				this.vm.Bytes.FromTypeToBytes( this.double_t )[ 0 ],
				this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetPtrType( this.any_t ) )[ 0 ],
				this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetPtrType( this.char_t ) )[ 0 ],
				this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetPtrType( this.int_t ) )[ 0 ],
				this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetPtrType( this.double_t ) )[ 0 ],
				this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetRefType( this.any_t ) )[ 0 ],
				this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetRefType( this.char_t ) )[ 0 ],
				this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetRefType( this.int_t ) )[ 0 ],
				this.vm.Bytes.FromTypeToBytes( this.vm.TypeSystem.GetRefType( this.double_t ) )[ 0 ]
			};
			var types = new AType[] {
				this.any_t,
				this.char_t,
                Int8.Get( this.vm ),
                UInt8.Get( this.vm ),
                Short.Get( this.vm ),
                UShort.Get( this.vm ),
                Int16.Get( this.vm ),
                UInt16.Get( this.vm ),
                Int32.Get( this.vm ),
                UInt32.Get( this.vm ),
				this.int_t,
                UInt.Get( this.vm ),
                Int64.Get( this.vm ),
                UInt64.Get( this.vm ),
                Long.Get( this.vm ),
                ULong.Get( this.vm ),
                Float.Get( this.vm ),
				this.double_t,
				this.vm.TypeSystem.GetPtrType( this.any_t ),
				this.vm.TypeSystem.GetPtrType( this.char_t ),
				this.vm.TypeSystem.GetPtrType( this.int_t ),
				this.vm.TypeSystem.GetPtrType( this.double_t ),
                this.vm.TypeSystem.GetRefType( this.any_t ),
                this.vm.TypeSystem.GetRefType( this.char_t ),
                this.vm.TypeSystem.GetRefType( this.int_t ),
                this.vm.TypeSystem.GetRefType( this.double_t )
			};

			for(int i = 0; i < types.Length; ++i) {
				var t = this.vm.Bytes.FromBytesToType( new []{ bytes[ i ] } );
				Assert.AreSame( types[ i ], t, t + " != " + types[ i ] );
			}
		}
        
        [Test]
        public void TypeStringRepresentation()
        {
            Variable type_vble = this.vm.Execute( @"type_t t;" );
            Variable int_vble = this.vm.Execute( @"int x;" );
            Variable char_vble = this.vm.Execute( @"char ch;" );
            Variable double_vble = this.vm.Execute( @"double d;" );

            Variable int_ptr_vble = this.vm.Execute( @"int * ptrInt;" );
            Variable char_ptr_vble = this.vm.Execute( @"char * ptrChar;" );
            Variable double_ptr_vble = this.vm.Execute( @"double * ptrDouble;" );
            
            Assert.AreSame( type_vble.Type, this.vm.TypeSystem.GetTypeType() );            
            Assert.AreSame( int_vble.Type, this.int_t, "Created vble is not: int" );
            Assert.AreSame( char_vble.Type, this.char_t, "Created vble is not: char" );
            Assert.AreSame( double_vble.Type, this.double_t, "Created vble is not: double" );
            
            Assert.AreSame( int_ptr_vble.Type, this.vm.TypeSystem.GetPtrType( this.int_t ), "Created vble is not: int *" );
            Assert.AreSame( char_ptr_vble.Type, this.vm.TypeSystem.GetPtrType( this.char_t ), "Created vble is not: char *" );
            Assert.AreSame( double_ptr_vble.Type, this.vm.TypeSystem.GetPtrType( this.double_t ), "Created vble is not: double *" );
        }

		private Machine vm;
		private AType any_t;
		private AType int_t;
        private AType char_t;
        private AType double_t;
	}
}

