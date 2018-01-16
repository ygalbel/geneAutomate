using System.Collections.Generic;
using System.Linq;
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

        public static List<BDDNode> GetValidPath(this BDDManager manager, BDDNode root, List<BDDNode> current, bool? value)
        {
            var c = new List<BDDNode>(current);

            if (value.HasValue)
            {
                c.LastOrDefault().Value = value;
            }

            c.Add(new BDDNode() {Index = root.Index} );

            if (root == manager.One)
            {
                return current;
            }
            else if (root == manager.Zero)
            {
                return null;
            }

            return GetValidPath(manager, root.Low, c, false) ?? GetValidPath(manager, root.High, c, true);
        }

    //    public static bool FindOneValidPath(this BDDManager manager, List<int> res, BDDNode node)
    //    {
    //        if (node == null)
    //        {
    //            return false;
    //        }

    //        var current = new List<int>(res);
    //        current.Add(node.Index);
    //        List<int> temp = null;

    //         var rr = manager.FindOneValidPath(current, node.Low);

    //        if (!rr)
    //        {
    //            current =new List<int>(res);
    //            rr = manager.FindOneValidPath(current, node.High);
    //        }

    //        if (temp == null && node.High != null)
    //        {
    //            temp = FindOneValidPath(current, node.High);
    //        }

    //        if (node.High == null && node.Low == null)
    //        {
    //            if(node == manager.One)
    //        }

    //        return temp;
    //    }
    }

    

}