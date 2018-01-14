//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;
//using GeneAutomate.Models;
//using NLog;
//using PAT.Common.Classes.CUDDLib;
//using PAT.Common.Classes.Expressions.ExpressionClass;
//using PAT.Common.Classes.LTS;
//using PAT.Common.Classes.SemanticModels.LTS.BDD;
//using Expression = PAT.Common.Classes.Expressions.ExpressionClass.Expression;

//namespace GeneAutomate.BDD
//{
//    public class BDDSolver : IBDDSolver
//    {
//        private static Logger logger = LogManager.GetCurrentClassLogger();

//        private const string nOT = PrimitiveApplication.NOT;

//        private static object locker = new object();

//        public bool IsValidPath(GeneNode automata, List<GeneLink> booleanNetwok, Dictionary<string, List<int>> availableFunctions = null)
//        {
//            lock (locker)
//            {
//                var tempAutomata = automata;
//                //    new GeneNode()
//                //{
//                //    NodeName = "Start",
//                //    CurrentCondition = new Condition(),
//                //    Transitions = new List<GeneTransition>()
//                //    {
//                //        new GeneTransition()
//                //        {
//                //            Node = automata
//                //        }
//                //    }
//                //};

//                var letters = new List<string>();

//                int z = 0;
//                var depth = tempAutomata.NodeLength;

//                tempAutomata.GetAllConditionLetters(letters);

//                letters = letters.SelectMany(l => Enumerable.Range(0, depth).ToList().Select(n => Formatter.FormatParameter(l, n))).ToList();

//                logger.Info(tempAutomata.NodeLength + 1);

//                Model.NUMBER_OF_EVENT = tempAutomata.NodeLength + 2;
//                Model.MAX_NUMBER_EVENT_PARAMETERS = 0;

//                BDDEncoder encoder = new BDDEncoder();

//                letters.Distinct().ToList().ForEach(l => encoder.model.AddLocalVar(l, 0, 1));
//                logger.Info(string.Join(",", letters));

//                SymbolicLTS lts = new SymbolicLTS();

//                List<State> states = new List<State>();
//                var state0 = lts.AddState();
//                states.Add(state0);


//                lts.InitialState = states[0];

//                var state1 = lts.AddState();
//                states.Add(state1);
//                //var goal2 = CreateExpressionBasedOnAutomata(tempAutomata);

//                var dictValues = BDDLogicHelper.CreateDictBasedOnAutomata(tempAutomata);
//                Expression seq = null;

//                /*CreateExpressionsFromBooleanNetwork(booleanNetwok,
//                        availableFunctions, depth, Mode.Assignment, dictValues);*/

//                if (dictValues.Any())
//                {
//                    dictValues.ToList().ForEach(f =>
//                    {
//                        Expression curr = new Assignment(f.Key, new BoolConstant( f.Value));
//                        seq = seq == null ? curr : new Sequence(seq, curr);

//                    });
//                }

                  

//                //seq =  new Sequence(seq, goal2);
//              //  //
//                logger.Info("Assignments: " + seq);

//                var trans1 =
//                    new Transition(new Event("a0"), null, seq,
//                        states[0], states[1]);

//                lts.Transitions.Add(trans1);
//                logger.Info(lts);
//                AutomataBDD systemBDD = lts.Encode(encoder);

//                systemBDD.PrintTransition();
//                CUDDNode initDD = CUDD.Function.Or(
//                    systemBDD.initExpression.TranslateBoolExpToBDD(encoder.model).GuardDDs);

//                bool reach1 = true;
//                var path = new List<CUDDNode>();
//                var geneTransition = tempAutomata;
//                InitInitialState(geneTransition, systemBDD, letters);
//                var goal = SetGoalsBasedOnAutomata(geneTransition);
//                var goal3 = CreateExpressionsFromBooleanNetwork(booleanNetwok,
//                        availableFunctions, depth, Mode.Equal);

//                goal = new PrimitiveApplication(PrimitiveApplication.AND, goal, goal3);
//                logger.Info("Goal: " + goal);
//                path = IsExistPath(goal, encoder, path, initDD, systemBDD, letters, ref reach1);
//                path.Clear();
//                encoder.model.Close();
//                return reach1;
//            }


//        }


//        private static Expression CreateExpressionsFromBooleanNetwork(List<GeneLink> booleanNetwok,
//                                                 Dictionary<string, List<int>> availableFunctions, 
//                                                 int depth, 
//                                                 Mode mode,
//                                                 Dictionary<string,bool> values=null )
//        {
//            Expression seq = null;

//            var toDictionary = booleanNetwok.GroupBy(a => a.To);

//            for (int i = 0; i < depth - 1; i++)
//            {
//                var ass = CreateFunctionApplication(availableFunctions, toDictionary, i, mode, values);

//                seq = AddIfExist(seq, ass, mode);
//            }

//            return seq;
//        }


//        private static Expression SetGoalBasedOnFunction(List<GeneLink> booleanNetwok,
//                                                                        Dictionary<string, List<int>> availableFunctions, int depth)
//        {
//            Expression seq = null;

//            var toDictionary = booleanNetwok.GroupBy(a => a.To);

//            for (int i = 0; i < depth - 1; i++)
//            {
//                Expression ass = null;


//                toDictionary.ToList().ForEach(ff =>
//                {
//                    var b = ff.FirstOrDefault();
//                    var from = Formatter.FormatParameter(b.From, i);
//                    var to = Formatter.FormatParameter(b.To, i + 1);

//                    if (b.IsPositive)
//                    {
//                        //ass = new Assignment(b.To,
//                        //    new Variable(b.From));

//                        ass = new PrimitiveApplication(PrimitiveApplication.EQUAL,
//                            new Variable(to), new Variable(from));
//                    }
//                    else
//                    {

//                        ass = new PrimitiveApplication(PrimitiveApplication.EQUAL,
//                            new Variable(to),
//                            new PrimitiveApplication(
//                                PrimitiveApplication.AND, new PrimitiveApplication(nOT, new Variable(from))));
//                    }

//                    if (seq == null)
//                    {
//                        seq = ass;
//                    }
//                    else
//                    {
//                        seq =
//                              new PrimitiveApplication(PrimitiveApplication.AND,
//                                        seq,
//                                        ass);
//                    }
//                });



//            }

//            return seq;
//        }

//        private static Expression CreateFunctionApplication(
//            Dictionary<string, List<int>> availableFunctions,
//            IEnumerable<IGrouping<string, GeneLink>> toDictionary,
//            int i,
//            Mode mode,
//            Dictionary<string,bool> values )
//        {
//            Expression res = null;


//            toDictionary.ToList().ForEach(ff =>
//            {
//                Expression ass = null;
//                var to = ff.Key;
//                var froms = ff.Where(a => !a.IsOptional).ToList();

//                // can be null!!

//                if (availableFunctions == null || !availableFunctions.ContainsKey(to))
//                {
//                    res = AddIfExist(res, CreateAssignment(ff.FirstOrDefault(), i, GetFunction(mode)), mode);
//                }
//                else
//                {

//                    var availableFunc = availableFunctions[to];
//                    var funcAssignmentHelper = new FuncAssignmentHelper();
//                    var toFormatted = Formatter.FormatParameter(to, i + 1);

//                    availableFunc.ForEach(f =>
//                    {
//                        ass =
//                            funcAssignmentHelper.CreateFuncAssignment(to, froms, i, f);

//                        //if (values != null && values.ContainsKey(toFormatted))
//                        //{
//                        //    ass = new PrimitiveApplication(PrimitiveApplication.AND, ass, new BoolConstant(values[toFormatted]));
//                        //    values.Remove(toFormatted);
//                        //}
//                    });


//                    if (mode == Mode.Equal)
//                    {
//                        res = new PrimitiveApplication(PrimitiveApplication.EQUAL, new Variable(toFormatted),
//                               ass);
//                    }
//                    else
//                    {
                        
//                        res = GetFunction(mode).Invoke(toFormatted, ass);
//                    }

//                }
//            });

//            return res;
//        }

//        private static Expression AddIfExist(Expression res, Expression createAssignment, Mode mode)
//        {
//            if (res == null)
//            {
//                return createAssignment;
//            }

//            if (mode == Mode.Equal)
//            {
//                res = new PrimitiveApplication(PrimitiveApplication.AND, res, createAssignment);
//            }
//            else
//            {
//                res = new Sequence(res, createAssignment);
//            }

//            return res;
//        }

//        private static Func<string, Expression, Expression> GetFunction(Mode mode)
//        {

//            return mode == Mode.Assignment ? AssignmentFunction : EqualityFunction;
//        }



//        private static Expression CreateEquality(string toFormatted, Expression ass)
//        {
//            return new PrimitiveApplication(PrimitiveApplication.EQUAL, new Variable(toFormatted), ass);
//        }

//        private static Assignment CreateAssignment(string toFormatted, Expression ass)
//        {
//            return new Assignment(toFormatted, ass);
//        }




//        private static List<CUDDNode> IsExistPath(Expression goal1, BDDEncoder encoder,
//            List<CUDDNode> path,
//            CUDDNode initDD,
//            AutomataBDD systemBDD,
//            List<string> letters,
//            ref bool reach1)
//        {
//            CUDDNode goal1DD;
//            goal1DD = CUDD.Function.Or(goal1.TranslateBoolExpToBDD(encoder.model).GuardDDs);

//            path = new List<CUDDNode>();

//            reach1 = encoder.model.PathForward(initDD, goal1DD, new List<List<CUDDNode>>()
//            {
//                systemBDD.transitionBDD
//            }, path, false);


//            logger.Info("Finish run. Result is " + reach1);

//#if DEBUG
//            StringBuilder builder = new StringBuilder();
//            PrintResult(builder, path, encoder, letters);
//            logger.Info(builder.ToString());
//#endif

//            return path;
//        }

//        private static void InitInitialState(GeneNode automata,
//            AutomataBDD systemBDD,
//            List<string> letters)
//        {
//            HashSet<string> handledLetters = new HashSet<string>();
//            if (automata == null)
//            {
//                return;
//            }

//            int i = 0;


//            //// init is only based on first condition
//            //// that's why here their not "visit"
//            //automata.CurrentCondition.ToList()
//            //        .Where(f => f.Value.HasValue).ToList().ForEach(f =>
//            //    {
//            //        Expression value = Value(f);

//            //        var formatParameter = Formater.FormatParameter(f.Key, i);
//            //        handledLetters.Add(formatParameter);
//            //        var primitiveApplication = BddHelper.SetBooleanValue(i, f.Value.Value, f.Key);

//            //        systemBDD.initExpression =
//            //        new PrimitiveApplication(PrimitiveApplication.AND,
//            //                systemBDD.initExpression,
//            //                primitiveApplication);
//            //    });
//            //i++;

//            logger.Info("init: " + systemBDD.initExpression);
//        }

//        private static Expression SetGoalsBasedOnAutomata(GeneNode automata)
//        {
//            Expression goal1 = null;

//            if (automata == null || automata.Transitions == null || !automata.Transitions.Any())
//            {
//                return null;
//            }

//            int i = 0;
//            automata.Visit(l =>
//            {
//                var tr = GetTransitions(l);

//                if (tr == null)
//                {
//                    return;

//                }

//                tr
//                    .ForEach(
//                        f =>
//                        {
//                            var primitiveApplication =
//                                BddHelper.SetBooleanValue(i, f.Value.Value, f.Key);

//                            if (goal1 == null)
//                            {
//                                goal1 = primitiveApplication;
//                            }
//                            else
//                            {
//                                goal1 = new PrimitiveApplication(PrimitiveApplication.AND,
//                                    goal1,
//                                    primitiveApplication);
//                            }
//                        });
//                i++;
//            });



//            return goal1;
//        }


//        private static Expression CreateExpressionBasedOnAutomata(GeneNode automata)
//        {
//            Expression goal1 = null;

//            if (automata == null || automata.Transitions == null || !automata.Transitions.Any())
//            {
//                return null;
//            }

//            int i = 0;
//            automata.Visit(l =>
//            {
//                var tr = GetTransitions(l);

//                if (tr == null)
//                {
//                    return;

//                }

//                tr
//                    .ForEach(
//                        f =>
//                        {
//                            var primitiveApplication =
//                            new Assignment(Formatter.FormatParameter(f.Key, i), new BoolConstant(f.Value.Value));

//                            if (goal1 == null)
//                            {
//                                goal1 = primitiveApplication;
//                            }
//                            else
//                            {
//                                goal1 = new Sequence(
//                                    goal1,
//                                    primitiveApplication);
//                            }
//                        });
//                i++;
//            });


//            logger.Info("Goal: " + goal1);
//            return goal1;
//        }

//        public static List<KeyValuePair<string, bool?>> GetTransitions(GeneNode l)
//        {
//            return l?.CurrentCondition.Where(f => f.Value.HasValue).ToList();
//        }

//        private static void PrintResult(StringBuilder sb, List<CUDDNode> path, BDDEncoder encoder, List<string> letters)
//        {
//            foreach (var cuddNode in path)
//            {
//                CUDD.Print.PrintMinterm(cuddNode);
//                //     CUDD.Print.PrintBDDTree(path);

//                encoder.model.PrintAllVariableValue(cuddNode);
//                letters.ForEach(l =>
//                    {
//                        int valueOfX = encoder.model.GetRowVarValue(cuddNode, l);
//                        sb.AppendLine(l + " = " + valueOfX);
//                    }
//                );

//                int v = encoder.model.GetRowVarValue(cuddNode, "#state#0");
//                sb.AppendLine("#state#0" + " = " + v);

//                int v1 = encoder.model.GetRowVarValue(cuddNode, "#event");
//                sb.AppendLine("#event" + " = " + v1);
//            }
//        }

//        private static Expression CreateAssignment(GeneLink b, int i, 
//            Func<string, Expression, Expression> accumulatorFunc)
//        {
//            Expression ass;
//            var from = Formatter.FormatParameter(b.From, i);
//            var to = Formatter.FormatParameter(b.To, i + 1);

//            if (b.IsPositive)
//            {
//                //ass = new Assignment(b.To,
//                //    new Variable(b.From));

//                ass = accumulatorFunc.Invoke(to, new Variable(from));
//            }
//            else
//            {
                
//                ass = accumulatorFunc.Invoke(to,
//                      new PrimitiveApplication(
//                                PrimitiveApplication.AND,new PrimitiveApplication(nOT, new Variable(from))));

//                //ass = new PrimitiveApplication(PrimitiveApplication.EQUAL,
//                //            new Variable(to),
//                //            new PrimitiveApplication(
//                //                PrimitiveApplication.AND, new PrimitiveApplication(nOT, new Variable(from))));
//            }
//            return ass;
//        }


//        private static Func<string, Expression, Expression> AssignmentFunction = (to, from) =>
//                  new Assignment(to, from);


//        private static Func<string, Expression, Expression> EqualityFunction = (to, from) =>
//                new PrimitiveApplication(PrimitiveApplication.EQUAL,
//                    new Variable(to), from);

//        private static Expression Value(KeyValuePair<string, bool?> f)
//        {
//            return new BoolConstant(f.Value.Value);
//        }
//    }

//    public enum Mode
//    {
//        Assignment,
//        Equal
//    }
//}

