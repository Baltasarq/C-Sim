using System;
using NUnit.Framework;

using CSim.Core;

namespace CSimTests {
	[TestFixture]
	public class TypeTests {

		[TestFixtureSetUp]
		public void Init()
		{
			this.vm = new Machine();

			this.int_t = this.vm.TypeSystem.GetIntType();
			this.char_t = this.vm.TypeSystem.GetCharType();
			this.double_t = this.vm.TypeSystem.GetDoubleType();
		}

		[Test]
		public void ByteConversion()
		{
			int int_v = 5;
			char char_v = 'a';
			double double_v = 7.7;

			int res_int_v;
			char res_char_v;
			double res_double_v;

			// Convert to bytes
			byte[] bytes_int_v = this.vm.CnvtIntToBytes( int_v );
			byte[] bytes_char_v = this.vm.CnvtCharToBytes( char_v );
			byte[] bytes_double_v = this.vm.CnvtDoubleToBytes( double_v );

			// Convert back to values
			res_int_v = this.vm.CnvtBytesToInt( bytes_int_v );
			res_char_v = this.vm.CnvtBytesToChar( bytes_char_v );
			res_double_v = this.vm.CnvtBytesToDouble( bytes_double_v );

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

		private Machine vm;
		private CSim.Core.Type int_t;
		private CSim.Core.Type char_t;
		private CSim.Core.Type double_t;
	}
}

