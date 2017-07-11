using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneAutomate.Models;

namespace GeneAutomate.BusinessLogic
{
    public class AutomataFromExperimentCreator
    {
        public GeneNode CreateAutomata(Experiment exp)
        {
            int minTime = exp.Conditions.Min(a => a.Key);
            int maxTime = exp.Conditions.Max(a => a.Key);

            GeneNode node = null;
            GeneNode tempNode = null;
            GeneNode fatherNode = null;

            for (int i = minTime; i <= maxTime; i++)
            {
                tempNode = new GeneNode()
                {
                    NodeName = exp.Name + "_" + i,
                    

                };

                // not firstCase
                if (fatherNode != null)
                {
                    fatherNode.Transitions = new List<GeneTransition>();

                    fatherNode.Transitions.Add(new GeneTransition()
                    {
                        Condition = GetConditionForThisIndex(exp, i),
                        Node = tempNode
                    });
                }
                else // keep first node
                {
                    node = tempNode;
                }

                fatherNode = tempNode;
            }

            return node;
        }

        private static Condition GetConditionForThisIndex(Experiment exp, int i)
        {
            return exp.Conditions.ContainsKey(i) ? exp.Conditions[i] : new Condition();
        }
    }
}
