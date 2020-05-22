using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YAMEP.Tests {


    [TestClass( )]
    public class SymbolTableTests {
    
        [TestMethod( )]
        public void AddOrUpdateTest( ) {

            var variables = new { x = 5, y = 10};
            var st = new SymbolTable();

            st.AddOrUpdate( variables );

            Assert.IsNotNull( st.Get( "x" ) );
            Assert.IsNotNull( st.Get( "y" ) );

            Assert.AreEqual( 5, ( st.Get( "x" ) as VariableSymbolTableEntry ).Value );
            Assert.AreEqual( 10, ( st.Get( "y" ) as VariableSymbolTableEntry ).Value );

        }
    }
}