using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneAutomate.Models
{
    public class BackTrackingNode 
    {
        public string Label { get; set; }

        public bool IsFinal { get; set; }

        public List<BackTrackingNode> Nodes { get; set; } = new List<BackTrackingNode>();

        public int Level { get; set; }
        

        public void Visit(Action<BackTrackingNode> function)
        {
            function.Invoke(this);

            if (Nodes != null && Nodes.Any())
            {
                Nodes.ForEach(d => d.Visit(function));
            }
        }
    }
}