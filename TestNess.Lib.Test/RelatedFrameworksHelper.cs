using TestNess.Lib.TestFramework;

namespace TestNess.Lib.Test
{
    public static class RelatedFrameworksHelper
    {
        public static RelatedFrameworks AsRelated(this ITestFramework tc)
        {
            return new RelatedFrameworks(tc);
        }
    }
}