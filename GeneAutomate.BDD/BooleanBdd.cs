using PAT.Common.Classes.Expressions.ExpressionClass;

namespace GeneAutomate.BDD
{
    public class BddHelper
    {
        public static PrimitiveApplication SetBooleanValue(int i, bool value, string parameter)
        {
            return new PrimitiveApplication(PrimitiveApplication.EQUAL,
                new Variable(Formatter.FormatParameter(parameter, i)),
                new BoolConstant(value));
        }
    }
}