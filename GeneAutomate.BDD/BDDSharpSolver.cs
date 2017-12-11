using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneAutomate.Models;
using NLog;
using UCLouvain.BDDSharp;

namespace GeneAutomate.BDD
{
    public class BDDSharpSolver : IBDDSolver
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public bool IsValidPath(GeneNode automata, List<GeneLink> booleanNetwok, Dictionary<string, List<int>> availableFunctions = null)
        {
            var letters = new List<string>();

            int z = 0;
            var manager = new BDDManager(z);
            var depth = automata.NodeLength;

            automata.GetAllConditionLetters(letters);


            letters =
            letters.SelectMany(l => Enumerable.Range(0, depth).ToList().Select(n => Formater.FormatParameter(l, n)))
                .ToList();

            List<BDDNode> nodes = new List<BDDNode>();
            int i = 0;
            Dictionary<string, Tuple<int,BDDNode>> letterIndexes = new Dictionary<string, Tuple<int,BDDNode>>();
            foreach (var l in letters.Distinct().ToList())
            {
                var bddNode = manager.Create(i, manager.One, manager.Zero);
                letterIndexes[l] = new Tuple<int, BDDNode>(i, bddNode);
                nodes.Add(bddNode);
                i++;
            }
            logger.Info(string.Join(",", letters));

            var assignments = BDDLogicHelper.CreateDictBasedOnAutomata(automata);
            assignments.ToList().ForEach(a =>
            {
                manager.Create(i++, 
                    manager.Equal(letterIndexes[a.Key].Item2, a.Value ? manager.One : manager.Zero), 
                    manager.Zero);
            });

            throw new NotImplementedException();
        }


        private static void CreateExpressionsFromBooleanNetwork(
            BDDManager manager,
                                                 List<GeneLink> booleanNetwok,
                                                 Dictionary<string, List<int>> availableFunctions,
                                                 int depth,
                                                 Mode mode,
                                                 Dictionary<string, bool> values = null)
        {
            var toDictionary = booleanNetwok.GroupBy(a => a.To);

            for (int i = 0; i < depth - 1; i++)
            {
                var ass = CreateFunctionApplication(availableFunctions, toDictionary, i, mode, values);

                seq = AddIfExist(seq, ass, mode);
            }

            return seq;
        }

        private static BDDNode CreateFunctionApplication(
            BDDManager manager,
            Dictionary<string, List<int>> availableFunctions,
            IEnumerable<IGrouping<string, GeneLink>> toDictionary,
            int i,
            Mode mode,
            Dictionary<string, bool> values)
        {
            Expression res = null;


            toDictionary.ToList().ForEach(ff =>
            {
                BDDNode ass = null;
                var to = ff.Key;
                var froms = ff.Where(a => !a.IsOptional).ToList();

                // can be null!!

                if (availableFunctions == null || !availableFunctions.ContainsKey(to))
                {
                    res = manager.Create() (res, CreateAssignment(ff.FirstOrDefault(), i, GetFunction(mode)), mode);
                }
                else
                {

                    var availableFunc = availableFunctions[to];
                    var funcAssignmentHelper = new FuncAssignmentHelper();
                    var toFormatted = Formater.FormatParameter(to, i + 1);

                    availableFunc.ForEach(f =>
                    {
                        ass =
                            funcAssignmentHelper.CreateFuncAssignment(to, froms, i, f);

                        //if (values != null && values.ContainsKey(toFormatted))
                        //{
                        //    ass = new PrimitiveApplication(PrimitiveApplication.AND, ass, new BoolConstant(values[toFormatted]));
                        //    values.Remove(toFormatted);
                        //}
                    });


                    if (mode == Mode.Equal)
                    {
                        res = new PrimitiveApplication(PrimitiveApplication.EQUAL, new Variable(toFormatted),
                               ass);
                    }
                    else
                    {

                        res = GetFunction(mode).Invoke(toFormatted, ass);
                    }

                }
            });

            return res;
        }
    }
}
