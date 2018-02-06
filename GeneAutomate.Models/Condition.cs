using System.Collections.Generic;

namespace GeneAutomate.Models
{
    public class Condition : Dictionary<string, bool?>
    {
        public string Name { get; set; }

        public bool IsFixedPoint { get; set; }

        public List<string> OverExpressedGenes { get; set; } = new List<string>();

        public List<string> KnockedOutGenes { get; set; } = new List<string>();
    }
}