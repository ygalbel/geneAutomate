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

            List<string> overExpressed = new List<string>();
            List<string> knockoutGenes = new List<string>();

            for (int i = minTime; i <= maxTime; i++)
            {
                tempNode = new GeneNode()
                {
                    NodeName = exp.Name + "_" + i,
                    CurrentCondition = GetConditionForThisIndex(exp, i)

                };

                if (tempNode.CurrentCondition != null)
                {
                    overExpressed.AddRange(tempNode.CurrentCondition.OverExpressedGenes);
                    knockoutGenes.AddRange(tempNode.CurrentCondition.KnockedOutGenes);

                    if (knockoutGenes.Any())
                    {
                        int z = 0;
                    }

                    tempNode.CurrentCondition.OverExpressedGenes = overExpressed;
                    tempNode.CurrentCondition.KnockedOutGenes = knockoutGenes;
                }


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
