using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneAutomate.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneAutomate.ParserTests
{
    [TestClass]
    public class FullFileParserTests
    {
        [TestMethod]
        public void TestCanParseToyCase()
        {
            var parser = new FileParser();

            var data = new ParseRuleResponse();
            var res = parser.ParseFiles($"toy_changed.net", $"toy_changed.spec");

            Assert.IsTrue(res.Merges.Count > 0);
            
        }


        [TestMethod]
        public void TestCanParseToys2()
        {
            var parser = new FileParser();

            var data = new ParseRuleResponse();
            var res = parser.ParseFiles($"toy_changed2.net", $"toy_changed2.spec");

            Assert.IsTrue(res.Merges.Count > 0);

        }
    }
}
