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
    public class TestParameters
    {
        public int CaseNumber { get; set; }
        public Condition FirstCondition { get; set; }

        public bool Expected_A_Value { get; set; }

        public bool IsValidPath { get; set; }
    }

    [TestClass]
    public class FunctionBddOrOperatorTests : AbstractBddTest
    {
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            FuncAssignmentHelper.dict.Add(44, (func) => func.OrNegativeIsTrue());
            FuncAssignmentHelper.dict.Add(45, (func) => func.OrPositiveIsTrue());

        }

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
            var solver = NinjectHelper.Get<IBDDSolver>();
            var secondCondition = new Condition() {{"a", firstValue } };

            var automata = TestHelper.CreateAutomataWithConditions(firstCondition, secondCondition);

            var booleanNetwork = CreateBooleanNetwork();

            var availableFunctions = new Dictionary<string, List<int>>() {{"a", new List<int>() {44}}};
            var res = solver.IsValidPath(automata, booleanNetwork, availableFunctions);

            Assert.IsTrue(res);

            secondCondition = new Condition() {{"a", !firstValue}};
            automata = TestHelper.CreateAutomataWithConditions(firstCondition, secondCondition);
            res = solver.IsValidPath(automata, booleanNetwork, availableFunctions);

            Assert.IsFalse(res);
        }
    }


    [TestClass]
    public class FunctionBddAndOperatorTests : AbstractBddTest
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
                   { "b", false }, {"c", false }
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


    [TestClass]
    public class FunctionBddSolverTestsComplexFunctions : AbstractBddTest
    {

        private Dictionary<int, List<int>> resultValues = new Dictionary<int, List<int>>()
        {
            {0, new List<int>()  {0, 0, 1, 0, 0, 0, 0, 0, 0}},
            {1, new List<int>()  {0, 1, 1, 0, 0, 0, 0, 0, 0}},
            {2, new List<int>()  {0, 0, 1, 0, 0, 1, 0, 0, 0}},
            {3, new List<int>()  {0, 1, 1, 0, 0, 1, 0, 0, 0}},
            {4, new List<int>()  {0, 0, 1, 0, 0, 1, 0, 0, 1}},
            {5, new List<int>()  {0, 1, 1, 0, 0, 1, 0, 0, 1}},
            {6, new List<int>()  {0, 1, 1, 0, 1, 1, 0, 0, 0}},
            {7, new List<int>()  {0, 1, 1, 0, 1, 1, 0, 0, 1}},
            {8, new List<int>()  {0, 1, 1, 0, 1, 1, 0, 1, 1}},
            {9, new List<int>()  {1, 1, 1, 0, 0, 0, 0, 0, 0}},
            {10, new List<int>() {1, 1, 1, 0, 0, 1, 0, 0, 0}},
            {11, new List<int>() {1, 1, 1, 0, 1, 1, 0, 0, 0}},
            {12, new List<int>() {1, 1, 1, 1, 1, 1, 0, 0, 0}},
            {13, new List<int>() {1, 1, 1, 0, 0, 1, 0, 0, 1}},
            {14, new List<int>() {1, 1, 1, 0, 1, 1, 0, 0, 1}},
            {15, new List<int>() {1, 1, 1, 1, 1, 1, 0, 0, 1}},
            {16, new List<int>() {1, 1, 1, 0, 1, 1, 0, 1, 1}},
            {17, new List<int>() {1, 1, 1, 1, 1, 1, 0, 1, 1}},
        };

        [TestInitialize]
        public void TestInit()
        {
            base.TestInit();
        }



        [TestMethod]
        public void TestFullCaseNumber()
        {
            var resultValues = this.resultValues;
            RunPositiveTest(resultValues);
        }

        


        [TestMethod]
        public void TestFullCaseNumberNegative()
        {
            RunNegativeTest(resultValues);
        }

        

        
        
    }

    

    [TestClass]
    public class FunctionBddSolverTestsSimpleFunctions : AbstractBddTest
    {


        private Dictionary<int, List<int>> resultValues = new Dictionary<int, List<int>>()
        {
            {-1, new List<int>()  {0, 0, 1, 0, 0, 1, 0, 0, 1}}, // all Activators
            {-2, new List<int>()  {1, 1, 1, 0, 0, 0, 0, 0, 0}}, // no repressesors
            {-3, new List<int>()  {0, 1, 1, 0, 1, 1, 0, 1, 1}}, // not no activators
            {-4, new List<int>()  {1, 1, 1, 1, 1, 1, 0, 0, 0}},// not all repressesors
        };

        [TestInitialize]
        public void TestInit()
        {
            base.TestInit();
        }



        [TestMethod]
        public void TestFullCaseNumber()
        {
            RunPositiveTest(resultValues);
        }




        [TestMethod]
        public void TestFullCaseNumberNegative()
        {
            RunNegativeTest(resultValues);
        }



    }

}