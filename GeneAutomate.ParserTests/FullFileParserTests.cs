using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneAutomate.Parser;
using GeneAutomate.Writer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;

namespace GeneAutomate.ParserTests
{
    [TestClass]
    public class FullFileParserTests
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        [TestMethod]
        public void TestCanParseToyChangedCase()
        {
            var parser = new FileParser();

            var data = new ParseRuleResponse();
            var res = parser.ParseFiles($"toy_changed.net", $"toy_changed.spec");

            Assert.IsTrue(res.Merges.Count > 0);

            
        }


        [TestMethod]
        public void TestCanParseToyOriginalCase()
        {
            var parser = new FileParser();

            var data = new ParseRuleResponse();
            var res = parser.ParseFiles($"toy.net", $"toy.spec");
            Assert.IsTrue(res.MergeObjects.Count > 0);
            Assert.IsTrue(res.MergeObjects.All(a => a.MergeName.Contains("^")));

            var txt = new RuleFileWriter().CreateSpecString(res.MergeObjects);

            logger.Info(txt);

        }

        [TestMethod]
        public void TestCanParsePluripotencySmallCase()
        {
            var parser = new FileParser();
            var data = new ParseRuleResponse();
            var res = parser.ParseFiles($"pluripotency_small.net", $"pluripotency_small.spec");

            Assert.IsTrue(res.MergeObjects.Count > 0);

            var txt = new RuleFileWriter().CreateSpecString(res.MergeObjects);

            logger.Info(txt);

        }

        [TestMethod]
        public void TestCanParsePluripotencyCase()
        {
            var parser = new FileParser();
            var data = new ParseRuleResponse();
            var res = parser.ParseFiles($"pluripotency.net", $"pluripotency.spec");

            Assert.IsTrue(res.MergeObjects.Count > 0);

            var txt = new RuleFileWriter().CreateSpecString(res.MergeObjects);

            logger.Info(txt);

        }

        [Ignore]
        [TestMethod]
        public void TestCanParseToys2()
        {
            var parser = new FileParser();

            var data = new ParseRuleResponse();
            var res = parser.ParseFiles($"toy_changed2.net", $"toy_changed2.spec");

            Assert.IsTrue(res.Merges.Count > 0);

        }


        [TestMethod]
        public void TestCanParsesimple1()
        {
            var parser = new FileParser();

            var data = new ParseRuleResponse();
            var res = parser.ParseFiles($"simple1.net", $"simple1.spec");

            Assert.IsTrue(res.Merges.Count > 0);

        }
    }
}
