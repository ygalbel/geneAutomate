using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;

namespace GeneAutomate.ParserTests
{
    [TestClass]
    public class LoggerTests
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [TestMethod]
        public void TestLogger()
        {
            logger.Info("Test");
        }
    }
}