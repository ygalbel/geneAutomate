using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneAutomate.Models;
using Newtonsoft.Json;

namespace GeneAutomate.BusinessLogic
{
    public class AutomataMergeLogic
    {
        BooleanNetworkValidator validator = new BooleanNetworkValidator();

        public List<GeneNode> GetValidMerges(GeneNode automata1, GeneNode automata2, List<GeneLink> booleanNetwok)
        {
            var possibleMerges = GetMerges(automata1, automata2);

            possibleMerges = possibleMerges.Where(m => validator.IsValidAutomata(m, null, booleanNetwok)).ToList();

            return possibleMerges;
        }

        public List<GeneNode> GetMerges(List<GeneNode> nodes)
        {
            var merges = from n1 in nodes
                from n2 in nodes
                where n1 != n2
                select GetMerges(n1, n2);

            return merges.SelectMany(a => a).ToList();
        }

        public List<GeneNode> GetMerges(GeneNode automata1, GeneNode automata2)
        {
            var possibleMerges = new List<GeneNode>();

            var t1 = automata1;
            while (t1.Transitions != null && t1.Transitions.Any())
            {
                var merge = CreateMerge(t1, automata2);

                if (merge != null)
                {
                    possibleMerges.Add(merge);
                }

                t1 = t1.Transitions.First().Node;
            }

            var t2 = automata2?.Transitions?.FirstOrDefault()?.Node;
            while (t2 != null && t2.Transitions != null && t2.Transitions.Any())
            {
                var merge = CreateMerge(automata1, t2);

                if (merge != null)
                {
                    possibleMerges.Add(merge);
                }

                t2 = t2.Transitions.First().Node;
            }

            return possibleMerges;
        }

        public GeneNode CreateMerge(GeneNode node1, GeneNode node2)
        {
            var cloned = CloneHelper.Clone(node1);
            var t1 = cloned;
            var t2 = node2;
            while (t1.Transitions != null && t1.Transitions.Any() &&  t2.Transitions != null && t2.Transitions.Any())
            {
                var newCondition = CreateMergedCondition(t1.CurrentCondition,
                    t2.CurrentCondition);

                if (newCondition == null)
                {
                    return null;
                }

                t1.Transitions.First().Condition = newCondition;
                t1.NodeName = t1.NodeName + " merged " + t2.NodeName;
                t1.CurrentCondition = newCondition;
                t1 = t1.Transitions.First().Node;
                t2 = t2.Transitions.First().Node;
            }

            if (t2.Transitions != null && t2.Transitions.Any())
            {
                t1.Transitions = t2.Transitions;
            }

            return cloned;
            ;
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
