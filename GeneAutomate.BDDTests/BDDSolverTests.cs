using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeneAutomate.BDD;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneAutomate.Models;
using NLog;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.SemanticModels.LTS.BDD;

namespace GeneAutomate.BDD.Tests
{
    [TestClass]
    public class FunctionBddSolverTests : AbstractBddTest
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
                CurrentCondition = new Condition() { { "a", true }, {"b", true} },
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

            var availableFunctions = new Dictionary<string, List<int>>() { {"a", new List<int>() {0} } };
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
    }

    public abstract class AbstractBddTest
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        protected TestContext m_testContext;

        public TestContext TestContext

        {

            get { return m_testContext; }

            set { m_testContext = value; }

        }

        public void TestInit()
        {
            logger.Info($"start test {TestContext.TestName}");
        }

    }

    [TestClass()]
    public class BDDSolverTests : AbstractBddTest
    {
        [TestInitialize]
        public void TestInit()
        {
            base.TestInit();
        }


        [TestMethod()]
        public void Test1Test()
        {
            
            var sb = new StringBuilder();
            string varX = "x";
            string varY = "y";

            //Set number of action names, 2 for a, b
            Model.NUMBER_OF_EVENT = 3;
            Model.MAX_NUMBER_EVENT_PARAMETERS = 0;

            BDDEncoder encoder = new BDDEncoder();
            encoder.model.AddGlobalVar(varX, 0, 10);
            encoder.model.AddGlobalVar(varY, 0, 10);

            SymbolicLTS lts = new SymbolicLTS();

            State state1 = lts.AddState();
            State state2 = lts.AddState();
            State state3 = lts.AddState();
            State state4 = lts.AddState();

            lts.InitialState = state1;

            var primitiveApplication1 = new Assignment(varX, new PrimitiveApplication(PrimitiveApplication.PLUS,
                new Variable(varX),
                new IntConstant(1)));


            var primitiveApplication2 = new Assignment(varY, new PrimitiveApplication(PrimitiveApplication.PLUS,
                new Variable(varY),
                new IntConstant(4)));


            /*
             * 
             * for (int i = 0; i < exps.Count; i++)
            {
                //Update eventParameterVariables[i] = exps[i]
                //Don't need to update exps because later after synchronization, not updated variable keeps the same value
                update = new Sequence(update, new Assignment(model.eventParameterVariables[i], exps[i]));
            }
             */
            var primitiveApplication = new Sequence(primitiveApplication1, 
                primitiveApplication2);

            /*PrimitiveApplication.CombineProgramBlock(
            primitiveApplication1,
            primitiveApplication2);*/
            logger.Info(primitiveApplication);

            Transition trans1 = new Transition(new Event("a"), null,
               primitiveApplication,
               state1,
               state2);

            Expression assignment = 
                new Assignment(varX, new PrimitiveApplication(PrimitiveApplication.PLUS,
                new Variable(varX),
                new IntConstant(2)));

            logger.Info("Assignments: " + assignment);

            var secAssignment = 
                new Assignment(varY, 
                new PrimitiveApplication(PrimitiveApplication.PLUS, 
                new Variable(varY), new WildConstant()));



            Transition trans2 = new Transition(new Event("b"), null,
                primitiveApplication,
                state2,
                state3);

            Transition trans3 = new Transition(new Event("c"), null,
              primitiveApplication,
                state3,
                state4);

            lts.AddTransition(trans1);
            lts.AddTransition(trans2);
            lts.AddTransition(trans3);

            AutomataBDD systemBDD = lts.Encode(encoder);

            //Variable x is initialised to 1
            systemBDD.initExpression = new PrimitiveApplication(PrimitiveApplication.AND,
                                                                systemBDD.initExpression,
                                                                 new PrimitiveApplication(PrimitiveApplication.EQUAL,
                                                                 new Variable(varX), new IntConstant(1)));

            systemBDD.initExpression = new PrimitiveApplication(PrimitiveApplication.AND,
                                                                systemBDD.initExpression,
                                                                 new PrimitiveApplication(PrimitiveApplication.EQUAL,
                                                                 new Variable(varY), new IntConstant(1)));

            CUDDNode initDD = CUDD.Function.Or(systemBDD.initExpression.TranslateBoolExpToBDD(encoder.model).GuardDDs);

            logger.Info("init: " + systemBDD.initExpression);

            var u = 1;

            for (; u < 2; u++)
            {
                logger.Info($"U is {u}");
                //Define 2 goals
                Expression goal1 = new PrimitiveApplication(PrimitiveApplication.EQUAL,
                        new Variable(varX), new IntConstant(3));

                goal1 = new PrimitiveApplication(PrimitiveApplication.AND, goal1, new PrimitiveApplication(PrimitiveApplication.EQUAL,new Variable(varY), new IntConstant(9)));

                //Encode 2 goals to BDD
                CUDDNode goal1DD = CUDD.Function.Or(goal1.TranslateBoolExpToBDD(encoder.model).GuardDDs);
                logger.Info("Goal: " + goal1);

                List<CUDDNode> path = new List<CUDDNode>();

                bool reach1 = encoder.model.PathForward(initDD, goal1DD, new List<List<CUDDNode>>()
            {
                systemBDD.transitionBDD
            }, path, true);


                if (reach1)
                {
                    sb.AppendLine("goal1 is reachable");

                    foreach (var cuddNode in path)
                    {
                        encoder.model.PrintAllVariableValue(cuddNode);
                        logger.Info("after");
                        CUDD.Print.PrintMinterm(cuddNode);
                        //     CUDD.Print.PrintBDDTree(path);
                        int valueOfX = encoder.model.GetRowVarValue(cuddNode, varX);
                        sb.AppendLine(varX + " = " + valueOfX);

                        int valueOfY = encoder.model.GetRowVarValue(cuddNode, varY);
                        sb.AppendLine(varY + " = " + valueOfY);
                    }
                }
                else
                {
                    sb.AppendLine("goal1 is unreachable");
                }

                path.Clear();

            }



            /*
            bool reach2 = encoder.model.PathForward(initDD, goal2DD, new List<List<CUDDNode>>() { systemBDD.transitionBDD }, path, true);
            if (reach2)
            {
                sb.AppendLine("goal2 is reachable");
                foreach (var cuddNode in path)
                {
                    int valueOfX = encoder.model.GetRowVarValue(cuddNode, varX);
                    sb.AppendLine(varX + " = " + valueOfX);
                }
            }
            else
            {
                sb.AppendLine("goal2 is unreachable");
            }
            */
            logger.Info(sb);
            encoder.model.Close();

        }

        [TestMethod]
        public void TestSimpleCaseNegativeFromTrueToFalseBDDSolver()
        {
            var solver = new BDDSolver();

            var automata = new GeneNode()
            {
                CurrentCondition = new Condition() { { "a", true } },
                NodeName = "n0",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            CurrentCondition = new Condition() {{"a", false}},
                            NodeName = "n1"
                        }
                    }
                }
            };

            var booleanNetwork = new List<GeneLink>()
            {
                new GeneLink() {From = "a", To = "a", IsPositive = false}
            };

            var res = solver.IsValidPath(automata, booleanNetwork);
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void TestSimpleCaseNegativeFromFalseToTrueBDDSolver()
        {
            var solver = new BDDSolver();

            var automata = new GeneNode()
            {
                CurrentCondition = new Condition() { { "a", false }, {"b", false} },
                NodeName = "n0",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            CurrentCondition = new Condition() {{"a", true}, {"b", false} },
                            NodeName = "n1"
                        }
                    }
                }
            };

            var booleanNetwork = new List<GeneLink>()
            {
                new GeneLink() {From = "b", To = "a", IsPositive = false}
            };

            var res = solver.IsValidPath(automata, booleanNetwork);
            Assert.IsTrue(res);
        }


        [TestMethod]
        public void TestSimpleCasePositiveBDDSolver()
        {
            var solver = new BDDSolver();

            var automata = new GeneNode()
            {
                CurrentCondition = new Condition() { { "a", true } },
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
                new GeneLink() {From = "a", To = "a", IsPositive = true}
            };

            var res = solver.IsValidPath(automata, booleanNetwork);
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void TestSCaseWithTwoParametersBDDSolver()
        {
            var solver = new BDDSolver();

            var automata = new GeneNode()
            {
                CurrentCondition = new Condition() { { "a", true }, {"b", false} },
                NodeName = "n0",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            CurrentCondition = new Condition() {{"a", true}, {"b", true} },
                            NodeName = "n1"
                        }
                    }
                }
            };

            var booleanNetwork = new List<GeneLink>()
            {
                new GeneLink() {From = "a", To = "b", IsPositive = true}
            };

            var res = solver.IsValidPath(automata, booleanNetwork);
            Assert.IsTrue(res);
        }


        [TestMethod]
        public void TestCaseWithOneParametersAndOneStepsIn_Time_2_BDDSolverShouldPass()
        {
            var solver = new BDDSolver();

            var automata = new GeneNode()
            {
                CurrentCondition = new Condition() { { "a", true }},
                NodeName = "n0",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            CurrentCondition = new Condition() {{"a", true}},
                            NodeName = "n1",
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition()
                                {
                                    Node = new GeneNode()
                                    {
                                        CurrentCondition = new Condition() { {"a", true } },
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
                //new GeneLink() {From = "b", To = "b", IsPositive = false}
            };

            var res = solver.IsValidPath(automata, booleanNetwork);
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void TestPositiveConnectionCantPassWithNegativeValue()
        {
            var solver = new BDDSolver();

            var automata = new GeneNode()
            {
                CurrentCondition = new Condition() {  },
                NodeName = "n0",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            CurrentCondition = new Condition() {{"a", true}},
                            NodeName = "n1",
                            Transitions = new List<GeneTransition>
                            {
                                new GeneTransition()
                                {
                                    Node = new GeneNode()
                                    {
                                        CurrentCondition =new Condition() {{"a", false}},
                                        NodeName = "n2",
                                      
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
                //new GeneLink() {From = "b", To = "b", IsPositive = false}
            };

            var res = solver.IsValidPath(automata, booleanNetwork);
            Assert.IsFalse(res);
        }

        [TestMethod]
        public void TestCaseWithOneParametersAndTwoStepsIn_Time_2_BDDSolverShouldPass()
        {
            var solver = new BDDSolver();

            var automata = new GeneNode()
            {
                CurrentCondition = new Condition() { { "a", true } },
                NodeName = "n0",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            CurrentCondition = new Condition() {{"a", true}},
                            NodeName = "n1",
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition()
                                {
                                    Node = new GeneNode()
                                    {
                                        CurrentCondition = new Condition() { {"a", true } },
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
                //new GeneLink() {From = "b", To = "b", IsPositive = false}
            };

            var res = solver.IsValidPath(automata, booleanNetwork);
            Assert.IsTrue(res);
        }


        [TestMethod]
        public void TestCaseWithOneParameterAndTwoStepsBDDSolverShouldFailedErrorInMiddle()
        {
            var solver = new BDDSolver();

            var automata = new GeneNode()
            {
                CurrentCondition = new Condition() { { "a", true } },
                NodeName = "n0",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            CurrentCondition = new Condition() {{"a", true} },
                            NodeName = "n1",
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition()
                                {
                                    Node = new GeneNode()
                                    {
                                        CurrentCondition = new Condition() { {"a", false }},
                                        NodeName = "n2",
                                       
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var booleanNetwork = new List<GeneLink>()
            {
                new GeneLink() {From = "a", To = "a", IsPositive = false},
            };

            var res = solver.IsValidPath(automata, booleanNetwork);
            Assert.IsFalse(res);
        }

        [TestMethod]
        public void TestCaseWithOneParameterAndTwoStepsSameAsAboveStartFromNegative()
        {
            var solver = new BDDSolver();

            var automata = new GeneNode()
            {
                CurrentCondition = new Condition() { { "a", false } },
                NodeName = "n0",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            CurrentCondition = new Condition() {{"a", false} },
                            NodeName = "n1",
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition()
                                {
                                    Node = new GeneNode()
                                    {
                                        CurrentCondition = new Condition() { {"a", true }},
                                        NodeName = "n2",

                                    }
                                }
                            }
                        }
                    }
                }
            };

            var booleanNetwork = new List<GeneLink>()
            {
                new GeneLink() {From = "a", To = "a", IsPositive = false},
            };

            var res = solver.IsValidPath(automata, booleanNetwork);
            Assert.IsFalse(res);
        }

        [TestMethod]
        public void TestCaseWithOneParameterAndTwoStepsBDDSolverShouldFailedErrorInLast()
        {
            var solver = new BDDSolver();

            var automata = new GeneNode()
            {
                CurrentCondition = new Condition() { { "a", true } },
                NodeName = "n0",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            CurrentCondition = new Condition() {{"a", false} },
                            NodeName = "n1",
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition()
                                {
                                    Node = new GeneNode()
                                    {
                                        CurrentCondition = new Condition() { {"a", false }},
                                        NodeName = "n2",

                                    }
                                }
                            }
                        }
                    }
                }
            };

            var booleanNetwork = new List<GeneLink>()
            {
                new GeneLink() {From = "a", To = "a", IsPositive = false},
            };

            var res = solver.IsValidPath(automata, booleanNetwork);
            Assert.IsFalse(res);
        }

        [TestMethod]
        public void TestCaseWithTwoParametersAndTwoStepsBDDSolverShouldFailed()
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
                            CurrentCondition = new Condition() {{"a", true}, {"b", true} },
                            NodeName = "n1",
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition()
                                {
                                    Node = new GeneNode()
                                    {                                           
                                        CurrentCondition = new Condition() { {"a", true }, { "b", false} },
                                        NodeName = "n2",
                                        Transitions = new List<GeneTransition>()
                                        {
                                            new GeneTransition()
                                            {
                                                Node = new GeneNode()
                                                {
                                                    NodeName = "n3",                               // here b is wrong
                                                    CurrentCondition = new Condition() { {"a", true }, { "b", false} },
                                                }
                                            }
                                        }
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
                new GeneLink() {From = "b", To = "b", IsPositive = false}
            };

            var res = solver.IsValidPath(automata, booleanNetwork);
            Assert.IsFalse(res);
        }


        [TestMethod]
        public void TestSCaseWithTwoParametersAndTwoRulesBDDSolver()
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
                            CurrentCondition = new Condition() {{"a", false}, {"b", true} },
                            NodeName = "n1"
                        }
                    }
                }
            };

            var booleanNetwork = new List<GeneLink>()
            {
                new GeneLink() {From = "a", To = "a", IsPositive = false},
                new GeneLink() {From = "b", To = "b", IsPositive = false}
            };

            var res = solver.IsValidPath(automata, booleanNetwork);
            Assert.IsTrue(res);
        }


        [TestMethod]
        public void TestSCaseWithThreeParametersAndThreeRulesBDDSolver()
        {
            var solver = new BDDSolver();

            var automata = new GeneNode()
            {
                CurrentCondition = new Condition() { { "a", true }, { "b", false }, {"c", true} },
                NodeName = "n0",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            CurrentCondition = new Condition() {{"a", false}, {"b", true}, {"c", true} },
                            NodeName = "n1",
                            //Transitions = new List<GeneTransition>()
                            //{
                            //    new GeneTransition()
                            //    {
                            //       Node = new GeneNode()
                            //        {
                            //            NodeName = "n2",
                            //            CurrentCondition = new Condition() { { "a", true }, { "b", false }, {"c", false} }

                            //        }
                            //    }
                            //}
                        }
                    }
                }
            };

            var booleanNetwork = new List<GeneLink>()
            {
                new GeneLink() {From = "a", To = "a", IsPositive = false},
                new GeneLink() {From = "b", To = "b", IsPositive = false},
                new GeneLink() {From = "a", To = "c", IsPositive = true},
            };

            var res = solver.IsValidPath(automata, booleanNetwork);
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void TestSCaseWithMultipleParametersButMissingRulesBDDSolver()
        {
            var solver = new BDDSolver();

            var automata = new GeneNode()
            {
                CurrentCondition = new Condition() { { "a", true }, { "b", false }, { "c", true } },
                NodeName = "n0",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            CurrentCondition = new Condition() {{"a", false}, {"b", true}, {"c", true} },
                            NodeName = "n1",
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition()
                                {
                                   Node = new GeneNode()
                                    {
                                        NodeName = "n2",
                                        CurrentCondition = new Condition() { { "a", true }, { "b", false }, {"c", true} }

                                    }
                                }
                            }
                        }
                    }
                }
            };


            // C not exist here!!
            var booleanNetwork = new List<GeneLink>()
            {
                new GeneLink() {From = "a", To = "a", IsPositive = false},
                new GeneLink() {From = "b", To = "b", IsPositive = false},
            };

            var res = solver.IsValidPath(automata, booleanNetwork);
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void TestSCaseWithMultipleParametersAddSomeParamsMissingInSomeNodes()
        {
            var solver = new BDDSolver();

            var automata = new GeneNode()
            {
                CurrentCondition = new Condition() { { "a", true }, { "b", false }, { "c", true } },
                NodeName = "n0",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {                                       // not C here
                            CurrentCondition = new Condition() {{"a", false}, {"b", true}},
                            NodeName = "n1",
                            Transitions = new List<GeneTransition>()
                            {
                                new GeneTransition()
                                {
                                   Node = new GeneNode()
                                    {
                                        NodeName = "n2",
                                        CurrentCondition = new Condition() { { "a", true }, { "b", false }, {"c", true} }

                                    }
                                }
                            }
                        }
                    }
                }
            };


            // C not exist here!!
            var booleanNetwork = new List<GeneLink>()
            {
                new GeneLink() {From = "a", To = "a", IsPositive = false},
                new GeneLink() {From = "b", To = "b", IsPositive = false},
            };

            var res = solver.IsValidPath(automata, booleanNetwork);
            Assert.IsTrue(res);
        }



        [TestMethod]
        public void TestNotOperatorIWorking()
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
                            CurrentCondition = new Condition() {{"a", true}, {"b", false} },
                            NodeName = "n1"
                        }
                    }
                }
            };

            var booleanNetwork = new List<GeneLink>()
            {
                new GeneLink() {From = "b", To = "b", IsPositive = false},
                new GeneLink() {From = "a", To = "a", IsPositive = false}
            };

            var res = solver.IsValidPath(automata, booleanNetwork);
            Assert.IsTrue(res);
        }
    }

}

