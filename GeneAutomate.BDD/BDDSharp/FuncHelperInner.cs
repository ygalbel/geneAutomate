using System.Collections.Generic;
using System.Linq;
using GeneAutomate.Models;
using PAT.Common.Classes.Expressions.ExpressionClass;
using UCLouvain.BDDSharp;

namespace GeneAutomate.BDD.BDDSharp
{
    public class BDDNodeFuncHelperInner : FuncHelperInnerBase<BDDNode>
    {
        private const string OR = "OR";
        private const string AND = "AND";
        private readonly string _to;
        private readonly List<GeneLink> _froms;
        private readonly int _i;
        private readonly BDDManager _manager;
        private readonly Dictionary<string, BDDNode> _nodeStore;
        private readonly BDDNode _root;


        public BDDNodeFuncHelperInner(string to, List<GeneLink> froms, int i, BDDManager manager, Dictionary<string, BDDNode> nodeStore)
        {
            _to = to;
            _froms = froms;
            _i = i;
            _manager = manager;
            _nodeStore = nodeStore;
        }

        public override BDDNode AllActivators()
        {
            return AppyToAll(_froms.Positives(), _i, true, AND);

        }

        public override BDDNode NotNoActivators()
        {
            return AppyToAll(_froms.Positives(), _i, true, OR);

        }

        /// <summary>
        /// Any of them is true
        /// </summary>
        /// <param name="to"></param>
        /// <param name="froms"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public override BDDNode NotAllRepressors()
        {
            return AppyToAll(_froms.Negatives(), _i, false, AND);
        }

        public override BDDNode NoRepressors()
        {
            return AppyToAll(_froms.Negatives(), _i, false, OR);
        }

        public override BDDNode OrPositiveIsTrue()
        {
            return AppyToAll(_froms.Positives(), _i, true, OR);
        }
        public override BDDNode OrNegativeIsTrue()
        {
            return AppyToAll(_froms.Negatives(), _i, true, OR);
        }
        public override BDDNode AndPositiveIsTrue()
        {
            return AppyToAll(_froms.Positives(), _i, true, AND);
        }

        private BDDNode AppyToAll(List<GeneLink> froms, int i,
            bool value, string func)
        {
            BDDNode app = null;
            foreach (var f in froms)
            {
                var formatParameter = Formatter.FormatParameter(f.From, i);

                var node1 = FetchNode(formatParameter);

                if (func == OR)
                {
                    if (f.IsOptional)
                    {
                        var relationParameter = Formatter.OptionalRelation(f.From, _to.Split('_')[0]);
                        node1 = _manager.OrOptional(FetchNode(relationParameter), node1);
                    }

                    app = _manager.OrSafe(app, node1);
                }
                else
                if (func == AND)
                {
                    if (f.IsOptional)
                    {
                        var relationParameter = Formatter.OptionalRelation(f.From, _to.Split('_')[0]);
                        node1 = _manager.AndOptional(FetchNode(relationParameter), node1);
                    }

                    app = _manager.AndSafe(app, node1);
                }
            }

            if (!value && froms.Any())
            {
                app = _manager.Not(app);
            }

            return app;
        }

        private BDDNode FetchNode(string formatParameter)
        {
            var param = _nodeStore[formatParameter];

            var node1 =
                BDDSharpSolver.CreateNodeBasedOnAutomata(
                    formatParameter, true, _manager,
                    param.Index);
            return node1;
        }
    }
}