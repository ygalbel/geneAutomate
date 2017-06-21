using System.Collections.Generic;

namespace GeneAutomate.Models
{
    public class Experiment
    {
        public string Name { get; set; }

        public Dictionary<int, Condition> Conditions;

        public Experiment()
        {
            Conditions = new Dictionary<int, Condition>();
        }

    }
}