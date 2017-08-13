using System.Web;

namespace GeneAutomate.Controllers
{
    public class PathHelper
    {
        public static string GetSpecPath(string exampleName)
        {
            return $"{HttpRuntime.AppDomainAppPath}/Examples/{exampleName}.spec";
        }

        public static string GetExamplePath(string exampleName)
        {
            return $"{HttpRuntime.AppDomainAppPath}/Examples/{exampleName}.net";
        }
    }
}