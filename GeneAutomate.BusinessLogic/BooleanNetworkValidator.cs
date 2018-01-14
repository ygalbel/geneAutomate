//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using GeneAutomate.Models;

//namespace GeneAutomate.BusinessLogic
//{
//    public class BooleanNetworkValidator
//    {

//        public bool IsValidAutomata(GeneNode automata, Condition currentStatus, List<GeneLink> booleanNetwok, HashSet<GeneNode> alreayViewedNodes= null)
//        {
//            if (automata == null || automata.Transitions == null || !automata.Transitions.Any())
//            {
//                return true;
//            }

//            if (alreayViewedNodes == null)
//            {
//                alreayViewedNodes = new HashSet<GeneNode>();
//            }

//            // handle case of loop in first node
//            if (currentStatus != null)
//            {
                
//                alreayViewedNodes.Add(automata);
//            }

//            var isValid = automata.Transitions.All(t => 
//            IsValidTransition(currentStatus, t.Condition, booleanNetwok) && 
//                    (alreayViewedNodes.Contains(t.Node) || 
//                    IsValidAutomata(t.Node, t.Condition, booleanNetwok, alreayViewedNodes)));

//            return isValid;
//        }

//        private bool IsValidTransition(Condition currentStatus, Condition newStatus, List<GeneLink> booleanNetwok)
//        {
//            if (currentStatus == null)
//            {
//                return true;
//            }

//            var importantRules = booleanNetwok.Where(a => !a.IsOptional).ToList();

//            var isFine = importantRules.All(r =>
//            {
//                var c = currentStatus.FirstOrDefault(a => a.Key == r.From);
//                var n = newStatus.FirstOrDefault(a => a.Key == r.To);

//                if (!c.Value.HasValue || !n.Value.HasValue)
//                {
//                    return true;

//                }

//                if (r.IsPositive)
//                {
//                    return c.Value.Value == n.Value.Value;
//                }
//                else
//                {
//                    return c.Value.Value == !n.Value.Value;
//                }
//            });


//            return isFine;
//        }

       
//    }
//}
