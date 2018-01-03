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


        public BDDNodeFuncHelperInner(string to, List<GeneLink> froms, int i, BDDManager manager, Dictionary<string, BDDNode> nodeStore, BDDNode root)
        {
            _to = to;
            _froms = froms;
            _i = i;
            _manager = manager;
            _nodeStore = nodeStore;
            _root = root;
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
            return AppyToAll(_froms.Negatives(), _i, true, OR);
        }

        public override BDDNode NoRepressors()
        {
            return AppyToAll(_froms.Negatives(), _i, false, AND);
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
                var formatParameter = Formater.FormatParameter(f.From, i);

                var param = _nodeStore[formatParameter];

                var node1 =
                    BDDSharpSolver.CreateNodeBasedOnAutomata(
                        formatParameter, true, _manager,
                     param.Index);

                if (func == OR)
                {
                    if (app == null)
                    {
                        app = node1;
                    }
                    else
                    {
                        app = _manager.Or(app, node1);
                    }
                }
                else
                if (func == AND)
                {
                    if (app == null)
                    {
                        app = node1;
                    }
                    else
                    {
                        app = _manager.And(app, node1);
                    }
                }
            }

            if (!value)
            {
                app = _manager.Not(app);
            }

            return app;
        }
    }
}