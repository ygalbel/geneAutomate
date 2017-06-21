using System.Collections.Generic;

namespace GeneAutomate.Models
{
    public class Condition : Dictionary<string, bool?>
    {
        public string Name { get; set; }

        public bool IsFixedPoint { get; set; }
    }
}