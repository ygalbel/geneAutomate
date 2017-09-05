using System.Collections.Generic;
using GeneAutomate.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneAutomate.BusinessLogic.Tests
{
    [TestClass]
    public class LoopMergeTests
    {
        [TestMethod]
        public void TestToySimpleLoopTest()
        {
            var automata1 = new GeneNode()
            {
                NodeName = "a0",
                CurrentCondition = new Condition() { { "A", true }, { "B", true }, { "C", true }, { "S1", false }, { "S2", true } },
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            NodeName = "a1",
                            CurrentCondition = new Condition() { { "A", false }, { "B", true }, { "C", true }, {"S1", false}, {"S2", true} },
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition()
                                {
                                    Node = new GeneNode()
                                    {
                                        NodeName = "a2",
                                        CurrentCondition = new Condition() { { "A", false }, { "B", true }, { "C", true }, {"S1", false}, {"S2", true} },
                                        Transitions = new List<GeneTransition>()
                                        {
                                            new GeneTransition() {
                                                Node = new GeneNode()
                                                {
                                                    NodeName = "a3",
                                                    CurrentCondition = new Condition() { { "A", false }, { "B", true }, { "C", true }, {"S1", false}, {"S2", true} },
                                                }
                                            },
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            List<GeneLink> booleanNetwok = new List<GeneLink>()
            {
              new GeneLink() { From = "S2", To = "B", IsPositive = true},
              new GeneLink() { From = "S1", To = "A", IsPositive = true},
              new GeneLink() { From = "S1", To = "S1", IsPositive = true},
              new GeneLink() { From = "S2", To = "S2", IsPositive = true},
            };

            var mergeLogic = new AutomataMergeLogic();
            var res = mergeLogic.CreateValidLoopMerge(automata1, booleanNetwok);
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void TestToyMultipleLoopTest()
        {
            var automata1 = new GeneNode()
            {
                NodeName = "a0",
                CurrentCondition = new Condition() { { "A", true }, { "B", true }, { "C", true }, { "S1", false }, { "S2", true } },
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            NodeName = "a1",
                            CurrentCondition = new Condition() { { "A", false }, { "B", true }, { "C", true }, {"S1", false}, {"S2", true} },
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition()
                                {
                                    Node = new GeneNode()
                                    {
                                        NodeName = "a2",
                                        CurrentCondition = new Condition() { { "A", false }, { "B", true }, { "C", true }, {"S1", false}, {"S2", true} },
                                        Transitions = new List<GeneTransition>()
                                        {
                                            new GeneTransition() {
                                                Node = new GeneNode()
                                                {
                                                    NodeName = "a3",
                                                    CurrentCondition = new Condition() { { "A", false }, { "B", true }, { "C", true }, {"S1", false}, {"S2", true} },
                                                }
                                            },
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            List<GeneLink> booleanNetwok = new List<GeneLink>()
            {
              new GeneLink() { From = "S2", To = "B", IsPositive = true},
              new GeneLink() { From = "S1", To = "A", IsPositive = true},
              new GeneLink() { From = "S1", To = "S1", IsPositive = true},
              new GeneLink() { From = "S2", To = "S2", IsPositive = true},
            };

            var mergeLogic = new AutomataMergeLogic();
            var res = mergeLogic.ApplyAllPossibleLoops(automata1, booleanNetwok);
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void TestToyMultipleLoopWitMissingDataTest()
        {
            var automata1 = new GeneNode()
            {
                NodeName = "a0",
                CurrentCondition = new Condition() { { "A", true }, { "B", true }, { "C", true }, { "S1", false }, { "S2", true } },
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            NodeName = "a1",
                            CurrentCondition =  new Condition() { },
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition()
                                {
                                    Node = new GeneNode()
                                    {
                                        NodeName = "a2",
                                        CurrentCondition = new Condition() {  },
                                        Transitions = new List<GeneTransition>()
                                        {
                                            new GeneTransition() {
                                                Node = new GeneNode()
                                                {
                                                    NodeName = "a3",
                                                    CurrentCondition = new Condition() { { "A", false }, { "B", true }, { "C", true }, {"S1", false}, {"S2", true} },
                                                }
                                            },
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            List<GeneLink> booleanNetwok = new List<GeneLink>()
            {
              new GeneLink() { From = "S2", To = "B", IsPositive = true},
              new GeneLink() { From = "S1", To = "A", IsPositive = true},
              new GeneLink() { From = "S1", To = "S1", IsPositive = true},
              new GeneLink() { From = "S2", To = "S2", IsPositive = true},
            };

            var mergeLogic = new AutomataMergeLogic();
            var res = mergeLogic.ApplyAllPossibleLoops(automata1, booleanNetwok);
            Assert.IsNotNull(res);
        }
    }
}