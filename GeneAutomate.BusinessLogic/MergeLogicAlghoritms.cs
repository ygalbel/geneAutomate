using System.Linq;
using GeneAutomate.Models;

namespace GeneAutomate.BusinessLogic
{
    public class MergeLogicAlghoritms
    {
        public Condition CreateWithPositiveLogic(Condition c1, Condition c2)
        {
            Condition mergedCondition = null;
            if (c1.All(v => !c2.ContainsKey(v.Key) || c2[v.Key] == c1[v.Key]) &&
                c2.All(v => !c1.ContainsKey(v.Key) || c2[v.Key] == c1[v.Key]) &&
                c1.IsFixedPoint == c2.IsFixedPoint)
            {
                mergedCondition = new Condition(); ;
                mergedCondition.IsFixedPoint = c1.IsFixedPoint;

                c1.ToList().ForEach(a => mergedCondition[a.Key] = a.Value);
                c2.ToList().ForEach(a => mergedCondition[a.Key] = a.Value);

            }

            return mergedCondition;
        }

        /// <summary>
        /// Negative logic mean in case that Keys that exist in c1 but not in c2,
        /// instead of set them to equality as I did in <seealso cref="CreateWithPositiveLogic"/>,
        /// I set them to inverted value of c2.
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public Condition CreateWithNegativeLogic(Condition c1, Condition c2)
        {
            // this case is already handled in CreateWithPositive function
            if (c1.All(v => c2.ContainsKey(v.Key)) && c2.All(v => c1.ContainsKey(v.Key)))
            {
                return null;
            }

            Condition mergedCondition = null;
            if (c1.All(v => !c2.ContainsKey(v.Key) || c2[v.Key] == c1[v.Key]) &&
                c2.All(v => !c1.ContainsKey(v.Key) || c2[v.Key] == c1[v.Key]) &&
                c1.IsFixedPoint == c2.IsFixedPoint)
            {
                mergedCondition = new Condition(); ;
                mergedCondition.IsFixedPoint = c1.IsFixedPoint;

                c1.ToList().ForEach(a => mergedCondition[a.Key] = a.Value);

                c2.Where(d => !c1.ContainsKey(d.Key))
                    .ToList()
                    .ForEach(a => mergedCondition[a.Key] = !a.Value);

            }

            return mergedCondition;
        }
    }
}