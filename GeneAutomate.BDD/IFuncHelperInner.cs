using PAT.Common.Classes.Expressions.ExpressionClass;

namespace GeneAutomate.BDD
{
    public abstract class FuncHelperInnerBase<T>
    {
        public abstract T AllActivators();
        public abstract T AndPositiveIsTrue();
        public abstract T NoRepressors();
        public abstract T NotAllRepressors();
        public abstract T NotNoActivators();
        public abstract T OrNegativeIsTrue();
        public abstract T OrPositiveIsTrue();
    }
}