using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneAutomate.Models;
using Newtonsoft.Json;
using GeneAutomate.BDD;
using NLog;

namespace GeneAutomate.BusinessLogic
{

    public class MergeResultCache
    {
        public Dictionary<string, List<GeneNode>> AlreadySeenMerges = new Dictionary<string, List<GeneNode>>();
    }

    public class AutomataMergeLogic
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        MergeResultCache _cache = new MergeResultCache();
        private readonly MergeLogicAlghoritms _mergeLogicAlghoritms = new MergeLogicAlghoritms();

        public List<GeneNode> GetValidMerges(GeneNode automata1, GeneNode automata2, GeneFullRules booleanNetwok)
        {
            var possibleMerges = GetMerges(automata1, automata2);

            logger.Info($"All merge (include invalids) is {possibleMerges.Count}");

            var validMerges = new List<GeneNode>();

            possibleMerges.ForEach(m =>
                {
                    if (IsBddValid(m, booleanNetwok))
                    {
                        logger.Info($"Merge for {m.NodeName} is valid");
                        validMerges.Add(m);
                    }
                    else
                    {
                        logger.Info($"Merge for {m.Path()} is not valid");
                    }

                });

            logger.Info($"Valid merges are {validMerges.Count}");


            return validMerges;
        }

        private bool IsBddValid(GeneNode geneNode, GeneFullRules booleanNetwok)
        {
            return NinjectHelper.Get<IBDDSolver>().IsValidPath(geneNode, booleanNetwok);
        }

        public List<GeneNode> GetMerges(List<GeneNode> nodes)
        {
            var merges = from n1 in nodes
                         from n2 in nodes
                         where n1 != n2
                         select GetMerges(n1, n2);

            return merges.SelectMany(a => a).ToList();
        }

        public List<GeneNode> GetValidMerges(List<GeneNode> nodes, GeneFullRules booleanNetwok)
        {
            var merges = from n1 in nodes
                         from n2 in nodes
                         where n1 != n2
                         select GetValidMerges(n1, n2, booleanNetwok);

            var validMerges = merges.SelectMany(a => a).ToList();

            TraceMerges(validMerges);

            return validMerges;
        }


        public void GetFinalMerges(Stack<GeneNode> availableNodes, 
            GeneFullRules booleanNetwok, 
            List<GeneNode> foundResults,
            BackTrackingNode node)
        {
            // search only 2 results
            if (foundResults.Count == 2)
            {
                return;
            }

            if (availableNodes.Count == 0)
            {
                return;
            }

            if (availableNodes.Count == 1)
            {
                var current = availableNodes.Pop();

                current = ApplyAllPossibleLoops(current, booleanNetwok);

                // add only if it's a real merge
                if (!string.IsNullOrEmpty(current.MergeName) && !foundResults.Any(a => a.MergeName == current.MergeName))
                {
                    node.IsFinal = true;
                    foundResults.Add(current);
                }

                return;
            }

            var currentStack = availableNodes.Clone();
            var first = currentStack.Pop();
            while (currentStack.Any())
            {
                var second = currentStack.Pop();

                if (AlreadyMerged(first, second))
                {
                    continue;
                }

                var merges = GetValidMerges(first, second, booleanNetwok);

                if (merges != null && merges.Any())
                {
                    var newStack = currentStack.Clone();
                    merges.ForEach(newStack.Push);

                    var currentNode = CreateBackTrackingNodeFromStack(newStack, node.Level + 1);
                    node.Nodes.Add(currentNode);
                    GetFinalMerges(newStack, booleanNetwok, foundResults, currentNode);
                }
            }

            var stackWithoutFirst = availableNodes.Clone();
            stackWithoutFirst.Pop();
            var newNode = CreateBackTrackingNodeFromStack(stackWithoutFirst, node.Level +1);
            node.Nodes.Add(newNode);
            GetFinalMerges(stackWithoutFirst, booleanNetwok, foundResults, newNode);
        }

        private bool AlreadyMerged(GeneNode first, GeneNode second)
        {
            return 
                !string.IsNullOrEmpty(first.MergeName) &&
                !string.IsNullOrWhiteSpace(second.MergeName) &&
                first.GetAllMergedExperiment().Intersect(second.GetAllMergedExperiment()).Any();
        }

        public static BackTrackingNode CreateBackTrackingNodeFromStack(Stack<GeneNode> newStack, int level)
        {
            return new BackTrackingNode() { Label = string.Join(",",newStack.Select(a => string.IsNullOrEmpty(a.MergeName) ? a.NodeName : a.MergeName)), Level = level};
        }

        private void TraceMerges(List<GeneNode> validMerges)
        {
            validMerges.ForEach(a =>
            {
                logger.Info($"{a.NodeName}\t{a.NodeLength}\t{a.Path()}");
            });
        }

        public List<GeneNode> GetMerges(GeneNode automata1, GeneNode automata2)
        {
            // check positive and negative merge algorithm
            return GetMerges(automata1, automata2, true)
                //.Union(GetMerges(automata1, automata2, false))
                .ToList();
        }

        public List<GeneNode> GetMerges(GeneNode automata1, GeneNode automata2, bool usePositiveAlgo)
        {
            var key = $"{automata1.MergeName} ~ {automata2.MergeName} - {usePositiveAlgo}";
            if (KeyAlreadyInCache(automata1, automata2, key))
            {
                logger.Info($"Already found {key} in cache");
                return _cache.AlreadySeenMerges[key];
            }

            var possibleMerges = new List<GeneNode>();

            var t1 = automata1;
            while (t1.Transitions != null && t1.Transitions.Any())
            {
                var merge = CreateMerge(automata2, t1, automata1, usePositiveAlgo);

                if (merge != null)
                {
                    possibleMerges.Add(merge);
                }

                t1 = t1.Transitions.First().Node;
            }

            var t2 = automata2?.Transitions?.FirstOrDefault()?.Node;
            while (t2 != null && t2.Transitions != null && t2.Transitions.Any())
            {
                var merge = CreateMerge(automata1, t2, automata2, usePositiveAlgo);

                if (merge != null)
                {
                    possibleMerges.Add(merge);
                }

                t2 = t2.Transitions.First().Node;
            }

            _cache.AlreadySeenMerges[key] = possibleMerges;
            return possibleMerges;
        }

        private bool KeyAlreadyInCache(GeneNode automata1, GeneNode automata2, string key)
        {
            return !string.IsNullOrWhiteSpace(automata1.MergeName) && !string.IsNullOrWhiteSpace(automata2.MergeName) &&
                   _cache.AlreadySeenMerges.ContainsKey(key);
        }

        public GeneNode CreateValidLoopMerge(GeneNode node, GeneFullRules booleanNetwok)
        {
            var cloned = CloneHelper.Clone(node);
            var t1 = cloned;
            var lastNode = FindLastNode(cloned);

            if (lastNode == null)
            {
                return null;
            }

            while (t1 != null && t1 != lastNode)
            {
                var newCondition = CreateMergedCondition(lastNode.CurrentCondition,
                    t1.CurrentCondition, true);



                if (newCondition != null)
                {
                    var currentNode = CloneHelper.Clone(cloned);

                    var tt1 = t1;
                    var tlastNode = lastNode;

                    t1 = currentNode.Find(t1);
                    lastNode = currentNode.Find(lastNode);
                    // now we can work on cloned

                    //          var mergeName = $"{t1.NodeName} ^ {lastNode.NodeName}";
                    //        t1.NodeName = mergeName;

                    ////  both sides take new condition
                    //t1.CurrentCondition = newCondition;
                    //lastNode.CurrentCondition = newCondition;

                    cloned = ApplyMergeToAllChildrens(tt1, tlastNode, cloned);

                    if (cloned != null && IsBddValid(cloned, booleanNetwok))
                    {
                        // set last node to "Looped" to allow more loops with other
                        tlastNode.IsInLoop = true;
                        cloned.MergeName = cloned.NodeName;
                        return cloned;
                        
                    }

                    cloned = currentNode;
                }

                t1 = t1.Transitions.First()?.Node;
            }

            return null;
        }

        private GeneNode ApplyMergeToAllChildrens(GeneNode tt1, GeneNode lastNode, GeneNode t1Head)
        {
            // direction is from last node to t1

            var t1 = tt1;
            var t2 = lastNode;
            while (t1 != null && t2 != null && t1 != lastNode)
            {
                
                var newCondition = CreateMergedCondition(t1.CurrentCondition,
                    t2.CurrentCondition, true);

                if (newCondition == null || AlreadyMergedNodes(t1, t2))
                {
                    return null;
                }

                //t1.Transitions.First().Condition = newCondition;
                var mergeName = $"{t1.NodeName} ^ {t2.NodeName}";
                t1.NodeName = mergeName;
                t1.CurrentCondition = newCondition;
                t2.CurrentCondition = newCondition;
                t1 = t1.Transitions?.First()?.Node;
                t2 = t2.Transitions?.First()?.Node;
            }

            return t1Head;
        }

        private static bool AlreadyMergedNodes(GeneNode t1, GeneNode t2)
        {
            var firstMerged = t1.NodeName.Split('^');
            
            var secondMerged = t2.NodeName.Split('^');

            return firstMerged.Intersect(secondMerged).Any();
        }

        public GeneNode ApplyAllPossibleLoops(GeneNode node, GeneFullRules booleanNetwok)
        {
            var current = node;
            GeneNode last = null;
            while (current != null)
            {
                last = current;
                current = CreateValidLoopMerge(current, booleanNetwok);

                LogLoop(node, current);
            }

            return last;
        }

        private static void LogLoop(GeneNode node, GeneNode current)
        {
            var builder = new StringBuilder();
            if (current != null)
            {
                current.AppendPath(builder);
            }
            logger.Info($"Get value of loop for {node.NodeName}, value is {builder}");
        }

        private GeneNode FindLastNode(GeneNode node)
        {
            var current = node;
            var last = current;
            while (current != null && !current.IsInLoop)
            {
                last = current;
                current = current.Transitions?.First().Node;
            }

            if (last != current)
            {
                return last;
            }

            return null;
        }

        public GeneNode CreateMerge(GeneNode node1, GeneNode node2, GeneNode node2Head,bool usePositiveAlgo)
        {
            var cloned = CloneHelper.Clone(node1);
            var t1 = cloned;
            GeneNode lastT1 = null;
            var t2 = node2;
            string mergeName = string.Empty;
            while (t1 != null && t2 != null)
            {
                var newCondition = CreateMergedCondition(t1.CurrentCondition,
                    t2.CurrentCondition, usePositiveAlgo);

                if (newCondition == null)
                {
                    return null;
                }

                //t1.Transitions.First().Condition = newCondition;
                var prefix = usePositiveAlgo ? string.Empty : "!";
                mergeName = $"{t1.NodeName} ~ {prefix}{t2.NodeName}";
                t1.NodeName = mergeName;
                t1.CurrentCondition = newCondition;
                lastT1 = t1;
                t1 = t1.Transitions?.First()?.Node;
                t2 = t2.Transitions?.First()?.Node;
            }

            // suffix
            if (t2?.Transitions != null && t2.Transitions.Any())
            {
                lastT1.Transitions = t2.Transitions;
            }

            // prefix
            if (node2 != node2Head) // node2 is not the first node in node2 automata, so we have
                                    // to add parents of node2
            {
                var cloned2 = CloneHelper.Clone(node2Head);
                var temp = cloned2;

                // find new head
                while (temp?.Transitions.First().Node.NodeName != node2.NodeName)
                {
                    temp = temp.Transitions.First().Node;
                }

                temp.Transitions.First().Node = cloned;

                cloned = cloned2;
            }

            if (cloned != null)
            {
                cloned.MergeName = mergeName;
            }

            return cloned;
        }

        //public bool CanMergeNode(GeneNode node1, GeneNode node2)
        //{
        //    if (node1 == null || node2 == null)
        //    {
        //        return true;
        //    }

        //    if (node1.Transitions.All(t => HaveMathingNode(t.Condition, node2))
        //        && node2.Transitions.All(t => HaveMathingNode(t.Condition, node1)))
        //    {
        //        return true;
        //    }

        //    return false;
        //}

     

        private Condition CreateMergedCondition(Condition c1, Condition c2, bool usePositiveStrategy)
        {
            if (c1 == null && c2 == null)
            {
                return null;
            }

            if (c1 == null || !c1.Any())
            {
                return c2;
            }

            if (c2 == null || !c2.Any())
            {
                return c1;
            }

            Condition mergedCondition;
            if (usePositiveStrategy)
            {
                mergedCondition = _mergeLogicAlghoritms.CreateWithPositiveLogic(c1, c2);
            }
            else
            {
                mergedCondition = _mergeLogicAlghoritms.CreateWithNegativeLogic(c1, c2);
            }

            return mergedCondition;
        }
    }

    public static class CloneHelper
    {
        public static T Clone<T>(T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }

    public static class GeneNodeHelper
    {
        public static GeneNode Find(this GeneNode source, GeneNode node)
        {
            var t = source;

            while (t != null)
            {
                if (t.NodeName == node.NodeName)
                {
                    return t;
                }

                t = t.Transitions?.First()?.Node;

            }

            return null;
        }

    }


}
