using System.Collections.Generic;
using System.Linq;

namespace GeneAutomate.BusinessLogic
{
    public static class StackHelper
    {
        public static Stack<T> Clone<T>(this Stack<T> stack)
        {
            return new Stack<T>(stack.Reverse());
        }
    }
}