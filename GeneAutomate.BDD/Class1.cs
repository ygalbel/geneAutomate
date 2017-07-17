using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.SemanticModels.LTS.BDD;

namespace GeneAutomate.BDD
{
    public class BDDSolver
    {
        public static string Test1()
        {
            var sb = new StringBuilder();
            string varX = "x";
            //Set number of action names, 2 for a, b
            Model.NUMBER_OF_EVENT = 2;
            Model.MAX_NUMBER_EVENT_PARAMETERS = 0;

            BDDEncoder encoder = new BDDEncoder();
            encoder.model.AddGlobalVar(varX, 0, 10);

            SymbolicLTS lts = new SymbolicLTS();

            State state1 = lts.AddState();
            State state2 = lts.AddState();
            State state3 = lts.AddState();

            lts.InitialState = state1;

            Transition trans1 = new Transition(new Event("a"), null, new Assignment(varX, new PrimitiveApplication(PrimitiveApplication.PLUS, new Variable(varX), new IntConstant(1))), state1, state2);
            Transition trans2 = new Transition(new Event("b"), null, new Assignment(varX, new PrimitiveApplication(PrimitiveApplication.PLUS, new Variable(varX), new IntConstant(2))), state2, state3);

            lts.AddTransition(trans1);
            lts.AddTransition(trans2);

            AutomataBDD systemBDD = lts.Encode(encoder);

            //Variable x is initialised to 1
            systemBDD.initExpression = new PrimitiveApplication(PrimitiveApplication.AND, systemBDD.initExpression,
                                                                  new PrimitiveApplication(PrimitiveApplication.EQUAL, new Variable(varX), new IntConstant(1)));

            CUDDNode initDD = CUDD.Function.Or(systemBDD.initExpression.TranslateBoolExpToBDD(encoder.model).GuardDDs);

            //Define 2 goals
            Expression goal1 = new PrimitiveApplication(PrimitiveApplication.EQUAL, new Variable(varX), new IntConstant(4));
            Expression goal2 = new PrimitiveApplication(PrimitiveApplication.EQUAL, new Variable(varX), new IntConstant(5));

            //Encode 2 goals to BDD
            CUDDNode goal1DD = CUDD.Function.Or(goal1.TranslateBoolExpToBDD(encoder.model).GuardDDs);
            CUDDNode goal2DD = CUDD.Function.Or(goal2.TranslateBoolExpToBDD(encoder.model).GuardDDs);

            List<CUDDNode> path = new List<CUDDNode>();

            bool reach1 = encoder.model.PathForward(initDD, goal1DD, new List<List<CUDDNode>>() { systemBDD.transitionBDD }, path, true);
            if (reach1)
            {
                sb.AppendLine("goal1 is reachable");
                foreach (var cuddNode in path)
                {
                    int valueOfX = encoder.model.GetRowVarValue(cuddNode, varX);
                    sb.AppendLine(varX + " = " + valueOfX);
                }
            }
            else
            {
                sb.AppendLine("goal1 is unreachable");
            }

            path.Clear();

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

            encoder.model.Close();

            return sb.ToString();
        }

    }
}

