using UCLouvain.BDDSharp;

namespace GeneAutomate.BDD
{
    public static class NodeFetcher
    {
        public static BDDNode CreateNodeBasedOnAutomata
        (string key, bool value,
            BDDManager manager,
            int i)
        {
            BDDNode nodeBasedOnAutomata;
            if (value)
            {
                nodeBasedOnAutomata = manager.Create(i, manager.One, manager.Zero);
            }
            else
            {
                nodeBasedOnAutomata = manager.Create(i, manager.Zero, manager.One);
            }

            return nodeBasedOnAutomata;
        }
    }
}