using System.Collections.Generic;
using System.Text.RegularExpressions;
using GeneAutomate.Models;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace GeneAutomate.BDD
{
    public class FuncAssignmentHelper
    {
        public Expression CreateFuncAssignment(string to, List<GeneLink> froms, int i, int funcNumber)
        {
            Expression pos = null;
            Expression neg = null;
            switch (funcNumber)
            {
                case 0:
                    pos = IsAllUp(to, froms.Positives(), i);
                    neg = IsAllDown(to, froms.Negatives(), i);
                    break;
                case 1:
                    pos = IsAnyUp(to, froms.Positives(), i);
                    neg = IsAllDown(to, froms.Negatives(), i);
                    break;
                case 2:
                    pos = IsAllUp(to, froms.Positives(), i);
                    neg = IsAnyUp(to, froms.Negatives(), i);
                    break;
                
            }

            if (pos == null)
            {
                return neg;
            }

            if (neg == null)
            {
                return pos;
            }
            
            return new PrimitiveApplication(PrimitiveApplication.AND, pos, neg);

        }

        public Expression IsAllUp(string to, List<GeneLink> froms, int i)
        {
            return AppyToAll(froms, i, true, PrimitiveApplication.AND);

        }


        public Expression IsAllDown(string to, List<GeneLink> froms, int i)
        {
            return AppyToAll(froms, i, false, PrimitiveApplication.AND);
        }

        public Expression IsAnyUp(string to, List<GeneLink> froms, int i)
        {
            return AppyToAll(froms, i, true, PrimitiveApplication.OR);

        }

        public Expression IsAnyDown(string to, List<GeneLink> froms, int i)
        {
            return AppyToAll(froms, i, false, PrimitiveApplication.OR);
        }

        public static int z = 0;

        private static Expression AppyToAll(List<GeneLink> froms, int i, 
            bool value, string func)
        {
            Expression app = null;

            froms.ForEach(f =>
            {
                var primitiveApplication = //BddHelper.SetBooleanValue(i, value, f.From); 
                new Variable(Formater.FormatParameter(f.From, i));
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

            return app;
        }
    }
}