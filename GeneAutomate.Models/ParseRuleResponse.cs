using System.Collections.Generic;
using GeneAutomate.Models;

namespace GeneAutomate.Parser
{
    public class ParseRuleResponse
    {
        public Dictionary<string, Condition> Conditions = new Dictionary<string, Condition>();

        public Dictionary<string, Experiment> Experiments = new Dictionary<string, Experiment>();
    }
}