using System;
using System.Collections.Generic;
using System.Linq;
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

        public void Visit(Action<GeneNode> function)
        {
            function.Invoke(this);

            if (Transitions != null && Transitions.Any())
            {
                Transitions.ForEach(d => d.Node.Visit(function));
            }
        }
    }

    public class GeneTransition
    {
        public GeneNode Node { get; set; }

        public Condition Condition { get; set; }
    }
}
