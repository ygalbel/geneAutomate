using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneAutomate.BDD.Tests
{
    [TestClass]
    public class FunctionBddSolverTestsComplexFunctions : AbstractBddTest
    {

        private Dictionary<int, List<int>> resultValues = new Dictionary<int, List<int>>()
        {
            {0, new List<int>()  {0, 0, 1, 0, 0, 0, 0, 0, 0}},
            {1, new List<int>()  {0, 1, 1, 0, 0, 0, 0, 0, 0}},
            {2, new List<int>()  {0, 0, 1, 0, 0, 1, 0, 0, 0}},
            {3, new List<int>()  {0, 1, 1, 0, 0, 1, 0, 0, 0}},
            {4, new List<int>()  {0, 0, 1, 0, 0, 1, 0, 0, 1}},
            {5, new List<int>()  {0, 1, 1, 0, 0, 1, 0, 0, 1}},
            {6, new List<int>()  {0, 1, 1, 0, 1, 1, 0, 0, 0}},
            {7, new List<int>()  {0, 1, 1, 0, 1, 1, 0, 0, 1}},
            {8, new List<int>()  {0, 1, 1, 0, 1, 1, 0, 1, 1}},
            {9, new List<int>()  {1, 1, 1, 0, 0, 0, 0, 0, 0}},
            {10, new List<int>() {1, 1, 1, 0, 0, 1, 0, 0, 0}},
            {11, new List<int>() {1, 1, 1, 0, 1, 1, 0, 0, 0}},
            {12, new List<int>() {1, 1, 1, 1, 1, 1, 0, 0, 0}},
            {13, new List<int>() {1, 1, 1, 0, 0, 1, 0, 0, 1}},
            {14, new List<int>() {1, 1, 1, 0, 1, 1, 0, 0, 1}},
            {15, new List<int>() {1, 1, 1, 1, 1, 1, 0, 0, 1}},
            {16, new List<int>() {1, 1, 1, 0, 1, 1, 0, 1, 1}},
            {17, new List<int>() {1, 1, 1, 1, 1, 1, 0, 1, 1}},
        };

        [TestInitialize]
        public void TestInit()
        {
            base.TestInit();
        }



        [TestMethod]
        public void TestFullCaseNumber()
        {
            var resultValues = this.resultValues;
            RunPositiveTest(resultValues);
        }

        


        [TestMethod]
        public void TestFullCaseNumberNegative()
        {
            RunNegativeTest(resultValues);
        }

        

        
        
    }
}