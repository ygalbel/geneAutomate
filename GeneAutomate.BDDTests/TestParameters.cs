using GeneAutomate.Models;

namespace GeneAutomate.BDD.Tests
{
    public class TestParameters
    {
        public int CaseNumber { get; set; }
        public Condition FirstCondition { get; set; }

        public bool Expected_A_Value { get; set; }

        public bool IsValidPath { get; set; }
    }
}