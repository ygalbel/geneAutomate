using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneAutomate.Parser;
using GeneAutomate.Writer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneAutomate.ParserTests
{
    [TestClass]
    public class FullFileParserTests
    {
        [TestMethod]
        public void TestCanParseToyChangedCase()
        {
            var parser = new FileParser();

            var data = new ParseRuleResponse();
            var res = parser.ParseFiles($"toy_changed.net", $"toy_changed.spec");

            Assert.IsTrue(res.Merges.Count > 0);

            
        }


        [TestMethod] public void TestCanParseToyOriginalCase()
        {
            var parser = new FileParser();

            var data = new ParseRuleResponse();
            var res = parser.ParseFiles($"toy.net", $"toy.spec");

            

            Assert.IsTrue(res.MergeObjects.Count > 0);

            var txt = new RuleFileWriter().CreateSpecString(res.MergeObjects);

            Trace.WriteLine(txt);

        }

        [TestMethod]
        public void TestCanParsePluripotencyOriginalCase()
        {
            var parser = new FileParser();

            var data = new ParseRuleResponse();
            var res = parser.ParseFiles($"pluripotency.net", $"pluripotency.spec");



            Assert.IsTrue(res.MergeObjects.Count > 0);

            var txt = new RuleFileWriter().CreateSpecString(res.MergeObjects);

            Trace.WriteLine(txt);

        }

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
