using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GeneAutomate.Models;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace GeneAutomate.BDD
{


    public class FuncAssignmentHelper : FuncAssignmentHelperBase<Expression>
    {
        public override Expression Or(Expression a, Expression b)
        {
            return new PrimitiveApplication(PrimitiveApplication.OR, a, b);
        }

        public override Expression And(Expression a, Expression b)
        {
            return new PrimitiveApplication(PrimitiveApplication.AND, a, b);
        }

        public override Expression CreateFuncAssignment(
            string to, List<GeneLink> froms,
            int i, int funcNumber)
        {
                FuncHelperInner func = new FuncHelperInner(to, froms, i);
                return dict[funcNumber].Invoke(func);
        }
    }

    

    public abstract class FuncAssignmentHelperBase<T>
    {
        public static Dictionary<int, Func<FuncHelperInnerBase<T>, T>> dict;

        protected void Init()
        {
            dict = new Dictionary<int, Func<FuncHelperInnerBase<T>, T>>()
            { { 47, (func) => func.AndPositiveIsTrue()},
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

        public FuncAssignmentHelperBase()
        {
            Init();
        }

        public abstract T CreateFuncAssignment(
            string to, List<GeneLink> froms,
            int i, int funcNumber);
        


        public abstract T Or(T a, T b);

        public abstract T And(T a, T b);
    }


}