using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneAutomate.Models;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.SemanticModels.LTS.BDD;

namespace GeneAutomate.BDD
{
    public class BDDSolver
    {
        private const string nOT = PrimitiveApplication.NOT;

        private static object locker = new object();

        public bool IsValidPath(GeneNode automata, List<GeneLink> booleanNetwok)
        {
            lock (locker)
            {
                var sb = new StringBuilder();

                var letters = new List<string>();

                int z = 0;
                var depth = automata.NodeLength;

                automata.GetAllConditionLetters(letters);

                letters = letters.SelectMany(l => Enumerable.Range(0, depth).ToList().Select(n => FormatParameter(l, n))).ToList();

                Trace.WriteLine(automata.NodeLength + 1);

                Model.NUMBER_OF_EVENT = automata.NodeLength + 2;
                Model.MAX_NUMBER_EVENT_PARAMETERS = 0;

                BDDEncoder encoder = new BDDEncoder();

                letters.Distinct().ToList().ForEach(l => encoder.model.AddLocalVar(l, 0, 1));
                Trace.WriteLine(string.Join(",", letters));

                SymbolicLTS lts = new SymbolicLTS();

                List<State> states = new List<State>();
                var state0 = lts.AddState();
                states.Add(state0);

                var state1 = lts.AddState();
                states.Add(state1);


                lts.InitialState = states[0];

                var seq = CreateExpressionsFromBooleanNetwork(booleanNetwok, automata.NodeLength - 1);

                Trace.WriteLine("Assignments: " + seq);

                var trans1 = new Transition(new Event("a"), null, seq, states[0], states[1]);
                lts.Transitions.Add(trans1);

                Trace.WriteLine(lts);
                AutomataBDD systemBDD = lts.Encode(encoder);

                // init is time 0

                CUDDNode initDD = CUDD.Function.Or(systemBDD.initExpression.TranslateBoolExpToBDD(encoder.model).GuardDDs);

                bool reach1 = true;
                var path = new List<CUDDNode>();
                var geneTransition = automata;


                InitInitialState(geneTransition, systemBDD);
                var goal = SetGoalsBasedOnAutomata(geneTransition);
                path = IsExistPath(goal, encoder, path, initDD, systemBDD, ref reach1);
                geneTransition = geneTransition.Transitions?.First()?.Node;


                path.Clear();
                Trace.WriteLine(sb);
                encoder.model.Close();
                return reach1;
            }
           

        }

        private static string FormatParameter(string f, int i)
        {
            return $"{f}_{i}";
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
                    var ass = CreateAssignment(b,i);

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

        private static List<CUDDNode> IsExistPath(Expression goal1, BDDEncoder encoder, List<CUDDNode> path, CUDDNode initDD, AutomataBDD systemBDD,
            ref bool reach1)
        {
            CUDDNode goal1DD;
            goal1DD = CUDD.Function.Or(goal1.TranslateBoolExpToBDD(encoder.model).GuardDDs);

            path = new List<CUDDNode>();

            reach1 = encoder.model.PathForward(initDD, goal1DD, new List<List<CUDDNode>>()
            {
                systemBDD.transitionBDD
            }, path, false);


            Trace.WriteLine("Finish run. Result is " + reach1);

            return path;
        }

        private static void InitInitialState(GeneNode automata, 
            AutomataBDD systemBDD)
        {
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

                        systemBDD.initExpression = new PrimitiveApplication(PrimitiveApplication.AND,
                            systemBDD.initExpression,
                            new PrimitiveApplication(PrimitiveApplication.EQUAL,
                                new Variable(FormatParameter(f.Key, i)), value));
                    });
                i++;
            ;

           

            Trace.WriteLine("init: " + systemBDD.initExpression);
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

            Trace.WriteLine("Goal: " + goal1);
            return goal1;
        }

        private static List<KeyValuePair<string, bool?>> GetTransitions(GeneNode l)
        {
            return l?.CurrentCondition.Where(f => f.Value.HasValue).ToList();
        }

        private static void PrintResult(StringBuilder sb, List<CUDDNode> path, BDDEncoder encoder, List<string> letters)
        {
            sb.AppendLine("goal1 is reachable");
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
            var from = FormatParameter(b.From,i);
            var to = FormatParameter(b.To,i+1);

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

