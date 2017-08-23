using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeneAutomate.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneAutomate.Models;

namespace GeneAutomate.BusinessLogic.Tests
{
   /* [TestClass]
    public class FinalMergeTests
    {
        [TestMethod]
        public void TestCanHandleExistingMergeCase()
        {
            var automata1 = new GeneNode()
            {
                NodeName = "a1",
                CurrentCondition = new Condition() { { "A", true }, { "B", false }, { "C", true } },
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            NodeName = "b1",
                            CurrentCondition = new Condition() { {"A" , false}, {"B", true} , {"C", false} },
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition()
                                {
                                    Node = new GeneNode() { NodeName = "c1", CurrentCondition = z}
                                }
                            }
                        }
                    }
                }

            };


            var automata2 = new GeneNode()
            {
                NodeName = "a2",
                CurrentCondition = new Condition() { { "A", false }, { "B", false }, { "C", false } },
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            NodeName = "b2",
                            CurrentCondition = new Condition() { {"A" , false}, {"B", true} , {"C", true} },
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition()
                                {
                                    Node = new GeneNode() { NodeName = "c2"}
                                }
                            }
                        }
                    }
                }

            };

            var automates = new List<GeneNode>()
            {
                new GeneNode()
                {
                    
                }
            };

            var res = new List<GeneNode>();
            new AutomataMergeLogic()
                .GetFinalMerges(new Stack<GeneNode>(automates.Select(a => a.Value).ToList()), links, res);

        }
}*/

    [TestClass()]
    public class AutomataMergeLogicTests
    {
        [TestMethod()]
        public void TestMergeWithSameConditionReturnMerged()
        {
            var automata1 = new GeneNode()
            {
                NodeName = "a1",
                CurrentCondition = new Condition() { { "A", true}, { "B" , true}, {"C", true} },
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            NodeName = "b1",
                            CurrentCondition = new Condition() { {"A" , false}, {"B", false} , {"C", false} },
                
                        }
                    }
                }

            };


            var automata2 = new GeneNode()
            {
                NodeName = "a2",
                CurrentCondition =  new Condition() { { "A", true}, { "B" , true}, {"C", true} },
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            NodeName = "b2",
                            CurrentCondition = new Condition() { {"A" , false}, {"B", false} , {"C", false} },
                
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
                CurrentCondition = new Condition() { { "B" , true}, {"C", true} }, // here A is missing
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            NodeName = "b1",
                            CurrentCondition = new Condition() { {"A" , false}, {"B", false} , {"C", false} },
                            
                        }
                    }
                }

            };


            var automata2 = new GeneNode()
            {
                NodeName = "a2",
                CurrentCondition = new Condition() { { "A", true}, { "B" , true} }, // Here C is missing
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            NodeName = "b2",
                            CurrentCondition = new Condition() { {"A" , false}, {"B", false} , {"C", false} },
                            
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
                CurrentCondition = new Condition() { { "A", true }, { "B", true }, { "C", true } },
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            NodeName = "b1",
                            CurrentCondition = new Condition() { {"A" , false}, {"B", false} , {"C", false} },
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition()
                                {
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
                CurrentCondition = new Condition() { {"A" , false}, {"B", false} , {"C", false} }, 
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            NodeName = "b2",
                            CurrentCondition = new Condition() { {"A" , false}, {"B", true} , {"C", true} },
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition()
                                {
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

        [TestMethod]
        public void TestPrefixLength()
        {
            var automata1 = new GeneNode()
            {
                NodeName = "a1",
                CurrentCondition = new Condition() { {"A" , false}, {"B", false} , {"C", false} },
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            NodeName = "b1",
                            CurrentCondition = new Condition() { { "A", true }, { "B", true }, { "C", true } },
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition() {Node = new GeneNode() { NodeName = "c1"} }
                            }
                        }
                    }
                }

            };

            // a2 is prefix, because only b2 will be matched
            var automata2 = new GeneNode()
            {
                NodeName = "a2",
                CurrentCondition = new Condition() { {"A" , false}, {"B", true} , {"C", true} },
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            NodeName = "b2",
                            CurrentCondition = new Condition() { { "A", false }, { "B", false }, { "C", false } },
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition() {Node = new GeneNode() { NodeName = "c2"} }
                            }
                        }
                    }
                }

            };

            var mergeLogic = new AutomataMergeLogic();
            var res = mergeLogic.GetMerges(automata1, automata2);

            // length should be 4 
            // a2 => b2@a1 => b1 => c1

            Assert.AreEqual(1, res.Count);
            var builder = new StringBuilder();
            res.First().AppendPath(builder);
            Trace.WriteLine(builder.ToString());
            Assert.AreEqual(4, res.First().NodeLength);
        }
    }
}