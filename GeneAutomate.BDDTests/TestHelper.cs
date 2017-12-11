using System.Collections.Generic;
using GeneAutomate.Models;

namespace GeneAutomate.BDD.Tests
{
    public class TestHelper
    {
        public static GeneNode CreateAutomataWithConditions(Condition firstCondition, Condition secondCondition)
        {
            var automata = new GeneNode()
            {
                CurrentCondition = firstCondition,
                NodeName = "n0",
                Transitions = new List<GeneTransition>()
                {
                    new GeneTransition()
                    {
                        Node = new GeneNode()
                        {
                            CurrentCondition = secondCondition,
                            NodeName = "n1"
                        }
                    }
                }
            };
            return automata;
        }
    }
}