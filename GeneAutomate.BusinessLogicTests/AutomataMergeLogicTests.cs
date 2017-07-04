using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeneAutomate.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneAutomate.Models;

namespace GeneAutomate.BusinessLogic.Tests
{
    [TestClass()]
    public class AutomataMergeLogicTests
    {
        [TestMethod()]
        public void TestMergeWithSameConditionReturnMerged()
        {
            var automata1 = new GeneNode()
            {
                NodeName = "a1",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Condition = new Condition() { { "A", true}, { "B" , true}, {"C", true} },
                        Node = new GeneNode()
                        {
                            NodeName = "b1",
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition()
                                {
                                    Condition = new Condition() { {"A" , false}, {"B", false} , {"C", false} },
                                    Node = new GeneNode() { NodeName = "c1"}
                                }
                            }
                        }
                    }
                }

            };


            var automata2 = new GeneNode()
            {
                NodeName = "a2",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Condition = new Condition() { { "A", true}, { "B" , true}, {"C", true} },
                        Node = new GeneNode()
                        {
                            NodeName = "b2",
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition()
                                {
                                    Condition = new Condition() { {"A" , false}, {"B", false} , {"C", false} },
                                    Node = new GeneNode() { NodeName = "c2"}
                                }
                            }
                        }
                    }
                }

            };

            var mergeLogic = new AutomataMergeLogic();
            var res = mergeLogic.GetMerges(automata1, automata2);

            Assert.AreEqual(1,res.Count);
        }

        [TestMethod()]
        public void TestMergeWithDifferentConditionReturnNull()
        {
            var automata1 = new GeneNode()
            {
                NodeName = "a1",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Condition = new Condition() { { "A", false}, { "B" , true}, {"C", true} },
                        Node = new GeneNode()
                        {
                            NodeName = "b1",
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition()
                                {
                                    Condition = new Condition() { {"A" , false}, {"B", false} , {"C", false} },
                                    Node = new GeneNode() { NodeName = "c1"}
                                }
                            }
                        }
                    }
                }

            };


            var automata2 = new GeneNode()
            {
                NodeName = "a2",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Condition = new Condition() { { "A", true}, { "B" , true}, {"C", true} },
                        Node = new GeneNode()
                        {
                            NodeName = "b2",
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition()
                                {
                                    Condition = new Condition() { {"A" , false}, {"B", false} , {"C", false} },
                                    Node = new GeneNode() { NodeName = "c2"}
                                }
                            }
                        }
                    }
                }

            };

            var mergeLogic = new AutomataMergeLogic();
            var res = mergeLogic.GetMerges(automata1, automata2);

            Assert.AreEqual(0, res.Count);

        }

        [TestMethod()]
        public void TestMergeWithNullConditionReturnValid()
        {
            var automata1 = new GeneNode()
            {
                NodeName = "a1",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Condition = new Condition() { { "B" , true}, {"C", true} }, // here A is missing
                        Node = new GeneNode()
                        {
                            NodeName = "b1",
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition()
                                {
                                    Condition = new Condition() { {"A" , false}, {"B", false} , {"C", false} },
                                    Node = new GeneNode() { NodeName = "c1"}
                                }
                            }
                        }
                    }
                }

            };


            var automata2 = new GeneNode()
            {
                NodeName = "a2",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Condition = new Condition() { { "A", true}, { "B" , true} }, // Here C is missing
                        Node = new GeneNode()
                        {
                            NodeName = "b2",
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition()
                                {
                                    Condition = new Condition() { {"A" , false}, {"B", false} , {"C", false} },
                                    Node = new GeneNode() { NodeName = "c2"}
                                }
                            }
                        }
                    }
                }

            };

            var mergeLogic = new AutomataMergeLogic();
            var res = mergeLogic.GetMerges(automata1, automata2);

            Assert.AreEqual(1, res.Count);
            Assert.AreEqual(res.First().Transitions.First().Condition["A"], true);
            Assert.AreEqual(res.First().Transitions.First().Condition["B"], true);
            Assert.AreEqual(res.First().Transitions.First().Condition["C"], true);


        }


        [TestMethod()]
        public void TestMergeWhenAutomata2CanBeMergedFromAutomata1Child()
        {
            var automata1 = new GeneNode()
            {
                NodeName = "a1",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Condition = new Condition() { { "A", true }, { "B" , true}, {"C", true} }, 
                        Node = new GeneNode()
                        {
                            NodeName = "b1",
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition()
                                {
                                    Condition = new Condition() { {"A" , false}, {"B", false} , {"C", false} },
                                    Node = new GeneNode() { NodeName = "c1"}
                                }
                            }
                        }
                    }
                }

            };


            var automata2 = new GeneNode()
            {
                NodeName = "a2",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Condition = new Condition() { {"A" , false}, {"B", false} , {"C", false} }, 
                        Node = new GeneNode()
                        {
                            NodeName = "b2",
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition()
                                {
                                    Condition = new Condition() { {"A" , false}, {"B", true} , {"C", true} },
                                    Node = new GeneNode() { NodeName = "c2"}
                                }
                            }
                        }
                    }
                }

            };

            var mergeLogic = new AutomataMergeLogic();
            var res = mergeLogic.GetMerges(automata1, automata2);

            Assert.AreEqual(1, res.Count);
            Assert.AreEqual(3, res.First().NodeLength);
        }
    }
}