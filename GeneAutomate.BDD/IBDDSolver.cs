using System.Collections.Generic;
using GeneAutomate.Models;

namespace GeneAutomate.BDD
{
    public interface IBDDSolver
    {
        bool IsValidPath(GeneNode automata, List<GeneLink> booleanNetwok, 
            Dictionary<string, List<int>> availableFunctions = null);
    }
}