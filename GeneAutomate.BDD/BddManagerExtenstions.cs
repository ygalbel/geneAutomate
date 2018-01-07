using UCLouvain.BDDSharp;

namespace GeneAutomate.BDD
{
    public static class BddManagerExtenstions
    {
        public static BDDNode Not(this BDDManager manager, BDDNode node)
        {
            return manager.Negate(node);
        }

        public static BDDNode Equal(this BDDManager manager, BDDNode a, BDDNode b)
        {
            return manager.ITE(a, b, manager.Not(b));
        }
    }
}