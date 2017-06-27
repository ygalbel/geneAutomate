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
    public class BooleanNetworkValidatorTests
    {
        [TestMethod()]
        public void TestValidAutomataSingleNodeIsTrue()
        {
            BooleanNetworkValidator validator = new BooleanNetworkValidator();


            var automata = new GeneNode()
            {
                NodeName = "a",
              
            };

            var booleanNetwork = new List<GeneLink>()
            {
                new GeneLink() { From  = "A", To = "B", IsPositive = true},
                new GeneLink() { From = "A", To = "C", IsPositive = true }
            }
            ;

            var res = validator.IsValidAutomata(automata, null, booleanNetwork);

            Assert.IsTrue(res);
        }


        [TestMethod()]
        public void TestValidAutomataSimplePathIsTrue()
        {
            BooleanNetworkValidator validator = new BooleanNetworkValidator();


            var automata = new GeneNode()
            {
                NodeName = "a",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Condition = new Condition() { { "A", true}, { "B" , false}, {"C", false} },
                        Node = new GeneNode()
                        {
                            NodeName = "b",
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition()
                                {
                                    Condition = new Condition() { {"A" , true}, {"B", true} , {"C", true} },
                                    Node = new GeneNode() { NodeName = "final"}
                                }
                            }
                        }
                    }
                }

            };

            var booleanNetwork = new List<GeneLink>()
            {
                new GeneLink() { From  = "A", To = "B", IsPositive = true},
                new GeneLink() { From = "A", To = "C", IsPositive = true }
            }
            ;

            var res = validator.IsValidAutomata(automata, null, booleanNetwork);

            Assert.IsTrue(res);
        }

        [TestMethod()]
        public void TestValidAutomataSimplePathIsTrueWithNegative()
        {
            BooleanNetworkValidator validator = new BooleanNetworkValidator();


            var automata = new GeneNode()
            {
                NodeName = "a",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Condition = new Condition() { { "A", true}, { "B" , false}, {"C", false} },
                        Node = new GeneNode()
                        {
                            NodeName = "b",
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition()
                                {
                                    Condition = new Condition() { {"A" , true}, {"B", false} , {"C", true} },
                                    Node = new GeneNode() { NodeName = "final"}
                                }
                            }
                        }
                    }
                }

            };

            var booleanNetwork = new List<GeneLink>()
            {
                new GeneLink() { From  = "A", To = "B", IsPositive = false},
                new GeneLink() { From = "A", To = "C", IsPositive = true }
            }
            ;

            var res = validator.IsValidAutomata(automata, null, booleanNetwork);

            Assert.IsTrue(res);
        }


        [TestMethod()]
        public void TestValidAutomataSimplePathIsTrueWithMissingValues()
        {
            BooleanNetworkValidator validator = new BooleanNetworkValidator();


            var automata = new GeneNode()
            {
                NodeName = "a",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Condition = new Condition() { { "A", true}, { "B" , false}, {"C", null} },
                        Node = new GeneNode()
                        {
                            NodeName = "b",
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition()
                                {
                                    Condition = new Condition() { {"A" , true}, {"B", false} , {"C", true} },
                                    Node = new GeneNode() { NodeName = "final"}
                                }
                            }
                        }
                    }
                }

            };

            var booleanNetwork = new List<GeneLink>()
            {
                new GeneLink() { From  = "A", To = "B", IsPositive = false},
                new GeneLink() { From = "A", To = "C", IsPositive = true }
            }
            ;

            var res = validator.IsValidAutomata(automata, null, booleanNetwork);

            Assert.IsTrue(res);
        }





        [TestMethod()]
        public void TestValidAutomataSimplePathWithFalseRules()
        {
            BooleanNetworkValidator validator = new BooleanNetworkValidator();


            var automata = new GeneNode()
            {
                NodeName = "a",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Condition = new Condition(){ { "A", true}, { "B" , false}, {"C", false} },
                        Node = new GeneNode()
                        {
                            NodeName = "b",
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition()
                                {
                                    Condition = new Condition() { {"A" , true}, {"B", false} , {"C", true} },
                                    Node = new GeneNode() { NodeName = "final"}
                                }
                            }
                        }
                    }
                }

            };

            var booleanNetwork = new List<GeneLink>()
            {
                new GeneLink() { From  = "A", To = "B", IsPositive = true},
                new GeneLink() { From = "A", To = "C", IsPositive = true }
            }
            ;

            var res = validator.IsValidAutomata(automata, null, booleanNetwork);

            Assert.IsFalse(res);
        }
    }
}