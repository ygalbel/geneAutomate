using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneAutomate.BDD.BDDSharp;
using GeneAutomate.Models;
using Ninject.Infrastructure.Disposal;
using NLog;
using UCLouvain.BDDSharp;

namespace GeneAutomate.BDD
{
    public class BDDSharpSolver : IBDDSolver
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public bool IsValidPath(GeneNode automata, GeneFullRules booleanNetwok)
        {
            var nodeStore = new Dictionary<string, BDDNode>();
            var letters = new List<string>();
            var depth = automata.NodeLength;

            int z = 0;
            var manager = new BDDManager(10000);
            automata.GetAllConditionLetters(letters);

            letters =
            letters.SelectMany(l => Enumerable.Range(0, depth).ToList().Select(n => Formatter.FormatParameter(l, n)))
                .ToList();

            int nodeNumber = 0;
            logger.Info(string.Join(",", letters));

            var assignments = BDDLogicHelper.CreateDictBasedOnAutomata(automata);

            nodeStore["1"] = manager.One;
            nodeStore["0"] = manager.Zero;

            var assignmentsBDDNode = CreateAssignmentsBddNodes(assignments, manager, nodeStore, ref nodeNumber);

            BDDNode functionNodes = CreateFunctionKeys(booleanNetwok.Functions, manager, nodeStore, ref nodeNumber);
            CreateOptionalKeys(manager, booleanNetwok.GeneLinks, nodeNumber, nodeStore);

            AddMissingNodes(manager,nodeStore, letters,ref nodeNumber);

            var relations =
                CreateExpressionsFromBooleanNetwork(manager, booleanNetwok.GeneLinks, booleanNetwok.Functions, depth, nodeStore);

            relations = manager.And(relations, functionNodes);
            var root = manager.And(relations, assignmentsBDDNode);


            root = manager.Reduce(root);
            //// LOG PART
            LogToDotFormat(root, manager, nodeStore);

            //logger.Info("relations");
            //logger.Info(manager.ToDot(relations, show_all: false));

            //logger.Info("assignments");
            //logger.Info(manager.ToDot(assignmentsBDDNode, show_all: false));


         /*  IEnumerable<KeyValuePair<string, bool>> truth = BuildThruthTable(manager, root, nodeStore.Count -2); // remove zero and one

            assignments.ToList().ForEach(a =>
            {
                var index = nodeStore[a.Key].Index;
                truth = truth.Where(d =>
                    d.Key[index] == (a.Value ? '1' : '0'));
            });

            var pathes = truth.Where(a => a.Value).ToList();

            LogValidPathes(pathes, nodeStore);
            */
            return !root.Equals(manager.Zero);

            //       CheckThruthTable(truth, res);

            //return true;
        }

        private void AddMissingNodes(BDDManager manager, Dictionary<string, BDDNode> nodeStore, List<string> letters, ref int nodeNumber)
        {
            var alreadyCreated = nodeStore.Keys;
            foreach (var l in letters.Where(a => !alreadyCreated.Contains(a)).ToList())
            {
                var node = manager.Create(nodeNumber++, 1, 1);
                nodeStore[l] = node;
            }
        }


        private void CreateOptionalKeys(BDDManager manager, List<GeneLink> booleanNetwok, int nodeNumber, Dictionary<string, BDDNode> nodeStore)
        {
            foreach (var geneLink in booleanNetwok)
            {
                if (geneLink.IsOptional)
                {
                    var key = Formatter.OptionalRelation(geneLink.From, geneLink.To);
                    var node = manager.Create(nodeNumber++, manager.One, manager.Zero);
                    nodeStore[key] = node;
                }    
            }
        }

        private static BDDNode CreateFunctionKeys(Dictionary<string, List<int>> availableFunctions, 
            BDDManager manager, 
            Dictionary<string, BDDNode> nodeStore, ref int nodeNumber)
        {
            BDDNode functionNodes = null;
            var functionsKeys = CreateFunctionsKeys(availableFunctions);
            foreach (var f in functionsKeys.ToList())
            {
                BDDNode currentNodeOr = null;
                List<BDDNode> currents = new List<BDDNode>();
                foreach (var d in f.Value)
                {
                    var c = manager.Create(nodeNumber++, 1, 0);
                    nodeStore[d] = c;
                    currents.Add(c);

                    currentNodeOr = manager.OrSafe(currentNodeOr, c);
                }

                currentNodeOr = manager.Equal(manager.One, currentNodeOr);
                functionNodes = manager.AndSafe(functionNodes, currentNodeOr);
            }

            return functionNodes;
        }

        private void LogValidPathes(List<KeyValuePair<string, bool>> pathes, Dictionary<string, BDDNode> nodeStore)
        {
            Dictionary<int, string> reverseHash = nodeStore.Where(a => a.Value.Index != 50).ToDictionary(a => a.Value.Index, a => a.Key);

            if (!pathes.Any())
            {
                logger.Info($"No Valid Pathes");
                return;
            }

            logger.Info("Valid pathes:");

            pathes.Select(a => a.Key).ToList().ForEach(p =>
            {
                int i = 0;
                foreach (var ch in p)
                {
                    logger.Info($"{reverseHash[i]} - {ch}");
                    i++;
                }

                logger.Info($"=============================");
            });
            logger.Warn("Finish");

        }

        private void LogToDotFormat(BDDNode root, BDDManager manager, Dictionary<string, BDDNode> nodeStore)
        {
            Dictionary<BDDNode, string> reverseHash = nodeStore.ToDictionary(a => a.Value, a => a.Key);

            Func<BDDNode, string> labelFunction = node =>
                reverseHash.ContainsKey(node)
                    ? reverseHash[node] + $"({node.Index})"
                    : $"({reverseHash.FirstOrDefault(a => a.Key.Index == node.Index).Value})";

            logger.Info("ROOT");
            logger.Info(manager.ToDot(root, show_all: false, labelFunction: labelFunction));
        }

        public static Dictionary<string, List<string>> CreateFunctionsKeys(Dictionary<string, List<int>> availableFunctions)
        {
            return (from af in availableFunctions
                    let k = af.Key
                    from val in af.Value
                    select new { key = k, value = Formatter.Function(val, k) }).GroupBy(a => a.key)
                .ToDictionary(a => a.Key, a => new List<string>(a.Select(v => v.value)));
        }

        private BDDNode CreateAssignmentsBddNodes(Dictionary<string, bool> assignments,
            BDDManager manager, Dictionary<string, BDDNode> nodeStore, ref int i)
        {
            BDDNode assignmentsBDDNode = null;

            foreach (var a in assignments.ToList())
            {
                BDDNode bddNode = CreateNodeBasedOnAutomata(a.Key, a.Value, manager, i);
                i++;
                nodeStore[a.Key] = bddNode;
                assignmentsBDDNode = manager.AndSafe(assignmentsBDDNode, bddNode);
            }

            return assignmentsBDDNode;
        }

        public static BDDNode CreateNodeBasedOnAutomata
            (string key, bool value,
            BDDManager manager,
            int i)
        {
            BDDNode nodeBasedOnAutomata;
            if (value)
            {
                nodeBasedOnAutomata = manager.Create(i, manager.One, manager.Zero);
            }
            else
            {
                nodeBasedOnAutomata = manager.Create(i, manager.Zero, manager.One);
            }

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
            Dictionary<string, BDDNode> nodeStore)
        {
            var toDictionary = booleanNetwok.GroupBy(a => a.To);

            BDDNode seq = null;

            for (int i = 0; i < depth - 1; i++)
            {
                var ass = CreateFunctionApplication(manager, availableFunctions, toDictionary, i, nodeStore);
                seq = manager.AndSafe(seq, ass);
            }

            return seq;
        }

        private BDDNode CreateFunctionApplication(
            BDDManager manager,
            Dictionary<string, List<int>> availableFunctions,
            IEnumerable<IGrouping<string, GeneLink>> toDictionary,
            int i,
            Dictionary<string, BDDNode> nodeStore)
        {
            BDDNode res = null;

            toDictionary.ToList().ForEach(ff =>
            {
                BDDNode ass = null;
                var froms = ff.ToList();

                // can be null!!
                var to = Formatter.FormatParameter(ff.Key, i + 1);

                if (availableFunctions == null || !availableFunctions.ContainsKey(ff.Key))
                {
                    var from1 = Formatter.FormatParameter(ff.FirstOrDefault().From, i);
                    //TODO: NOT!
                    res = manager.Equal(nodeStore[from1], nodeStore[to]);
                }
                else // real functions
                {
                    var availableFunc = availableFunctions[ff.Key];
                    var funcAssignmentHelper =
                        new BddNodeFuncAssignmentHelper(manager, nodeStore);
                    BDDNode rightSide = null;

                    var leftSide =
                        BDDSharpSolver.CreateNodeBasedOnAutomata(
                            to, true, manager,
                            nodeStore[to].Index);

                    availableFunc.ForEach(f =>
                    {
                        ass =
                            funcAssignmentHelper.CreateFuncAssignment(to, froms, i, f);

                        ass = manager.And(nodeStore[Formatter.Function(f, ff.Key)], ass);
                        rightSide = manager.OrSafe(rightSide, ass);
                    });

                    //logger.Info("Right Side");
                    //LogToDotFormat(rightSide, manager, nodeStore);

                    res = manager.Equal(leftSide, rightSide);

                }
            });

            return res;
        }
    }
}
