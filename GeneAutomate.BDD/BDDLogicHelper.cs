using System.Collections.Generic;
using System.Linq;
using GeneAutomate.Models;

namespace GeneAutomate.BDD
{
    public class BDDLogicHelper
    {
        public static Dictionary<string,bool> CreateDictBasedOnAutomata(GeneNode automata)
        {
            Dictionary<string, bool> result = new Dictionary<string, bool>();

            if (automata == null || automata.Transitions == null || !automata.Transitions.Any())
            {
                return null;
            }

            int i = 0;
            automata.Visit(l =>
            {
                var tr = BDDSolver.GetTransitions(l);

                if (tr == null)
                {
                    return;

                }

                tr
                    .ForEach(
                        f =>
                        {
                            var key = Formater.FormatParameter(f.Key, i);
                            var value = f.Value.Value;

                            result[key] = value;
                        });
                i++;
            });


            return result;
        }
    }
}