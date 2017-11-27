using System.Collections.Generic;
using System.Linq;
using GeneAutomate.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneAutomate.BDD.Tests
{
    public class TetParameters
    {
        public int CaseNumber { get; set; }
        public Condition FirstCondition { get; set; }

        public bool Expected_A_Value { get; set; }

        public bool IsValidPath { get; set; }
    }


    [TestClass]
    public class FunctionBddSolverTestsFuncNum3 : AbstractBddTest
    {

        public List<TetParameters> TetsParameters = new List<TetParameters>()
        {
            new TetParameters()
            {
                CaseNumber = 0,
                FirstCondition =  new Condition()
                {
                   { "b", false }, {"c", false }, {"d", false }, {"e" ,false }
                },
                Expected_A_Value = true,
                IsValidPath = false
            },
            new TetParameters()
            {
                CaseNumber = 1,
                FirstCondition =  new Condition()
                {
                   { "b", true }, {"c", false }, {"d", false },  { "e", false }
                },
                Expected_A_Value = true,
                IsValidPath = true
            },
            new TetParameters()
            {
                CaseNumber = 2,
                FirstCondition =  new Condition()
                {
                   { "b", true }, {"c", true }, {"d", false },  { "e", false }
                },
                Expected_A_Value = true,
                IsValidPath = true
            },
            new TetParameters()
            {
                CaseNumber = 3,
                FirstCondition =  new Condition()
                {
                   { "b", false }, {"c", false }, {"d", true },  { "e", false }
                },
                Expected_A_Value = true,
                IsValidPath = false
            },
            new TetParameters()
            {
                CaseNumber = 4,
                FirstCondition =  new Condition()
                {
                   { "b", true }, {"c", false }, {"d", true },  { "e", false }
                },
                Expected_A_Value = true,
                IsValidPath = false
            },
            new TetParameters()
            {
                CaseNumber = 5,
                FirstCondition =  new Condition()
                {
                   { "b", true }, {"c", true }, {"d", true },  { "e", false }
                },
                Expected_A_Value = true,
                IsValidPath = true
            },
            new TetParameters()
            {
                CaseNumber = 6,
                FirstCondition =  new Condition()
                {
                   { "b", false }, {"c", false }, {"d", true },  { "e", true }
                },
                Expected_A_Value = true,
                IsValidPath = false
            },
            new TetParameters()
            {
                CaseNumber = 7,
                FirstCondition =  new Condition()
                {
                   { "b", true }, {"c", false }, {"d", true },  { "e", true }
                },
                Expected_A_Value = true,
                IsValidPath = false
            },
            new TetParameters()
            {
                CaseNumber = 8,
                FirstCondition =  new Condition()
                {
                   { "b", true }, {"c", true }, {"d", true },  { "e", true }
                },
                Expected_A_Value = true,
                IsValidPath = false
            }
        };

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
            var solver = new BDDSolver();
            resultValues.ToList().ForEach((rv) =>
                {
                    TetsParameters.ForEach(tp =>
                    {
                        var functionNum = rv.Key;
                        var expectedValue = rv.Value[tp.CaseNumber];


                        logger.Info($" function num: {functionNum}, expectedValue : {expectedValue}, case number {tp.CaseNumber}");
                        var firstCondition = tp.FirstCondition;

                        var secondCondition = new Condition() { { "a", expectedValue == 1 } };

                        var automata = TestHelper.CreateAutomataWithConditions(firstCondition, secondCondition);

                        var booleanNetwork = CreateBooleanNetwork();

                        var availableFunctions = new Dictionary<string, List<int>>() { { "a", new List<int>() { functionNum } } };
                        var res = solver.IsValidPath(automata, booleanNetwork, availableFunctions);
                        Assert.IsTrue(res);
                    });
                }
            );
            
        }

        private static List<GeneLink> CreateBooleanNetwork()
        {
            return new List<GeneLink>()
            {
                new GeneLink() {From = "b", To = "a", IsPositive = true},
                new GeneLink() {From = "c", To = "a", IsPositive = true},
                new GeneLink() {From = "d", To = "a", IsPositive = false},
                new GeneLink() {From = "e", To = "a", IsPositive = false},

            };
        }

        [TestMethod]
        public void TestOrFunctionNegativeCase1()
        {
            var solver = new BDDSolver();

            var automata = new GeneNode()
            {
                CurrentCondition = new Condition() { { "a", false }, { "b", true } },
                NodeName = "n0",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            CurrentCondition = new Condition() {{"a", true}},
                            NodeName = "n1"
                        }
                    }
                }
            };

            var booleanNetwork = new List<GeneLink>()
            {
                new GeneLink() {From = "a", To = "a", IsPositive = true},
                new GeneLink() {From = "b", To = "a", IsPositive = true}
            };

            var availableFunctions = new Dictionary<string, List<int>>() { { "a", new List<int>() { 1 } } };
            var res = solver.IsValidPath(automata, booleanNetwork, availableFunctions);
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void TestOrFunctionNegativeCase2()
        {
            var solver = new BDDSolver();

            var automata = new GeneNode()
            {
                CurrentCondition = new Condition() { { "a", true }, { "b", false } },
                NodeName = "n0",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            CurrentCondition = new Condition() {{"a", true}},
                            NodeName = "n1"
                        }
                    }
                }
            };

            var booleanNetwork = new List<GeneLink>()
            {
                new GeneLink() {From = "a", To = "a", IsPositive = true},
                new GeneLink() {From = "b", To = "a", IsPositive = true},
            };

            var availableFunctions = new Dictionary<string, List<int>>() { { "a", new List<int>() { 1 } } };
            var res = solver.IsValidPath(automata, booleanNetwork, availableFunctions);
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void TestOrFunctionNegativeCase3()
        {
            var solver = new BDDSolver();

            var automata = new GeneNode()
            {
                CurrentCondition = new Condition() { { "a", false }, { "b", false } },
                NodeName = "n0",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            CurrentCondition = new Condition() {{"a", true}},
                            NodeName = "n1"
                        }
                    }
                }
            };

            var booleanNetwork = new List<GeneLink>()
            {
                new GeneLink() {From = "a", To = "a", IsPositive = true},
                new GeneLink() {From = "b", To = "a", IsPositive = true},
            };

            var availableFunctions = new Dictionary<string, List<int>>() { { "a", new List<int>() { 1 } } };
            var res = solver.IsValidPath(automata, booleanNetwork, availableFunctions);
            Assert.IsFalse(res);
        }
        
    }
}