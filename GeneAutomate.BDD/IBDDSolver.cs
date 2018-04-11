using System.Collections.Generic;
using GeneAutomate.Models;

namespace GeneAutomate.BDD
{
    public interface IBDDSolver
    {
        bool IsValidPath(GeneNode automata, GeneFullRules booleanNetwos, int depth=40);
    }
}