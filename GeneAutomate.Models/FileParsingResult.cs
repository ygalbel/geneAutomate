using System.Collections.Generic;

namespace GeneAutomate.Models
{
    public class FileParsingResult
    {
        public List<GeneLink> GeneLinks { get; set; }

        public Dictionary<string, Condition> Conditions { get; set; } 
        public Dictionary<string, Experiment> Experiments { get; set; }
    }
}