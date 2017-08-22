using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneAutomate.Models;
using Newtonsoft.Json;
using GeneAutomate.BDD;

namespace GeneAutomate.BusinessLogic
{
    public class MergeResultCache
    {
        public Dictionary<string, List<GeneNode>> AlreadySeenMerges = new Dictionary<string, List<GeneNode>>();
    }

    public class AutomataMergeLogic
    {
        BooleanNetworkValidator validator = new BooleanNetworkValidator();

        MergeResultCache _cache = new MergeResultCache();

        public List<GeneNode> GetValidMerges(GeneNode automata1, GeneNode automata2, List<GeneLink> booleanNetwok)
        {
            var possibleMerges = GetMerges(automata1, automata2);

            Trace.WriteLine($"All merge (include invalids) is {possibleMerges.Count}");

            var validMerges = new List<GeneNode>();

            possibleMerges.ForEach(m =>
                {
                    if (validator.IsValidAutomata(m, null, booleanNetwok) && IsBddValid(m, booleanNetwok))
                    {
                        Trace.WriteLine($"Merge for {m.NodeName} is valid");
                        validMerges.Add(m);
                    }
                    else
                    {
                        Trace.WriteLine($"Merge for {m.Path()} is not valid");
                    }

                });

            Trace.WriteLine($"Valid merges are {validMerges.Count}");


            return validMerges;
        }

        private bool IsBddValid(GeneNode geneNode, List<GeneLink> booleanNetwok)
        {
            return new BDDSolver().IsValidPath(geneNode, booleanNetwok);
        }

        public List<GeneNode> GetMerges(List<GeneNode> nodes)
        {
            var merges = from n1 in nodes
                from n2 in nodes
                where n1 != n2
                select GetMerges(n1, n2);

            return merges.SelectMany(a => a).ToList();
        }

        public List<GeneNode> GetValidMerges(List<GeneNode> nodes, List<GeneLink> booleanNetwok)
        {
            var merges = from n1 in nodes
                         from n2 in nodes
                         where n1 != n2
                         select GetValidMerges(n1, n2, booleanNetwok);

            var validMerges = merges.SelectMany(a => a).ToList();

            TraceMerges(validMerges);

            return validMerges;
        }


        public void GetFinalMerges(Stack<GeneNode> availableNodes, List<GeneLink> booleanNetwok, List<GeneNode> foundResults)
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
                foundResults.Add(availableNodes.Pop());
                return;
            }

            var currentStack = new Stack<GeneNode>(availableNodes);
            var first = currentStack.Pop();
            bool found = false;
            while (currentStack.Any())
            {
                var second = currentStack.Pop();

                var merges = GetValidMerges(first, second, booleanNetwok);

                if (merges != null && merges.Any())
                {
                    var newStack = new Stack<GeneNode>(currentStack);
                    merges.ForEach(newStack.Push);

                    GetFinalMerges(newStack, booleanNetwok, foundResults);
                    found = true;
                }
            }

            var stackWithoutFirst = new Stack<GeneNode>(availableNodes);
            GetFinalMerges(stackWithoutFirst, booleanNetwok, foundResults);


        }

        private void TraceMerges(List<GeneNode> validMerges)
        {
            validMerges.ForEach(a =>
            {
                Trace.WriteLine($"{a.NodeName}\t{a.NodeLength}\t{a.Path()}");
            });
        }

        public List<GeneNode> GetMerges(GeneNode automata1, GeneNode automata2)
        {
            var key = $"{automata1.MergeName} ~ {automata2.MergeName}";
            if (!string.IsNullOrWhiteSpace(automata1.MergeName) && !string.IsNullOrWhiteSpace(automata2.MergeName) &&
                _cache.AlreadySeenMerges.ContainsKey(key))
            {
                Trace.WriteLine($"Already found {key} in cahce");
                return _cache.AlreadySeenMerges[key];
            }

            var possibleMerges = new List<GeneNode>();

            var t1 = automata1;
            while (t1.Transitions != null && t1.Transitions.Any())
            {
                var merge = CreateMerge(automata2, t1, automata1);

                if (merge != null)
                {
                    possibleMerges.Add(merge);
                }

                t1 = t1.Transitions.First().Node;
            }

            var t2 = automata2?.Transitions?.FirstOrDefault()?.Node;
            while (t2 != null && t2.Transitions != null && t2.Transitions.Any())
            {
                var merge = CreateMerge(automata1, t2, automata2);

                if (merge != null)
                {
                    possibleMerges.Add(merge);
                }

                t2 = t2.Transitions.First().Node;
            }

            _cache.AlreadySeenMerges[key] = possibleMerges;
            return possibleMerges;
        }

        public GeneNode CreateMerge(GeneNode node1, GeneNode node2, GeneNode node2Head)
        {
            var cloned = CloneHelper.Clone(node1);
            var t1 = cloned;
            GeneNode lastT1 = null;
            var t2 = node2;
            string mergeName = string.Empty;
            while (t1 != null &&  t2 != null)
            {
                var newCondition = CreateMergedCondition(t1.CurrentCondition,
                    t2.CurrentCondition);

                if (newCondition == null)
                {
                    return null;
                }

                //t1.Transitions.First().Condition = newCondition;
                mergeName = t1.NodeName + " ~ " + t2.NodeName;
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

        private Condition HaveMathingNode(Condition condition, GeneNode node2)
        {
            return
                node2.Transitions.Select(a => CreateMergedCondition(condition, a.Condition)).FirstOrDefault(a => a != null);
        }

        private Condition CreateMergedCondition(Condition c1, Condition c2)
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

            if (c1.All(v => !c2.ContainsKey(v.Key) || c2[v.Key] == c1[v.Key]) &&
                c2.All(v => !c1.ContainsKey(v.Key) || c2[v.Key] == c1[v.Key]) &&
                c1.IsFixedPoint == c2.IsFixedPoint)
            {
                var mergedCondition = new Condition();

                mergedCondition.IsFixedPoint = c1.IsFixedPoint;

                c1.ToList().ForEach(a => mergedCondition[a.Key] = a.Value);
                c2.ToList().ForEach(a => mergedCondition[a.Key] = a.Value);

                return mergedCondition;
            }

            return null;
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
}
