using System.Collections.Generic;
using GeneAutomate.Models;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace GeneAutomate.BDD
{
    public class FuncHelperInner
    {
        private readonly string _to;
        private readonly List<GeneLink> _froms;
        private readonly int _i;


        public FuncHelperInner(string to, List<GeneLink> froms, int i)
        {
            _to = to;
            _froms = froms;
            _i = i;
        }

        public Expression AllActivators()
        {
            return AppyToAll(_froms.Positives(), _i, true, PrimitiveApplication.AND);

        }

        public Expression NotNoActivators()
        {
            return AppyToAll(_froms.Positives(), _i, true, PrimitiveApplication.OR);

        }

        /// <summary>
        /// Any of them is true
        /// </summary>
        /// <param name="to"></param>
        /// <param name="froms"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public Expression NotAllRepressors()
        {
            return AppyToAll(_froms.Negatives(), _i, false, PrimitiveApplication.OR);
        }

        public Expression NoRepressors()
        {
            return AppyToAll(_froms.Negatives(), _i, false, PrimitiveApplication.AND);
        }

     

        private static Expression AppyToAll(List<GeneLink> froms, int i, 
            bool value, string func)
        {
            Expression app = null;

            froms.ForEach(f =>
            {
                var primitiveApplication = //BddHelper.SetBooleanValue(i, value, f.From); 
                   new PrimitiveApplication(PrimitiveApplication.EQUAL, new Variable(Formater.FormatParameter(f.From, i)), new BoolConstant(value));
                //BddHelper.SetBooleanValue(i, value, f.From);

                if (app == null)
                {
                    app = primitiveApplication;
                }
                else
                {
                    app = new PrimitiveApplication(func, app, primitiveApplication);
                }
            });

            //if (value == false)
            //{
            //    app = new PrimitiveApplication(PrimitiveApplication.NOT, app);
                
            //}
            return app;
        }
    }
}