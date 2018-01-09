using System.Collections.Generic;
using GeneAutomate.BusinessLogic;
using GeneAutomate.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneAutomate.BDD.Tests
{
    [TestClass]
    public class FunctionBddSolverAndOperatorTests : AbstractBddTest
    {
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            // FuncAssignmentHelper.dict.Add(47, (func) => func.AndPositiveIsTrue());

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

        [TestInitialize]
        public void Init()
        {
            logger.Info("start " + this.TestContext.TestName);
        }

        [TestMethod]
        public void TestAndSecondCase()
        {
            var firstCondition = new Condition()
            {
                { "b", false }, {"c", true }, {"d", false }, {"e" ,false }
            };

            RunSingle(firstCondition, false);
        }

        [TestMethod]
        public void TestAndThirdCase()
        {
            var firstCondition = new Condition()
            {
                { "b", true }, {"c", false }, {"d", false }, {"e" ,false }
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

        private static void RunSingle(Condition firstCondition, bool firstValue)
        {
            var solver = NinjectHelper.Get<IBDDSolver>();
            var secondCondition = new Condition() { { "a", firstValue } };

            var automata = TestHelper.CreateAutomataWithConditions(firstCondition, secondCondition);

            var booleanNetwork = CreateBooleanNetwork();

            var availableFunctions = new Dictionary<string, List<int>>() { { "a", new List<int>() { 47 } } };
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