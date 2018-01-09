using System.Collections.Generic;
using GeneAutomate.BusinessLogic;
using GeneAutomate.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneAutomate.BDD.Tests
{
    /// <summary>
    /// B and E, are optional
    /// </summary>
    [TestClass]
    public class FunctionBddSolverAndOptionalRelationOperatorTests : AbstractBddTest
    {
        [TestInitialize]
        public void Init()
        {
            logger.Info("start " + this.TestContext.TestName);
        }

        [TestMethod]
        public void TestAndFirstCase()
        {
            var firstCondition = new Condition()
            {
                { "b", false }, {"c", false }, {"d", false }, {"e" ,false }
            };

            RunSingle(firstCondition, false);
        }

        [TestMethod]
        public void TestAndSecondCase()
        {
            // b is optional, so both pathes are valid
            var firstCondition = new Condition()
            {
                { "b", true }, {"c", false }, {"d", false }, {"e" ,false }
            };

            RunBoth(firstCondition, true, true, true);
        }

        [TestMethod]
        public void TestAndThirdCase()
        {
            var firstCondition = new Condition()
            {
                { "b", false }, {"c", true }, {"d", false }, {"e" ,false }
            };

            RunSingle(firstCondition, false);
        }

        [TestMethod]
        public void TestAndFourthCase()
        {
            var firstCondition = new Condition()
            {
                { "b", true }, {"c", true }, {"d", false }, {"e" ,false }
            };

            RunSingle(firstCondition, true);
        }

        private static void RunBoth(Condition firstCondition, bool firstValue, bool firstHavePath, bool secondHavePath)
        {
            var solver = NinjectHelper.Get<IBDDSolver>();
            var secondCondition = new Condition() { { "a", firstValue } };
            var automata = TestHelper.CreateAutomataWithConditions(firstCondition, secondCondition);
            var booleanNetwork = CreateBooleanNetworkWithOptional();
            var availableFunctions = new Dictionary<string, List<int>>()
                { {"a", new List<int>() {47}}};

            var res = solver.IsValidPath(automata, booleanNetwork, availableFunctions);

            Assert.AreEqual(res, firstHavePath);

            secondCondition = new Condition() { { "a", !firstValue } };
            automata = TestHelper.CreateAutomataWithConditions(firstCondition, secondCondition);

            res = solver.IsValidPath(automata, booleanNetwork, availableFunctions);

            Assert.AreEqual(res, secondHavePath);
        }

        private static void RunSingle(Condition firstCondition, bool firstValue)
        {
            var solver = NinjectHelper.Get<IBDDSolver>();
            var secondCondition = new Condition() { { "a", firstValue } };

            var automata = 
                TestHelper.CreateAutomataWithConditions(
                    firstCondition, secondCondition);

            var booleanNetwork = CreateBooleanNetworkWithOptional();

            var availableFunctions = 
                new Dictionary<string, List<int>>() { { "a", new List<int>() { 47 } } };
            var res = solver.IsValidPath(automata, booleanNetwork, availableFunctions);

            Assert.IsTrue(res);

            secondCondition = new Condition() { { "a", !firstValue } };
            automata = TestHelper.CreateAutomataWithConditions(firstCondition, secondCondition);
            solver = NinjectHelper.Get<IBDDSolver>();
            res = solver.IsValidPath(automata, booleanNetwork, availableFunctions);

            Assert.IsFalse(res);
        }
    }
}