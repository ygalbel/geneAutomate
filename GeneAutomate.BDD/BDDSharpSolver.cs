using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneAutomate.BDD.BDDSharp;
using GeneAutomate.Models;
using NLog;
using UCLouvain.BDDSharp;

namespace GeneAutomate.BDD
{
    public class BDDSharpSolver : IBDDSolver
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        private Dictionary<string, BDDNode> nodeStore = new Dictionary<string, BDDNode>();

        public bool IsValidPath(GeneNode automata, List<GeneLink> booleanNetwok, Dictionary<string, List<int>> availableFunctions = null)
        {
            var letters = new List<string>();
            var depth = automata.NodeLength;

            int z = 0;
            var manager = new BDDManager(50);
            automata.GetAllConditionLetters(letters);


            letters =
            letters.SelectMany(l => Enumerable.Range(0, depth).ToList().Select(n => Formater.FormatParameter(l, n)))
                .ToList();

            List<BDDNode> nodes = new List<BDDNode>();
            int i = 0;
            Dictionary<string, Tuple<int, BDDNode>> letterIndexes = new Dictionary<string, Tuple<int, BDDNode>>();

            BDDNode assignmentsBDDNode = null;
            /*foreach (var l in letters.Distinct().ToList())
            {
                var bddNode = manager.Create(i, manager.One, manager.Zero);
                letterIndexes[l] = new Tuple<int, BDDNode>(i, bddNode);
                nodeStore[l] = bddNode;
                nodes.Add(bddNode);
                i++;

                if (assignmentsBDDNode == null)
                {
                    assignmentsBDDNode = bddNode;
                }
                else
                {
                    assignmentsBDDNode = manager.And(assignmentsBDDNode, bddNode);
                }
            }*/
            logger.Info(string.Join(",", letters));

            var assignments = BDDLogicHelper.CreateDictBasedOnAutomata(automata);
            var last = manager.One;
            assignments.ToList().ForEach(a =>
            {
                BDDNode bddNode =  CreateNodeBasedOnAutomata(a.Key,a.Value, manager, last, i);
                i++;

                last = bddNode;
                letterIndexes[a.Key] = new Tuple<int, BDDNode>(i, bddNode);
                nodeStore[a.Key] = bddNode;
                /*if (assignmentsBDDNode == null)
                {
                    assignmentsBDDNode = node;
                }
                else
                {
                    assignmentsBDDNode = manager.And(assignmentsBDDNode, node);
                }*/
            });
            assignmentsBDDNode = last;

            var relations =
                CreateExpressionsFromBooleanNetwork(manager, booleanNetwok, availableFunctions, depth);

            var root =  relations;


            //  root = manager.Reduce(root);

            Dictionary<BDDNode, string> reverseHash = nodeStore.ToDictionary(a => a.Value, a => a.Key);


            // reverseHash.Add(root, "ROOT");
            Func<BDDNode, string> labelFunction = node =>
            reverseHash.ContainsKey(node) ?
                        reverseHash[node] + $"({node.Index})" :
                        $"({ reverseHash.FirstOrDefault(a => a.Key.Index == node.Index).Value})";
            logger.Info(manager.ToDot(root, labelFunction, show_all: true));

            IEnumerable<KeyValuePair<string, bool>> truth = BuildThruthTable(manager, root, i);

            assignments.ToList().ForEach(a =>
            {
                var index = letterIndexes[a.Key].Item2.Index;
                truth = truth.Where(d => d.Key[index] == (a.Value ? '1' : '0'));
            });

            return truth.Any(a => a.Value);

            //       CheckThruthTable(truth, res);

            //return true;
        }

        public static BDDNode CreateNodeBasedOnAutomata
            (string key, bool value,
            BDDManager manager, BDDNode last,
            int i)
        {

            BDDNode nodeBasedOnAutomata;
            if (value)
            {
                nodeBasedOnAutomata = manager.Create(i, last, manager.Zero, key);
            }
            else
            {

                nodeBasedOnAutomata = manager.Create(i, manager.Zero, last, key);
            }

            nodeBasedOnAutomata.OriginalValue = value;
            return nodeBasedOnAutomata;
        }

        protected Dictionary<string, bool> BuildThruthTable(BDDManager manager, BDDNode root, int i)
        {
            var truth = new Dictionary<string, bool>();
            AddThruthValue("", root, truth, i);
            return truth;
        }

        protected void CheckThruthTable(Dictionary<string, bool> matrix, BDDNode node)
        {
            foreach (var kv in matrix)
            {
                Dictionary<int, bool> interpretation = BuildInterpretation(kv.Key);
                bool value = EvaluateBDD(node, interpretation);
                //Assert.AreEqual(matrix[kv.Key], value);
            }
        }

        void AddThruthValue(string key, BDDNode node, Dictionary<string, bool> matrix, int acc)
        {
            if (acc == 0)
            {
                Dictionary<int, bool> interpretation = BuildInterpretation(key);
                bool value = EvaluateBDD(node, interpretation);
                matrix.Add(key, value);
                return;
            }

            AddThruthValue(key + "0", node, matrix, acc - 1);
            AddThruthValue(key + "1", node, matrix, acc - 1);
        }

        private static Dictionary<int, bool> BuildInterpretation(string key)
        {
            int index = 0;
            var interpretation = new Dictionary<int, bool>();
            foreach (var v in key)
            {
                interpretation.Add(index, v == '1');
                index++;
            }

            return interpretation;
        }

        bool EvaluateBDD(BDDNode root, Dictionary<int, bool> interpretation)
        {
            if (root.IsOne)
            {
                return true;
            }
            else if (root.IsZero)
            {
                return false;
            }
            else
            {
                var b = interpretation[root.Index];
                if (b)
                    return EvaluateBDD(root.High, interpretation);
                else
                    return EvaluateBDD(root.Low, interpretation);
            }
        }


        private BDDNode CreateExpressionsFromBooleanNetwork(
            BDDManager manager,
                                                 List<GeneLink> booleanNetwok,
                                                 Dictionary<string, List<int>> availableFunctions,
                                                 int depth,
                                                 Dictionary<string, bool> values = null)
        {
            var toDictionary = booleanNetwok.GroupBy(a => a.To);

            BDDNode seq = null;

            for (int i = 0; i < depth - 1; i++)
            {
                var ass = CreateFunctionApplication(manager, availableFunctions, toDictionary, i, values);

                if (seq == null)
                {
                    seq = ass;

                }
                else
                {
                    seq = manager.And(seq, ass);
                }

            }

            return seq;
        }

        private BDDNode CreateFunctionApplication(
            BDDManager manager,
            Dictionary<string, List<int>> availableFunctions,
            IEnumerable<IGrouping<string, GeneLink>> toDictionary,
            int i,
            Dictionary<string, bool> values)
        {
            BDDNode res = null;


            toDictionary.ToList().ForEach(ff =>
            {
                BDDNode ass = null;
                var froms = ff.Where(a => !a.IsOptional).ToList();

                // can be null!!
                var to = Formater.FormatParameter(ff.Key, i + 1);

                if (availableFunctions == null || !availableFunctions.ContainsKey(ff.Key))
                {
                    var from1 = Formater.FormatParameter(ff.FirstOrDefault().From, i);
                    //TODO: NOT!
                    res = manager.Equal(nodeStore[from1], nodeStore[to]);
                }
                else // real functions
                {

                    var availableFunc = availableFunctions[ff.Key];
                    var funcAssignmentHelper = new BddNodeFuncAssignmentHelper(manager, nodeStore);
                    var toFormatted = Formater.FormatParameter(to, i + 1);

                    availableFunc.ForEach(f =>
                    {
                        ass =
                            funcAssignmentHelper.CreateFuncAssignment(to, froms, i, f);

                        res = manager.Equal(nodeStore[to], ass);
                    });

                }
            });

            return res;
        }
    }
}
