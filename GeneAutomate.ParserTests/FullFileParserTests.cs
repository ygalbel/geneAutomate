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
            var res = parser.ParseFiles($"toy.net", $"toy.spec");

        }
    }
}
