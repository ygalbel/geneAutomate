using System;
using System.Collections.Generic;
using System.Linq;
using GeneAutomate.Models;
using PAT.Common.Classes.Expressions.ExpressionClass;
using UCLouvain.BDDSharp;

namespace GeneAutomate.BDD.BDDSharp
{
    public class BddNodeFuncAssignmentHelper : FuncAssignmentHelperBase<BDDNode>
    {
        private readonly BDDManager _manager;
        private readonly Dictionary<string, BDDNode> _nodes;

        public BddNodeFuncAssignmentHelper(BDDManager manager, Dictionary<string,BDDNode> nodes)
        {
            _manager = manager;
            _nodes = nodes;
        }
        public override BDDNode Or(BDDNode a, BDDNode b)
        {
            var bddNode = _manager.Or(a,b);
            return bddNode;
        }

        public override BDDNode And(BDDNode a, BDDNode b)
        {
            if (b == null)
            {
                return a;
            }

            var bddNode = _manager.And(a, b);
            return bddNode;
        }

        public override BDDNode CreateFuncAssignment(
            string to, List<GeneLink> froms,
            int i, int funcNumber)
        {
            BDDNodeFuncHelperInner func = new BDDNodeFuncHelperInner(to, froms, i, _manager, _nodes);
            return dict[funcNumber].Invoke(func);
        }
    }
}