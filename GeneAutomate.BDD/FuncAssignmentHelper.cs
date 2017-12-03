using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GeneAutomate.Models;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace GeneAutomate.BDD
{
    public class FuncAssignmentHelper
    {
        public static Dictionary<int, Func<FuncHelperInner,Expression>> dict;

        private static void Init()
        {
            dict = new Dictionary<int, Func<FuncHelperInner,Expression>>()
            {
                {-1, (func) => func.AllActivators() },
                {-2, (func) => func.NoRepressors() },
                {-3, (func) => func.NotNoActivators() },
                {-4, (func) => func.NotAllRepressors() },
                {0, (func) => And(func.AllActivators(), func.NoRepressors()) },
                {1, (func) => And(func.NotNoActivators(), func.NoRepressors()) },
                {2, (func) => And(func.AllActivators(), func.NotAllRepressors())},
                {3, (func) => {var a = And(func.NoRepressors(), func.NotNoActivators());
                    var b = And(func.NotAllRepressors(), func.AllActivators());
                    return Or(a, b);} },
                {4, (func) => func.AllActivators() },
                {5, (func) => Or(func.AllActivators(), And(func.NoRepressors(), func.NotNoActivators())) },
                {6, (func) => And(func.NotNoActivators(), func.NotAllRepressors()) },
                {7, (func) => Or(And(func.NotNoActivators(), func.NotAllRepressors()), func.AllActivators()) },
                {8, (func) => func.NotNoActivators() },
                {9, (func) => func.NoRepressors() },
                {10, (func) => Or(func.NoRepressors(), And(func.NotAllRepressors(), func.AllActivators())) },
                {11, (func) => Or(func.NoRepressors(), And(func.NotNoActivators(), func.NotAllRepressors())) },
                {12, (func) => func.NotAllRepressors()},
                {13, (func) => Or(func.NoRepressors(), func.AllActivators())},
                {14, (func) =>  Or(
                        Or(func.NoRepressors(),func.AllActivators()),
                        And(func.NotAllRepressors(), func.NotNoActivators()))
        },
                {15, (func) => Or(func.NotAllRepressors(), func.AllActivators())},
                {16, (func) => Or(func.NoRepressors(), func.NotNoActivators())},
                {17, (func) => Or(func.NotAllRepressors(), func.NotNoActivators())},
            };
        }

        static FuncAssignmentHelper()
        {
            Init();
        }

        public Expression CreateFuncAssignment(
            string to, List<GeneLink> froms, 
            int i, int funcNumber)
        {
            FuncHelperInner func = new FuncHelperInner(to, froms,i);


            return dict[funcNumber].Invoke(func);
            //Expression pos = null;
            //Expression neg = null;
            //switch (funcNumber)
            //{
            //    case -1:
            //        return func.AllActivators();
            //    case -2:
            //        return func.NoRepressors();
            //    case -3:
            //        return func.NotNoActivators();
            //    case -4:
            //        return func.NotAllRepressors();
            //    case 0:
            //        return And(func.AllActivators(), func.NoRepressors());
            //    case 1:
            //        return And(func.NotNoActivators(), func.NoRepressors());
            //    case 2:
            //        return And(func.AllActivators(), func.NotAllRepressors());
            //    case 3:
            //        var a = And(func.NoRepressors(), func.NotNoActivators());
            //        var b = And(func.NotAllRepressors(), func.AllActivators());
            //        return Or(a, b);
            //    case 4:
            //        return func.AllActivators();
            //    case 5:
            //        return Or(func.AllActivators(), And(func.NoRepressors(), func.NotNoActivators()));
            //    case 6:
            //        return And(func.NotNoActivators(), func.NotAllRepressors());
            //    case 7:
            //        return Or(And(func.NotNoActivators(), func.NotAllRepressors()), func.AllActivators());
            //    case 8:
            //        return func.NotNoActivators();
            //    case 9:
            //        return func.NoRepressors();
            //    case 10:
            //        return Or(func.NoRepressors(), And(func.NotAllRepressors(), func.AllActivators()));
            //    case 11:
            //        return Or(func.NoRepressors(), And(func.NotNoActivators(), func.NotAllRepressors()));
            //    case 12:
            //        return func.NotAllRepressors();
            //    case 13:
            //        return Or(func.NoRepressors(), func.AllActivators());
            //    case 14:
            //        return Or(
            //            Or(func.NoRepressors(),func.AllActivators()), 
            //            And(func.NotAllRepressors(), func.NotNoActivators()));
            //    case 15:
            //        return Or(func.NotAllRepressors(), func.AllActivators());
            //    case 16:
            //        return Or(func.NoRepressors(), func.NotNoActivators());
            //    case 17:
            //        return Or(func.NotAllRepressors(), func.NotNoActivators());
            //}

            //if (pos == null)
            //{
            //    return neg;
            //}

            //if (neg == null)
            //{
            //    return pos;
            //}
            
            //return new PrimitiveApplication(PrimitiveApplication.AND, pos, neg);

        }


        public static Expression Or(Expression a, Expression b)
        {
            return new PrimitiveApplication(PrimitiveApplication.OR, a, b);
        }

        public static Expression And(Expression a, Expression b)
        {
            return new PrimitiveApplication(PrimitiveApplication.AND, a, b);
        }
    }


}