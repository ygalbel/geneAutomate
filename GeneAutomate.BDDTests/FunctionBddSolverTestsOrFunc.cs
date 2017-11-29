using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeneAutomate.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneAutomate.BDD.Tests
{
    public class TetParameters
    {
        public int CaseNumber { get; set; }
        public Condition FirstCondition { get; set; }

        public bool Expected_A_Value { get; set; }

        public bool IsValidPath { get; set; }
    }


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
            var solver = new BDDSolver();
            resultValues.ToList().ForEach((rv) =>
                {
                    TetsParameters.ForEach(tp =>
                    {
                        var functionNum = rv.Key;
                        var expectedValue = rv.Value[tp.CaseNumber];


                        var log = $" function num: {functionNum}, case number {tp.CaseNumber}, expectedValue : {expectedValue}";
                        logger.Info(log);
                        Trace.WriteLine(log);
                        var firstCondition = tp.FirstCondition;

                        var secondCondition = new Condition() { { "a", expectedValue == 1 } };

                        var automata = TestHelper.CreateAutomataWithConditions(firstCondition, secondCondition);

                        var booleanNetwork = CreateBooleanNetwork();

                        var availableFunctions = new Dictionary<string, List<int>>() { { "a", new List<int>() { functionNum } } };
                        var res = solver.IsValidPath(automata, booleanNetwork, availableFunctions);
                        Assert.IsTrue(res);
                    });
                }
            );
            
        }


        [TestMethod]
        public void TestFullCaseNumberNegative()
        {
            var solver = new BDDSolver();
            resultValues.ToList().ForEach((rv) =>
            {
                TetsParameters.ForEach(tp =>
                {
                    var functionNum = rv.Key;
                    var expectedValue = rv.Value[tp.CaseNumber];


                    var log = $" function num: {functionNum}, case number {tp.CaseNumber}, expectedValue : {expectedValue}";
                    logger.Info(log);
                    Trace.WriteLine(log);
                    var firstCondition = tp.FirstCondition;

                    var secondCondition = new Condition() { { "a", expectedValue == 0 } }; /**CHANGE1**/

                    var automata = TestHelper.CreateAutomataWithConditions(firstCondition, secondCondition);

                    var booleanNetwork = CreateBooleanNetwork();

                    var availableFunctions = new Dictionary<string, List<int>>() { { "a", new List<int>() { functionNum } } };
                    var res = solver.IsValidPath(automata, booleanNetwork, availableFunctions);
                    Assert.IsFalse(res); /** Change2 **/
                });
            }
            );

        }

        private static List<GeneLink> CreateBooleanNetwork()
        {
            return new List<GeneLink>()
            {
                new GeneLink() {From = "b", To = "a", IsPositive = true},
                new GeneLink() {From = "c", To = "a", IsPositive = true},
                new GeneLink() {From = "d", To = "a", IsPositive = false},
                new GeneLink() {From = "e", To = "a", IsPositive = false},

            };
        }
        
    }

    

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
            var solver = new BDDSolver();
            resultValues.ToList().ForEach((rv) =>
            {
                TetsParameters.ForEach(tp =>
                {
                    var functionNum = rv.Key;
                    var expectedValue = rv.Value[tp.CaseNumber];


                    var log = $" function num: {functionNum}, case number {tp.CaseNumber}, expectedValue : {expectedValue}";
                    logger.Info(log);
                    Trace.WriteLine(log);
                    var firstCondition = tp.FirstCondition;

                    var secondCondition = new Condition() { { "a", expectedValue == 1 } };

                    var automata = TestHelper.CreateAutomataWithConditions(firstCondition, secondCondition);

                    var booleanNetwork = CreateBooleanNetwork();

                    var availableFunctions = new Dictionary<string, List<int>>() { { "a", new List<int>() { functionNum } } };
                    var res = solver.IsValidPath(automata, booleanNetwork, availableFunctions);
                    Assert.IsTrue(res);
                });
            }
            );

        }


        [TestMethod]
        public void TestFullCaseNumberNegative()
        {
            var solver = new BDDSolver();
            resultValues.ToList().ForEach((rv) =>
            {
                TetsParameters.ForEach(tp =>
                {
                    var functionNum = rv.Key;
                    var expectedValue = rv.Value[tp.CaseNumber];


                    var log = $" function num: {functionNum}, case number {tp.CaseNumber}, expectedValue : { expectedValue == 0 }";
                    logger.Info(log);
                    Trace.WriteLine(log);
                    var firstCondition = tp.FirstCondition;

                    var secondCondition = new Condition() { { "a", expectedValue == 0 } }; /**CHANGE1**/

                    var automata = TestHelper.CreateAutomataWithConditions(firstCondition, secondCondition);

                    var booleanNetwork = CreateBooleanNetwork();

                    var availableFunctions = new Dictionary<string, List<int>>() { { "a", new List<int>() { functionNum } } };
                    var res = solver.IsValidPath(automata, booleanNetwork, availableFunctions);
                    Assert.IsFalse(res); /** Change2 **/
                });
            }
            );

        }

        private static List<GeneLink> CreateBooleanNetwork()
        {
            return new List<GeneLink>()
            {
                new GeneLink() {From = "b", To = "a", IsPositive = true},
                new GeneLink() {From = "c", To = "a", IsPositive = true},
                new GeneLink() {From = "d", To = "a", IsPositive = false},
                new GeneLink() {From = "e", To = "a", IsPositive = false},

            };
        }

    }

}