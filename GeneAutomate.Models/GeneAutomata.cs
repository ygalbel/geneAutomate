﻿using System;
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

        public int NodeLength
        {
            get { return 1 + ((Transitions != null) ? Transitions.First().Node.NodeLength : 0); }
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