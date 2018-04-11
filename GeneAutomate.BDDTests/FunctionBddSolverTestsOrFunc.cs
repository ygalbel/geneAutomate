using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeneAutomate.BusinessLogic;
using GeneAutomate.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace GeneAutomate.BDD.Tests
{
    [TestClass]
    public class FunctionBddOrOperatorTests : AbstractBddTest
    {
        [TestMethod]
        public void TestOrFirstCase()
        {
            var firstCondition  = new Condition()
                {
                   { "b", false }, {"c", false }, {"d", false }, {"e" ,false }
                };

            RunSingle(firstCondition, false);
        }

        [TestInitialize]
        public void Init()
        {
            logger.Info("start " + this.TestContext.TestName);
        }

        [TestMethod]
        public void TestOrSecondCase()
        {
            var firstCondition = new Condition()
                {
                   { "b", false }, {"c", false }, {"d", true }, {"e" ,false }
                };

            RunSingle(firstCondition, true);
        }

        [TestMethod]
        public void TestOrThirdCase()
        {
            var firstCondition = new Condition()
                {
                   { "b", false }, {"c", false }, {"d", false }, {"e" ,true }
                };

            RunSingle(firstCondition, true);
        }

        [TestMethod]
        public void TestOrFourthCase()
        {
            var firstCondition = new Condition()
                {
                   { "b", false }, {"c", false }, {"d", true }, {"e" ,true }
                };

            RunSingle(firstCondition, true);
        }

        private static void RunSingle(Condition firstCondition, bool firstValue)
        {
            var solver = new BDDSharpSolver();
            var secondCondition = new Condition() {{"a", firstValue } };

            var automata = TestHelper.CreateAutomataWithConditions(firstCondition, secondCondition);

            var booleanNetwork = CreateBooleanNetwork();

            var availableFunctions = new Dictionary<string, List<int>>() {{"a", new List<int>() {44}}};
            var res = solver.IsValidPath(automata,
                new GeneFullRules() { GeneLinks = booleanNetwork, Functions = availableFunctions }, 5);

            Assert.IsTrue(res);

            solver = new BDDSharpSolver();
            secondCondition = new Condition() {{"a", !firstValue}};
            automata = TestHelper.CreateAutomataWithConditions(firstCondition, secondCondition);
            res = solver.IsValidPath(automata,
                new GeneFullRules() { GeneLinks = booleanNetwork, Functions = availableFunctions },5);

            Assert.IsFalse(res);
        }
    }
}