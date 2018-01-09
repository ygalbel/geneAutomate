using UCLouvain.BDDSharp;

namespace GeneAutomate.BDD
{
    public static class BddManagerExtensions
    {
        public static BDDNode Not(this BDDManager manager, BDDNode node)
        {
            return manager.Negate(node);
        }

        public static BDDNode Equal(this BDDManager manager, BDDNode a, BDDNode b)
        {
            return manager.ITE(a, b, manager.Not(b));
        }

        public static BDDNode OrSafe(this BDDManager manager, BDDNode node, BDDNode second)
        {
            if (node == null)
            {
                node = second;
            }
            else
            {
                node = manager.Or(node, second);
            }

            return node;
        }

        public static BDDNode AndSafe(this BDDManager manager, BDDNode node, BDDNode second)
        {
            if (node == null)
            {
                node = second;
            }
            else
            {
                node = manager.And(node, second);
            }

            return node;
        }

        public static BDDNode AndOptional(this BDDManager manager, 
            BDDNode relation, BDDNode second)
        {
            return manager.ITE(relation, second, manager.One);
        }

        public static BDDNode OrOptional(this BDDManager manager,
            BDDNode relation, BDDNode second)
        {
            return manager.And(relation, second);
        }


    }

    

}