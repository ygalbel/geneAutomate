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

namespace GeneAutomate.BDD
{
    public class BDDSolver
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private const string nOT = PrimitiveApplication.NOT;

        private static object locker = new object();

        public bool IsValidPath(GeneNode automata, List<GeneLink> booleanNetwok)
        {
            lock (locker)
            {
                var tempAutomata = automata;

                var letters = new List<string>();

                int z = 0;
                var depth = tempAutomata.NodeLength;

                tempAutomata.GetAllConditionLetters(letters);

                letters = letters.SelectMany(l => Enumerable.Range(0, depth).ToList().Select(n => FormatParameter(l, n))).ToList();

                logger.Info(tempAutomata.NodeLength + 1);

                Model.NUMBER_OF_EVENT = tempAutomata.NodeLength + 2;
                Model.MAX_NUMBER_EVENT_PARAMETERS = 0;

                BDDEncoder encoder = new BDDEncoder();

                letters.Distinct().ToList().ForEach(l => encoder.model.AddLocalVar(l, 0, 1));
                logger.Info(string.Join(",", letters));

                SymbolicLTS lts = new SymbolicLTS();

                List<State> states = new List<State>();
                var state0 = lts.AddState();
                states.Add(state0);

                var state1 = lts.AddState();
                states.Add(state1);


                lts.InitialState = states[0];

                var seq = CreateExpressionsFromBooleanNetwork(booleanNetwok, tempAutomata.NodeLength - 1);

                //// init is only based on first condition
                //// that's why here their not "visit"
                //automata.CurrentCondition.ToList()
                //        .Where(f => f.Value.HasValue).ToList().ForEach(f =>
                //        {
                //            Expression value = Value(f);

                //            var formatParameter = FormatParameter(f.Key, 0);
                //            seq = new  Sequence(seq,
                //                    new PrimitiveApplication(PrimitiveApplication.EQUAL,
                //                        new Variable(formatParameter), value));
                //        });


                logger.Info("Assignments: " + seq);

                var trans1 = 
                    new Transition(new Event("a"), null, seq, 
                    states[0], states[1]);

                lts.Transitions.Add(trans1);

                logger.Info(lts);
                AutomataBDD systemBDD = lts.Encode(encoder);

                // init is time 0

                CUDDNode initDD = CUDD.Function.Or(
                    systemBDD.initExpression.TranslateBoolExpToBDD(encoder.model).GuardDDs);

                bool reach1 = true;
                var path = new List<CUDDNode>();
                var geneTransition = tempAutomata;
                InitInitialState(geneTransition, systemBDD, letters);
                var goal = SetGoalsBasedOnAutomata(geneTransition);
                path = IsExistPath(goal, encoder, path, initDD, systemBDD, letters, ref reach1);
                path.Clear();
                encoder.model.Close();
                return reach1;
            }


        }

        private static string FormatParameter(string f, int i)
        {
            return $"{f.Replace('(', '_').Replace(')', '_')}_{i}";
        }

        private static Expression CreateExpressionsFromBooleanNetwork(List<GeneLink> booleanNetwok,
                                                                        int depth)
        {
            Expression seq = null;

            for (int i = 0; i < depth; i++)
            {
                // filter optional connections
                booleanNetwok.Where(s => !s.IsOptional).ToList().ForEach(b =>
                {
                    var ass = CreateAssignment(b, i);

                    if (seq == null)
                    {
                        seq = ass;
                    }
                    else
                    {
                        seq = new Sequence(seq,
                            ass);
                    }
                });
            }

            

            return seq;
        }

        private static List<CUDDNode> IsExistPath(Expression goal1, BDDEncoder encoder,
            List<CUDDNode> path,
            CUDDNode initDD,
            AutomataBDD systemBDD,
            List<string> letters,
            ref bool reach1)
        {
            CUDDNode goal1DD;
            goal1DD = CUDD.Function.Or(goal1.TranslateBoolExpToBDD(encoder.model).GuardDDs);

            path = new List<CUDDNode>();

            reach1 = encoder.model.PathForward(initDD, goal1DD, new List<List<CUDDNode>>()
            {
                systemBDD.transitionBDD
            }, path, false);


            logger.Info("Finish run. Result is " + reach1);

#if DEBUG
            StringBuilder builder = new StringBuilder();
            PrintResult(builder, path, encoder, letters);
            logger.Info(builder.ToString());
#endif

            return path;
        }

        private static void InitInitialState(GeneNode automata,
            AutomataBDD systemBDD,
            List<string> letters)
        {
            HashSet<string> handledLetters = new HashSet<string>();
            if (automata == null)
            {
                return;
            }

            int i = 0;
            

            // init is only based on first condition
            // that's why here their not "visit"
            automata.CurrentCondition.ToList()
                    .Where(f => f.Value.HasValue).ToList().ForEach(f =>
                {
                    Expression value = Value(f);

                    var formatParameter = FormatParameter(f.Key, i);
                    handledLetters.Add(formatParameter);
                    systemBDD.initExpression = 
                    new PrimitiveApplication(PrimitiveApplication.AND,
                            systemBDD.initExpression,
                            new PrimitiveApplication(PrimitiveApplication.EQUAL,
                                new Variable(formatParameter), value));
                });
            i++;
            ;


   
            
            logger.Info("init: " + systemBDD.initExpression);
        }

        private static Expression SetGoalsBasedOnAutomata(GeneNode automata)
        {
            Expression goal1 = null;

            if (automata == null || automata.Transitions == null || !automata.Transitions.Any())
            {
                return null;
            }

            int i = 0;
            automata.Visit(l =>
            {
                var tr = GetTransitions(l);

                if (tr == null)
                {
                    return;

                }

                tr
                    .ForEach(
                        f =>
                        {
                            var primitiveApplication = new PrimitiveApplication(PrimitiveApplication.EQUAL,
                                new Variable(FormatParameter(f.Key, i)),
                                new BoolConstant(f.Value.Value));

                            if (goal1 == null)
                            {
                                goal1 = primitiveApplication;
                            }
                            else
                            {
                                goal1 = new PrimitiveApplication(PrimitiveApplication.AND,
                                    goal1,
                                    primitiveApplication);
                            }
                        });
                i++;
            });

            logger.Info("Goal: " + goal1);
            return goal1;
        }

        private static List<KeyValuePair<string, bool?>> GetTransitions(GeneNode l)
        {
            return l?.CurrentCondition.Where(f => f.Value.HasValue).ToList();
        }

        private static void PrintResult(StringBuilder sb, List<CUDDNode> path, BDDEncoder encoder, List<string> letters)
        {
            foreach (var cuddNode in path)
            {
                CUDD.Print.PrintMinterm(cuddNode);
                //     CUDD.Print.PrintBDDTree(path);

                encoder.model.PrintAllVariableValue(cuddNode);
                letters.ForEach(l =>
                    {
                        int valueOfX = encoder.model.GetRowVarValue(cuddNode, l);
                        sb.AppendLine(l + " = " + valueOfX);
                    }
                );
            }
        }

        private static Assignment CreateAssignment(GeneLink b, int i)
        {
            Assignment ass;
            var from = FormatParameter(b.From, i);
            var to = FormatParameter(b.To, i + 1);

            if (b.IsPositive)
            {
                //ass = new Assignment(b.To,
                //    new Variable(b.From));

                ass = new Assignment(to,
                   new PrimitiveApplication(
                       PrimitiveApplication.AND, new Variable(from)));
            }
            else
            {
                ass = new Assignment(to,
                    new PrimitiveApplication(
                        PrimitiveApplication.AND, new PrimitiveApplication(nOT, new Variable(from))));
            }
            return ass;
        }

        private static Expression Value(KeyValuePair<string, bool?> f)
        {
            return new BoolConstant(f.Value.Value);
        }
    }
}

