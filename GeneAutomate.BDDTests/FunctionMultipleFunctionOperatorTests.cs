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
    public class FunctionMultipleFunctionOperatorTests : AbstractBddTest
    {
        [TestMethod]
        public void TestAndOrFirstCase()
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
        public void TestAndOrSecondCase()
        {
            var firstCondition = new Condition()
                {
                   { "b", false }, {"c", false }, {"d", true }, {"e" ,false }
                };

            RunSingle(firstCondition, false);
        }

        [TestMethod]
        public void TestAndOrThirdCase()
        {
            var firstCondition = new Condition()
                {
                   { "b", false }, {"c", false }, {"d", false }, {"e" ,true }
                };

            RunSingle(firstCondition, false);
        }

        [TestMethod]
        public void TestAndOrFourthCase()
        {
            var firstCondition = new Condition()
                {
                   { "b", false }, {"c", false }, {"d", true }, {"e" ,true }
                };

            RunSingle(firstCondition, false);
        }

        [TestMethod]
        public void TestAndOr5Case()
        {
            var firstCondition = new Condition()
            {
                { "b", true }, {"c", false }, {"d", true }, {"e" ,true }
            };

            // This test depend and the two functions.
            // if is AND -> it's false
            // But if it's OR -> it's true
            RunBoth(firstCondition, true, true, true);
        }
        [TestMethod]
        public void TestAndOr6Case()
        {
            var firstCondition = new Condition()
            {
                { "b", false }, {"c", true}, {"d", true }, {"e" ,true }
            };

            // This test depend and the two functions.
            // if is AND -> it's false
            // But if it's OR -> it's true
            RunBoth(firstCondition, true, true, true);
        }

        [TestMethod]
        public void TestAndOr7Case()
        {
            var firstCondition = new Condition()
            {
                { "b", true}, {"c", true}, {"d", true }, {"e" ,true }
            };

            RunSingle(firstCondition, true);
        }

        private static void RunSingle(Condition firstCondition, bool firstValue)
        {
            RunBoth(firstCondition, firstValue, true, false);
        }

        private static void RunBoth(Condition firstCondition, bool firstValue, bool firstHavePath, bool secondHavePath)
        {
            var solver = NinjectHelper.Get<IBDDSolver>();
            var secondCondition = new Condition() {{"a", firstValue } };
            var automata = TestHelper.CreateAutomataWithConditions(firstCondition, secondCondition);
            var booleanNetwork = CreateBooleanNetwork();
            var availableFunctions = new Dictionary<string, List<int>>()
                { {"a", new List<int>() {45, 47}}};

            var res = solver.IsValidPath(automata,
                new GeneFullRules() {GeneLinks = booleanNetwork, Functions = availableFunctions});

            Assert.AreEqual(res, firstHavePath);

            secondCondition = new Condition() {{"a", !firstValue}};
            automata = TestHelper.CreateAutomataWithConditions(firstCondition, secondCondition);

            res = solver.IsValidPath(automata,
                new GeneFullRules() {GeneLinks = booleanNetwork, Functions = availableFunctions});

            Assert.AreEqual(res, secondHavePath);
        }
    }

}