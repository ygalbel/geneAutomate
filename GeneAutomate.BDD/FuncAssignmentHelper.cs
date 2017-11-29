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
            FuncHelperInner func = new FuncHelperInner(to, froms,i);

            Expression pos = null;
            Expression neg = null;
            switch (funcNumber)
            {
                case -1:
                    return func.AllActivators();
                case -2:
                    return func.NoRepressors();
                case -3:
                    return func.NotNoActivators();
                case -4:
                    return func.NotAllRepressors();
                case 0:
                    return And(func.AllActivators(), func.NoRepressors());
                case 1:
                    return And(func.NotNoActivators(), func.NoRepressors());
                case 2:
                    return And(func.AllActivators(), func.NotAllRepressors());
                case 3:
                    var a = And(func.NoRepressors(), func.NotNoActivators());
                    var b = And(func.NotAllRepressors(), func.AllActivators());
                    return Or(a, b);
                case 4:
                    return func.AllActivators();
                case 5:
                    return Or(func.AllActivators(), And(func.NoRepressors(), func.NotNoActivators()));
                case 6:
                    return And(func.NotNoActivators(), func.NotAllRepressors());
                case 7:
                    return Or(And(func.NotNoActivators(), func.NotAllRepressors()), func.AllActivators());
                case 8:
                    return func.NotNoActivators();
                case 9:
                    return func.NoRepressors();
                case 10:
                    return Or(func.NoRepressors(), And(func.NotAllRepressors(), func.AllActivators()));
                case 11:
                    return Or(func.NoRepressors(), And(func.NotNoActivators(), func.NotAllRepressors()));
                case 12:
                    return func.NotAllRepressors();
                case 13:
                    return Or(func.NoRepressors(), func.AllActivators());
                case 14:
                    return Or(
                        Or(func.NoRepressors(),func.AllActivators()), 
                        And(func.NotAllRepressors(), func.NotNoActivators()));
                case 15:
                    return Or(func.NotAllRepressors(), func.AllActivators());
                case 16:
                    return Or(func.NoRepressors(), func.NotNoActivators());
                case 17:
                    return Or(func.NotAllRepressors(), func.NotNoActivators());
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


        public Expression Or(Expression a, Expression b)
        {
            return new PrimitiveApplication(PrimitiveApplication.OR, a, b);
        }

        public Expression And(Expression a, Expression b)
        {
            return new PrimitiveApplication(PrimitiveApplication.AND, a, b);
        }
    }


}