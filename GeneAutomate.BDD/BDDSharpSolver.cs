using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
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

        public bool IsValidPath(GeneNode automata, 
            GeneFullRules booleanNetwok, int depth=40)
        {
            depth = Math.Min(depth, automata.NodeLength);
            logger.Info($"Choosed depth is {depth}");
            var nodeStore = new Dictionary<string, BDDNode>();
            var letters = new List<string>();
            //depth = 40; //automata.NodeLength; 50;

            int z = 0;

            //int bucketNodeCounts = CalculateBucketCounts(letters, booleanNetwok);
            var manager = new BDDManager(1000);
            automata.GetAllConditionLetters(letters);

            letters =
            letters.SelectMany(l => Enumerable.Range(0, depth).ToList().Select(n => Formatter.FormatParameter(l, n)))
                .ToList();

            logger.Info(string.Join(",", letters));

            var assignments = BDDLogicHelper.CreateDictBasedOnAutomata(automata);
            logger.Info(string.Join(",", assignments.Select(a => $"{a.Key} == {a.Value}")));
            nodeStore["1"] = manager.One;
            nodeStore["0"] = manager.Zero;

            BDDNode functionNodes = CreateFunctionKeys(booleanNetwok.Functions, manager, nodeStore);
            CreateOptionalKeys(manager, booleanNetwok.GeneLinks, nodeStore);
            AddMissingNodes(manager, nodeStore, letters);

            var relationsBDD =
                CreateExpressionsFromBooleanNetwork(manager, booleanNetwok.GeneLinks, booleanNetwok.Functions, depth, nodeStore);

            relationsBDD = manager.And(relationsBDD, functionNodes);

            var assignmentsBDDNode = 
                CreateAssignmentsBddNodes(assignments, 
                                          manager, 
                                          nodeStore);

            var root = manager.And(relationsBDD, assignmentsBDDNode);
            logger.Info(manager.ToDot(root, show_all: false, labelFunction: node => nodeStore.FirstOrDefault(a => a.Value.Index == node.Index).Key));
            //LogToDotFormat(root, manager, nodeStore);
            root = manager.Reduce(root);
            //// LOG PART
            logger.Info("after reduce");
            //LogToDotFormat(root, manager, nodeStore);

            //logger.Info("relations");
            //logger.Info(manager.ToDot(relations, show_all: false));

            logger.Info("root");
            

            logger.Info("count is: " + nodeStore.Count + " nextId: " +  manager.nextId);
           /* var truth =
                BuildThruthTable(manager, root, nodeStore.Count - 2); // remove zero and one

            assignments.ToList().ForEach(a =>
            {
                var index = nodeStore[a.Key].Index;
                truth = truth.Where(d =>
                {
                    return d.Key[index] == (a.Value ? '1' : '0');
                }).ToDictionary(b => b.Key, b => b.Value);
            });*/

            //var pathes = truth.Where(a => a.Value).ToList();

           // LogValidPathes(pathes, nodeStore);
            var path = manager.GetValidPath(root, new List<BDDNode>(),  null) ?? Enumerable.Empty<BDDNode>();
            LogValidPathes(path, nodeStore);
            return path.Any();

            //       CheckThruthTable(truth, res);

            //return true;
        }

        private void LogValidPathes(IEnumerable<BDDNode> path, Dictionary<string, BDDNode> nodeStore)
        {
            //logger.Info("Found " + pathes.Count + " pathes");
            //pathes.ForEach(p =>
            //{
                logger.Info("========================");
                LogValidPath(path, nodeStore);
            //});
        }


        private void LogValidPath(IEnumerable<BDDNode> path, Dictionary<string, BDDNode> nodeStore)
        {
            if (path == null)
            {
                return;
            }

            logger.Info("Valid path simple:");
            int i = 0;

            foreach (var ch in path)
            {
                logger.Info($"{nodeStore.First(a => a.Value.Index == ch.Index).Key} - {ch.Value}");
                i++;
            }

            logger.Info($"=============================");
        }

        private void AddMissingNodes(
            BDDManager manager, 
            Dictionary<string, BDDNode> nodeStore, 
            List<string> letters)
        {
            var alreadyCreated = nodeStore.Keys;
            foreach (var l in letters.Where(a => !alreadyCreated.Contains(a)).ToList())
            {
                var node = CreateNode(manager,1,1);
                nodeStore[l] = node;
            }
        }

        private int nodeIndex = 0;

        private static object locker1 = new object();

        public BDDNode CreateNode(BDDManager manager, int high, int low)
        {
            lock (locker1)
            {
                return manager.Create(nodeIndex++, high, low);
            }
        }


        public BDDNode CreateNodeWithIndex(BDDManager manager, int high, int low, int index)
        {
            lock (locker1)
            {
                return manager.Create(index, high, low);
            }
        }

        private void CreateOptionalKeys(BDDManager manager, List<GeneLink> booleanNetwok, Dictionary<string, BDDNode> nodeStore)
        {
            foreach (var geneLink in booleanNetwok)
            {
                if (geneLink.IsOptional)
                {
                    var key = Formatter.OptionalRelation(geneLink.From, geneLink.To);
                    var node = CreateNode(manager, 1, 0);
                    nodeStore[key] = node;
                }
            }
        }

        private BDDNode CreateFunctionKeys(Dictionary<string, List<int>> availableFunctions,
            BDDManager manager,
            Dictionary<string, BDDNode> nodeStore)
        {
            BDDNode functionNodes = null;
            var functionsKeys = CreateFunctionsKeys(availableFunctions);
            foreach (var f in functionsKeys.ToList())
            {
                logger.Info("start function of " + f.Key);
                BDDNode currentNodeOr = null;
                List<BDDNode> currents = new List<BDDNode>();
                foreach (var d in f.Value)
                {
                    var c = CreateNode(manager,1, 0);
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
            Dictionary<int, string> reverseHash = nodeStore.Where(a => a.Key != "1" && a.Key != "0").ToDictionary(a => a.Value.Index, a => a.Key);

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
            BDDManager manager, Dictionary<string, BDDNode> nodeStore)
        {
            BDDNode assignmentsBDDNode = null;


            foreach (var a in assignments.ToList())
            {
                logger.Info("assignments of " + a.Key);
                var index = nodeStore[a.Key].Index;
                BDDNode bddNode = CreateNewNodeBasedOnAutomata(a.Key, a.Value, index,manager);
                nodeStore[a.Key] = bddNode;
                assignmentsBDDNode = manager.AndSafe(assignmentsBDDNode, bddNode);
            }

            return assignmentsBDDNode;
        }

        public BDDNode CreateNewNodeBasedOnAutomata
        (string key, bool value, int index,
            BDDManager manager)
        {
            BDDNode nodeBasedOnAutomata;
            if (value)
            {
                nodeBasedOnAutomata = CreateNodeWithIndex(manager, 1, 0, index);
            }
            else
            {
                nodeBasedOnAutomata = CreateNodeWithIndex(manager, 0, 1, index);
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

            for(int i=0; i < depth - 1; i++)
            {
                var ass = CreateFunctionApplication(manager, availableFunctions, toDictionary, i, nodeStore);
                seq = manager.AndSafe(seq, ass);
            }
            ;
            

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

                    var leftSide = NodeFetcher.CreateNodeBasedOnAutomata(
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
