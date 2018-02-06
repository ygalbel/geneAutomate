using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace GeneAutomate.Models
{
    public class GeneNode
    {
        public string NodeName { get; set; }

        public List<GeneTransition> Transitions { get; set; }

        public Condition CurrentCondition { get; set; }

        public int NodeLength
        {
            get { return 1 + ((Transitions != null) ? Transitions.First().Node.NodeLength : 0); }
        }

        /// <summary>
        /// Don't work with loops!!
        /// </summary>
        /// <param name="letters"></param>
        public void GetAllConditionLetters(List<string> letters)
        {
            var keys = CurrentCondition.Keys;

            foreach (var key in keys)
            {
                if (!letters.Contains(key))
                {
                    letters.Add(key);
                }
            }

            Transitions?.ForEach(f => f.Node.GetAllConditionLetters(letters));
        }

        public void AppendPath(StringBuilder builder)
        {
            this.Visit((a) => builder.Append($"{a.NodeName} => "));
        }

        public void Visit(Action<GeneNode> function)
        {
            function.Invoke(this);

            if (Transitions != null && Transitions.Any())
            {
                Transitions.ForEach(d => d.Node.Visit(function));
            }
        }

        public string MergeName { get; set; }
        public bool IsInLoop { get; set; }

        // find all merged node in curren
        public List<string> GetAllMergedExperiment()
        {
            if (string.IsNullOrEmpty(MergeName))
            {
                return null;
            }
            return MergeName.Split('~').Select(a => a.Split('_').FirstOrDefault()?.Replace('!',' ').Trim()).Distinct().ToList();
        }

        public List<string> GetAllMergedNodes()
        {
            if (string.IsNullOrEmpty(MergeName))
            {
                return null;
            }
            return MergeName.Split('~', '^').Select(a => a.Replace('!', ' ').Trim()).Distinct().ToList();
        }
        public string GetExperimentName()
        {
            return NodeName.Split('_').FirstOrDefault()?.Replace('!', ' ').Trim();
        }

        public bool IsLoopFixedPoint()
        {
            if (Transitions == null || !Transitions.Any())
            {
                return true;
            }

            var nextCondition = Transitions.First().Node.CurrentCondition;

            return CurrentCondition.Count == nextCondition.Count &&
                CurrentCondition.All(a => nextCondition.ContainsKey(a.Key) && nextCondition[a.Key] == a.Value) 
                   && Transitions.First().Node.IsLoopFixedPoint();
        }
    }

    public class GeneTransition
    {
        public GeneNode Node { get; set; }

        public Condition Condition { get; set; }
    }


    public static class NodeExtenstions
    {
        public static string Path(this GeneNode node)
        {
            StringBuilder builder = new StringBuilder();
            node.AppendPath(builder);
            return builder.ToString();
        }
    }
}
