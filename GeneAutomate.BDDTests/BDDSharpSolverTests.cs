using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeneAutomate.BDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneAutomate.BDD.Tests
{
    [TestClass()]
    public class BDDSharpSolverCreateFunctionKeysTests
    {
        [TestMethod()]
        public void CreateFunctionsKeysTestSimpleCase()
        {
            //Act
            var res = BDDSharpSolver.CreateFunctionsKeys(new Dictionary<string, List<int>>()
            {
                {"a", new List<int>() {1, 2, 3}}
            });

            // Assert
            Assert.AreEqual(res.Count, 1);
            Assert.AreEqual(res["a"].Count, 3);
            Assert.IsTrue(res["a"][0] == "#F1_a");
            Assert.IsTrue(res["a"][1] == "#F2_a");
            Assert.IsTrue(res["a"][2] == "#F3_a");
        }

        [TestMethod()]
        public void CreateFunctionsKeysTestSecondCase()
        {
            var res = BDDSharpSolver.CreateFunctionsKeys(new Dictionary<string, List<int>>()
            {
                {"a", new List<int>() {1, 2, 3}},
                {"b", new List<int>() {1, 2, 3}}
            });


            Assert.AreEqual(res.Count, 2);
            Assert.IsTrue(res["a"].Any(a => a == "#F1_a"));
            Assert.IsTrue(res["a"].Any(a => a == "#F2_a"));
            Assert.IsTrue(res["a"].Any(a => a == "#F3_a"));
            Assert.IsTrue(res["b"].Any(a => a == "#F1_b"));
            Assert.IsTrue(res["b"].Any(a => a == "#F2_b"));
            Assert.IsTrue(res["b"].Any(a => a == "#F3_b"));
        }
    }
}