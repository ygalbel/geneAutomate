using System;
using System.Collections.Generic;
using System.Linq;
using GeneAutomate.Models;

namespace GeneAutomate.BDD
{
    public static class ListGeneNodeExtensions
    {
        public static List<GeneLink> Positives(this List<GeneLink> links)
        {
            return links.Where(a => a.IsPositive).ToList();
        }

        public static List<GeneLink> Negatives(this List<GeneLink> links)
        {
            return links.Where(a => !a.IsPositive).ToList();
        }
    }
}