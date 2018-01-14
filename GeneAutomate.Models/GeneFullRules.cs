using System.Collections.Generic;

namespace GeneAutomate.Models
{
    public class GeneFullRules
    {
        public List<GeneLink> GeneLinks { get; set; }

        public Dictionary<string, List<int>> Functions { get; set; }
    }
}