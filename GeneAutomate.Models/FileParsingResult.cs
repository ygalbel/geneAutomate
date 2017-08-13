using System.Collections.Generic;

namespace GeneAutomate.Models
{
    public class FileParsingResult
    {
        public List<GeneLink> GeneLinks { get; set; }

        public Dictionary<string, Condition> Conditions { get; set; } 
        public Dictionary<string, Experiment> Experiments { get; set; }
        public Dictionary<string,AutomateObject> Automates { get; set; }
        public List<AutomateObject> Merges { get; set; }

        public List<AutomateObject> AllMerges { get; set; }
    }
}