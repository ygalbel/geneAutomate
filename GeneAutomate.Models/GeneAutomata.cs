using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneAutomate.Models
{
    // TODO: change structure to Node and Transition, to allow loops
    public class GeneNode
    {
        public string NodeName { get; set; }

        public List<GeneTransition> Transitions { get; set; }
    }

    public class GeneTransition
    {
        public GeneNode Node { get; set; }

        public Condition Condition { get; set; }
    }
}
