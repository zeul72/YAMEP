namespace YAMEP.Tests {

    public abstract class UnitTestBase {

        protected const string SimpleExpression             = "1 + 2 - 3 * 4 / 5";
        protected const string SimpleExpression_NoSpaces    = "1+2-3*4/5";


        protected SourceScanner GetSourceScanner( string expression ) => new SourceScanner( expression );

    }
}
