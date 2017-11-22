using System.Collections.Generic;
using GeneAutomate.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneAutomate.BDD.Tests
{
    [TestClass]
    public class FunctionBddSolverTestsAndFunc : AbstractBddTest
    {
        [TestInitialize]
        public void TestInit()
        {
            base.TestInit();
        }


        [TestMethod]
        public void TestAndFunctionPositive()
        {
            var solver = new BDDSolver();

            var automata = new GeneNode()
            {
                CurrentCondition = new Condition() { { "a", true }, { "b", true } },
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

            var availableFunctions = new Dictionary<string, List<int>>() { { "a", new List<int>() { 0 } } };
            var res = solver.IsValidPath(automata, booleanNetwork, availableFunctions);
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void TestAndFunctionNegativeCase1()
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

            var availableFunctions = new Dictionary<string, List<int>>() { { "a", new List<int>() { 0 } } };
            var res = solver.IsValidPath(automata, booleanNetwork, availableFunctions);
            Assert.IsFalse(res);
        }

        [TestMethod]
        public void TestAndFunctionNegativeCase2()
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

            var availableFunctions = new Dictionary<string, List<int>>() { { "a", new List<int>() { 0 } } };
            var res = solver.IsValidPath(automata, booleanNetwork, availableFunctions);
            Assert.IsFalse(res);
        }

        [TestMethod]
        public void TestAndFunctionNegativeCase3()
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

            var availableFunctions = new Dictionary<string, List<int>>() { { "a", new List<int>() { 0 } } };
            var res = solver.IsValidPath(automata, booleanNetwork, availableFunctions);
            Assert.IsFalse(res);
        }


        [TestMethod]
        public void TestAndFunctionPositiveTime2()
        {
            var solver = new BDDSolver();

            var automata = new GeneNode()
            {
                CurrentCondition = new Condition() { { "a", true }, { "b", true } },
                NodeName = "n0",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            CurrentCondition = new Condition() {{"a", true}, {"b", true} },
                            NodeName = "n1",
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition()
                                {
                                    Node = new GeneNode()
                                    {
                                        CurrentCondition = new Condition() {{"a", true}, {"b", true} },
                                        NodeName = "n2"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var booleanNetwork = new List<GeneLink>()
            {
                new GeneLink() {From = "a", To = "a", IsPositive = true},
                new GeneLink() {From = "b", To = "a", IsPositive = true},
            };

            var availableFunctions = new Dictionary<string, List<int>>() { { "a", new List<int>() { 0 } } };
            var res = solver.IsValidPath(automata, booleanNetwork, availableFunctions);
            Assert.IsTrue(res);
        }
    }


    [TestClass]
    public class FunctionBddSolverTestsOrFunc : AbstractBddTest
    {
        [TestInitialize]
        public void TestInit()
        {
            base.TestInit();
        }


        [TestMethod]
        public void TestOrFunctionPositive()
        {
            var solver = new BDDSolver();

            var automata = new GeneNode()
            {
                CurrentCondition = new Condition() { { "a", true }, { "b", true } },
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