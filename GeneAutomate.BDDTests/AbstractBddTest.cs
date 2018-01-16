using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GeneAutomate.BusinessLogic;
using GeneAutomate.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NLog;

namespace GeneAutomate.BDD.Tests
{
    public abstract class AbstractBddTest
    {
        public List<TestParameters> TetsParameters = new List<TestParameters>()
        {
            new TestParameters()
            {
                CaseNumber = 0,
                FirstCondition =  new Condition()
                {
                    { "b", false }, {"c", false }, {"d", false }, {"e" ,false }
                },
                Expected_A_Value = true,
                IsValidPath = false
            },
            new TestParameters()
            {
                CaseNumber = 1,
                FirstCondition =  new Condition()
                {
                    { "b", true }, {"c", false }, {"d", false },  { "e", false }
                },
                Expected_A_Value = true,
                IsValidPath = true
            },
            new TestParameters()
            {
                CaseNumber = 2,
                FirstCondition =  new Condition()
                {
                    { "b", true }, {"c", true }, {"d", false },  { "e", false }
                },
                Expected_A_Value = true,
                IsValidPath = true
            },
            new TestParameters()
            {
                CaseNumber = 3,
                FirstCondition =  new Condition()
                {
                    { "b", false }, {"c", false }, {"d", true },  { "e", false }
                },
                Expected_A_Value = true,
                IsValidPath = false
            },
            new TestParameters()
            {
                CaseNumber = 4,
                FirstCondition =  new Condition()
                {
                    { "b", true }, {"c", false }, {"d", true },  { "e", false }
                },
                Expected_A_Value = true,
                IsValidPath = false
            },
            new TestParameters()
            {
                CaseNumber = 5,
                FirstCondition =  new Condition()
                {
                    { "b", true }, {"c", true }, {"d", true },  { "e", false }
                },
                Expected_A_Value = true,
                IsValidPath = true
            },
            new TestParameters()
            {
                CaseNumber = 6,
                FirstCondition =  new Condition()
                {
                    { "b", false }, {"c", false }, {"d", true },  { "e", true }
                },
                Expected_A_Value = true,
                IsValidPath = false
            },
            new TestParameters()
            {
                CaseNumber = 7,
                FirstCondition =  new Condition()
                {
                    { "b", true }, {"c", false }, {"d", true },  { "e", true }
                },
                Expected_A_Value = true,
                IsValidPath = false
            },
            new TestParameters()
            {
                CaseNumber = 8,
                FirstCondition =  new Condition()
                {
                    { "b", true }, {"c", true }, {"d", true },  { "e", true }
                },
                Expected_A_Value = true,
                IsValidPath = false
            }
        };

        public void RunNegativeTest(Dictionary<int, List<int>> resultValues)
        {
            var fault = new List<Tuple<int, int>>();
            var solver = NinjectHelper.Get<IBDDSolver>();
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
                        var res = solver.IsValidPath(automata,
                            new GeneFullRules() { GeneLinks = booleanNetwork, Functions = availableFunctions });
                        if (res)
                        {
                            fault.Add(new Tuple<int, int>(functionNum, tp.CaseNumber));
                            logger.Warn("failed in this case");
                        }
                    });
                }
            );
            Assert.IsTrue(fault.Count == 0, JsonConvert.SerializeObject(fault)); /** Change2 **/
        }

        protected void RunPositiveTest(Dictionary<int, List<int>> resultValues)
        {
            var fault = new List<Tuple<int, int>>();

            var solver = NinjectHelper.Get<IBDDSolver>();
            resultValues.ToList().ForEach((rv) =>
                {
                    TetsParameters.ForEach( tp =>
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
                        var res = solver.IsValidPath(automata,
                            new GeneFullRules() { GeneLinks = booleanNetwork, Functions = availableFunctions });
                        if (!res)
                        {
                            fault.Add(new Tuple<int, int>(functionNum, tp.CaseNumber));
                            logger.Warn("failed in this case");
                        }
                    });
                }
            );

            Assert.IsTrue(fault.Count == 0, JsonConvert.SerializeObject(fault)); /** Change2 **/

        }

        protected static List<GeneLink> CreateBooleanNetwork()
        {
            return new List<GeneLink>()
            {
                new GeneLink() {From = "b", To = "a", IsPositive = true},
                new GeneLink() {From = "c", To = "a", IsPositive = true},
                new GeneLink() {From = "d", To = "a", IsPositive = false},
                new GeneLink() {From = "e", To = "a", IsPositive = false},

            };
        }

        protected static List<GeneLink> CreateBooleanNetworkWithOptional()
        {
            return new List<GeneLink>()
            {
                new GeneLink() {From = "b", To = "a", IsPositive = true},
                new GeneLink() {From = "c", To = "a", IsPositive = true, IsOptional = true},
                new GeneLink() {From = "d", To = "a", IsPositive = false},
                new GeneLink() {From = "e", To = "a", IsPositive = false, IsOptional = true},

            };
        }

        protected static Logger logger = LogManager.GetCurrentClassLogger();

        protected TestContext m_testContext;

        public TestContext TestContext

        {

            get { return m_testContext; }

            set { m_testContext = value; }

        }

        public void TestInit()
        {
            logger.Info($"start test {TestContext.TestName}");
        }

    }
}