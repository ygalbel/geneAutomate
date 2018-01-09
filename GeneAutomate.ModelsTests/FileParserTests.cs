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
    public class FileParserTests
    {
        [TestMethod()]
        public void FunctionParserLogicTest()
        {
            var res = FileParser.FunctionParserLogic("C[](1,3,5); B[](0..8); A[](0..8); S2[](0); S1[](0);");
            Assert.AreEqual(res.Keys.Count,5);
            Assert.AreEqual(res["C"].Count,3);
            Assert.AreEqual(res["B"].Count, 9);
            Assert.AreEqual(res["A"].Count, 9);
            Assert.AreEqual(res["S2"].Count, 1);
            Assert.AreEqual(res["S1"].Count, 1);
        }

        [TestMethod()]
        public void FunctionParserPluriTest()
        {
            var res = FileParser.FunctionParserLogic(
                "Tcf3[](16, 17); MEKERK[](16, 17); Oct4[-](0..15); Sall4[](0..15); Sox2[-](0..15); Tbx3[](0..15); Klf2[](0..15); Nanog[-](0..15); Esrrb[-+](0..15); Tfcp2l1[+](0..15); Gbx2[](0..15); Klf4[](0..15); Stat3[-](0..15); PD[](0..15); CH[](0..15); LIF[](0..15);");
            Assert.AreEqual(res.Keys.Count, 16);
        }
    }
}