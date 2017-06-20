using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeneAutomate.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneAutomate.Parser.Tests
{
    [TestClass()]
    public class RuleFileParserTests
    {
        [TestMethod()]
        public void TestCanReadToysSpec()
        {
            var parser = new RuleFileParser();

            parser.ParseRules(@"C:\Users\ygalbe\Dropbox\Automate\toy.spec", new List<string>() {"S1"});
        }
    }
}