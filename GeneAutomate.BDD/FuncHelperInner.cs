using System;
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
            return AppyToAll(_froms.Negatives(), _i, true, PrimitiveApplication.OR);
        }

        public Expression NoRepressors()
        {
            return AppyToAll(_froms.Negatives(), _i, false, PrimitiveApplication.AND);
        }


        


        public Expression OrPositiveIsTrue()
        {
            return AppyToAll(_froms.Positives(), _i, true, PrimitiveApplication.OR);
        }

        public Expression OrNegativeIsTrue()
        {
            return AppyToAll(_froms.Negatives(), _i, true, PrimitiveApplication.OR);
        }

        public Expression AndPositiveIsTrue()
        {
            return AppyToAll(_froms.Positives(), _i, true, PrimitiveApplication.AND);
        }



        private static Expression AppyToAll(List<GeneLink> froms, int i, 
            bool value, string func)
        {
            if (func == PrimitiveApplication.AND)
            {


                Expression app = null;

                froms.ForEach(f =>
                {
                    var formatParameter = Formater.FormatParameter(f.From, i);

                    Expression primitiveApplication =
                        new Variable(formatParameter);

                    if (app == null)
                    {
                        app = primitiveApplication;
                    }
                    else
                    {
                        app = new PrimitiveApplication(func, app, primitiveApplication);

                    }
                });

                if (!value)
                {
                    app = new PrimitiveApplication(PrimitiveApplication.NOT, app);
                }

                return new PrimitiveApplication(func, app);
            }
            else // or
            {

                Expression app = null;

                froms.ForEach(f =>
                {
                    var formatParameter = Formater.FormatParameter(f.From, i);

                    Expression primitiveApplication =
                        new Variable(formatParameter);
                    

                    if (app == null)
                    {
                        app = primitiveApplication;
                    }
                    else
                    {
                        app = new PrimitiveApplication(PrimitiveApplication.OR, 
                            app, 
                            primitiveApplication);

                    }
                });

                if (!value)
                {
                    app = new PrimitiveApplication(PrimitiveApplication.NOT, app);
                }

                return new PrimitiveApplication(PrimitiveApplication.OR, app, new BoolConstant(true));
            }


            return null;
        }
    }
}