using System.Collections.Generic;

namespace GeneAutomate.Models
{
    public class AutomateObject
    {
        public List<Node> nodes { get; set; }
        public List<Edge> edges { get; set; }
    }
}