using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneAutomate.BDD.Tests
{
    [TestClass]
    public class FunctionBddSolverTestsSimpleFunctions : AbstractBddTest
    {


        private Dictionary<int, List<int>> resultValues = new Dictionary<int, List<int>>()
        {
            {-1, new List<int>()  {0, 0, 1, 0, 0, 1, 0, 0, 1}}, // all Activators
            {-2, new List<int>()  {1, 1, 1, 0, 0, 0, 0, 0, 0}}, // no repressesors
            {-3, new List<int>()  {0, 1, 1, 0, 1, 1, 0, 1, 1}}, // not no activators
            {-4, new List<int>()  {1, 1, 1, 1, 1, 1, 0, 0, 0}},// not all repressesors
        };

        [TestInitialize]
        public void TestInit()
        {
            base.TestInit();
        }



        [TestMethod]
        public void TestFullCaseNumber()
        {
            RunPositiveTest(resultValues);
        }




        [TestMethod]
        public void TestFullCaseNumberNegative()
        {
            RunNegativeTest(resultValues);
        }



    }
}