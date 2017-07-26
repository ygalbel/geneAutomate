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


            //Set number of action names, 2 for a, b
            Model.NUMBER_OF_EVENT = automata.NodeLength+1;
            Model.MAX_NUMBER_EVENT_PARAMETERS = 0;

            BDDEncoder encoder = new BDDEncoder();

            letters.Distinct().ToList().ForEach(l => encoder.model.AddGlobalVar(l, 0, 1));

            Trace.WriteLine(string.Join(",", letters));

            SymbolicLTS lts = new SymbolicLTS();

            List<State> states = new List<State>();

            for (int i = 0; i < automata.NodeLength+1; i++)
            {
                var state = lts.AddState();
                states.Add(state);
            }

            lts.InitialState = states[0];


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
                    seq = new Sequence( seq,
                        ass);
                }
            });

            Trace.WriteLine("Assingments: " + seq);

            
            for (int i = 0; i < states.Count - 1; i++)
            {
                var trans1 = new Transition(new Event("a" + i), null, seq, states[i], states[i + 1]);
                lts.Transitions.Add(trans1);
            }

            Trace.WriteLine(lts);
            /*
            Transition trans1 = new Transition(new Event("a"), null,
               primitiveApplication1,
               state1,
               state2);

            Expression assignment = new Assignment(varX, new PrimitiveApplication(PrimitiveApplication.PLUS,
                new Variable(varX),
                new IntConstant(2)));

            var secAssignment = new Assignment(varY, new PrimitiveApplication(PrimitiveApplication.PLUS, new Variable(varY), new WildConstant()));
            


            Transition trans2 = new Transition(new Event("b"), null,
                assignment,
                state2,
                state3);

            Transition trans3 = new Transition(new Event("c"), null,
              primitiveApplication,
                state3,
                state4);

            lts.AddTransition(trans1);
            lts.AddTransition(trans2);
            lts.AddTransition(trans3);

            */
            AutomataBDD systemBDD = lts.Encode(encoder);

            // init is time 0

            automata.CurrentCondition.ToList().Where(f => f.Value.HasValue).ToList().ForEach(f =>
            {
                Expression value = Value(f);
                ;
                systemBDD.initExpression = new PrimitiveApplication(PrimitiveApplication.AND,
                    systemBDD.initExpression,
                    new PrimitiveApplication(PrimitiveApplication.EQUAL,
                        new Variable(f.Key), value));

            });

            Trace.WriteLine("init: " + systemBDD.initExpression);
            /*
            systemBDD.initExpression = new PrimitiveApplication(PrimitiveApplication.AND,
                                                                systemBDD.initExpression,
                                                                 new PrimitiveApplication(PrimitiveApplication.EQUAL,
                                                                 new Variable(varY), new IntConstant(1)));
                                                                 */
            CUDDNode initDD = CUDD.Function.Or(systemBDD.initExpression.TranslateBoolExpToBDD(encoder.model).GuardDDs);

            var u = 1;

            //Define 2 goals
            Expression goal1 = null;

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

            //new PrimitiveApplication(PrimitiveApplication.EQUAL,
            //    new Variable(varX), new IntConstant(5));


            Trace.WriteLine("Goal: "  +goal1);
            //Encode 2 goals to BDD
            CUDDNode goal1DD = CUDD.Function.Or(goal1.TranslateBoolExpToBDD(encoder.model).GuardDDs);

            List<CUDDNode> path = new List<CUDDNode>();

            bool reach1 = encoder.model.PathForward(initDD, goal1DD, new List<List<CUDDNode>>()
            {
                systemBDD.transitionBDD
            }, path, false);


            if (reach1)
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
            else
            {
                sb.AppendLine("goal1 is unreachable");
            }

            path.Clear();




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
            Trace.WriteLine(sb);
            encoder.model.Close();
            return reach1;

        }

        private static Assignment CreateAssignment(GeneLink b)
        {
            Assignment ass;

            if (b.IsPositive)
            {
                ass = new Assignment(b.To,
                    new Variable(b.From));
            }
            else
            {
                ass = new Assignment(b.To, 
                    new PrimitiveApplication(
                        PrimitiveApplication.AND, new PrimitiveApplication(nOT,new Variable(b.From))));
            }
            return ass;
        }

        private static Expression Value(KeyValuePair<string, bool?> f)
        {
            return new BoolConstant(f.Value.Value);
        }
    }
}

