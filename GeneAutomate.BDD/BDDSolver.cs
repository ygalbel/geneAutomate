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

        public bool IsValidPath(GeneNode automata, List<GeneLink> booleanNetwok)
        {
            var sb = new StringBuilder();

            var letters = new List<string>();

            automata.GetAllConditionLetters(letters);
          //  letters.Add("Time");

            //Set number of action names, 2 for a, b

            Trace.WriteLine(automata.NodeLength + 1);
            Model.NUMBER_OF_EVENT = automata.NodeLength + 2;
            Model.MAX_NUMBER_EVENT_PARAMETERS = 0;

            BDDEncoder encoder = new BDDEncoder();

            letters.Distinct().ToList().ForEach(l => encoder.model.AddLocalVar(l, 0, 1));
            encoder.model.AddLocalVar("Time", 0 ,automata.NodeLength +1);

            Trace.WriteLine(string.Join(",", letters));

            SymbolicLTS lts = new SymbolicLTS();

            List<State> states = new List<State>();

            for (int i = 0; i < automata.NodeLength + 2; i++)
            {
                var state = lts.AddState();
                states.Add(state);
            }

            lts.InitialState = states[0];

            var seq = CreateExpressionsFromBooleanNetwork(booleanNetwok);

            Trace.WriteLine("Assignments: " + seq);


            for (int i = 0; i < states.Count - 1; i++)
            {
                var trans1 = new Transition(new Event("a" + i), null, seq, states[i], states[i + 1]);
                lts.Transitions.Add(trans1);
            }

            Trace.WriteLine(lts);
            AutomataBDD systemBDD = lts.Encode(encoder);

            // init is time 0

            CUDDNode initDD = CUDD.Function.Or(systemBDD.initExpression.TranslateBoolExpToBDD(encoder.model).GuardDDs);

            bool reach1 = true;
            var path = new List<CUDDNode>();
            var geneTransition = automata;
            int n = 0;
            Expression goal1;

            while (geneTransition != null && reach1)
            {
                if (geneTransition == null || geneTransition.Transitions == null || !geneTransition.Transitions.Any())
                {
                    break;
                }

                Trace.WriteLine("Start step " + n);

                //systemBDD = lts.Encode(encoder);
                InitInitialState(geneTransition.CurrentCondition, systemBDD, n);
                goal1 = SetGoalsBasedOnAutomataNextTransition(geneTransition, n+1);
                path = IsExistPath(goal1, encoder, path, initDD, systemBDD, ref reach1);
                geneTransition = geneTransition.Transitions?.First()?.Node;
                n++;
            }


            path.Clear();
            Trace.WriteLine(sb);
            encoder.model.Close();
            return reach1;

        }

        private static Expression CreateExpressionsFromBooleanNetwork(List<GeneLink> booleanNetwok)
        {
            Expression seq = null;
            booleanNetwok.ForEach(b =>
            {
                var ass = CreateAssignment(b);

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

            seq = new Sequence(seq, new Assignment("Time", new PrimitiveApplication(PrimitiveApplication.PLUS,
                new Variable("Time"),
                new IntConstant(1))));
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

        private static void InitInitialState(Condition automataCurrentCondition, AutomataBDD systemBDD, int time)
        {
            if (automataCurrentCondition == null)
            {
                return;
            }
            systemBDD.initExpression =  new PrimitiveApplication(PrimitiveApplication.EQUAL,
                        new Variable("#state#0"), new IntConstant(0));

            automataCurrentCondition.ToList().Where(f => f.Value.HasValue).ToList().ForEach(f =>
            {
                Expression value = Value(f);
                    
                systemBDD.initExpression = new PrimitiveApplication(PrimitiveApplication.AND,
                    systemBDD.initExpression,
                    new PrimitiveApplication(PrimitiveApplication.EQUAL,
                        new Variable(f.Key), value));
            });

            systemBDD.initExpression = new PrimitiveApplication(PrimitiveApplication.AND,
                systemBDD.initExpression,
                new PrimitiveApplication(PrimitiveApplication.EQUAL,
                    new Variable("Time"), new IntConstant(time)));

            Trace.WriteLine("init: " + systemBDD.initExpression);
        }

        private static Expression SetGoalsBasedOnAutomataNextTransition(GeneNode automata, int time)
        {
            Expression goal1 = null;

            if (automata == null || automata.Transitions == null || !automata.Transitions.Any())
            {
                return null;
            }


            automata.Transitions.First()
                .Node.CurrentCondition.Where(f => f.Value.HasValue)
                .ToList()
                .ForEach(
                    f =>
                    {
                        var primitiveApplication = new PrimitiveApplication(PrimitiveApplication.EQUAL, new Variable(f.Key),
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
            if (goal1 != null)
            {
                goal1 = new PrimitiveApplication(PrimitiveApplication.AND,
                    goal1,
                    new PrimitiveApplication(PrimitiveApplication.EQUAL,
                        new Variable("Time"),
                        new IntConstant(time))
                );
            }

            Trace.WriteLine("Goal: " + goal1);
            return goal1;
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

        private static Assignment CreateAssignment(GeneLink b)
        {
            Assignment ass;

            if (b.IsPositive)
            {
                //ass = new Assignment(b.To,
                //    new Variable(b.From));

                ass = new Assignment(b.To,
                   new PrimitiveApplication(
                       PrimitiveApplication.AND, new Variable(b.From)));
            }
            else
            {
                ass = new Assignment(b.To,
                    new PrimitiveApplication(
                        PrimitiveApplication.AND, new PrimitiveApplication(nOT, new Variable(b.From))));
            }
            return ass;
        }

        private static Expression Value(KeyValuePair<string, bool?> f)
        {
            return new BoolConstant(f.Value.Value);
        }
    }
}

